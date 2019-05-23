using SuperSocket.SocketBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SSCommonLib;

namespace Server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private MyServer appServer;
        private int port = 8089;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (appServer != null && appServer.State == SuperSocket.SocketBase.ServerState.Running)
                return;
            var config = new SuperSocket.SocketBase.Config.ServerConfig()
            {
                Name = "SuperSocketServer",
                ServerTypeName = "SuperSocketServer",
                ClearIdleSession = false, //60秒执行一次清理90秒没数据传送的连接
                ClearIdleSessionInterval = 60,
                IdleSessionTimeOut = 90,
                MaxRequestLength = 10000, //最大包长度
                Ip = "Any",
                Port = port,
                MaxConnectionNumber = 10000,//最大允许的客户端连接数目
            };
            appServer = new MyServer(app_NewSessionConnected, app_SessionClosed);
            //移除请求处理方法的注册，因为它和命令不能同时被支持：
            appServer.NewRequestReceived -= App_NewRequestReceived;
            appServer.NewRequestReceived += App_NewRequestReceived;
            appServer.Setup(config);
            if (!appServer.Start())
            {
                txbReceive.AppendText("初始化服务失败" + '\n');
            }
        }
        //客户端断开
        void app_SessionClosed(MySession session, CloseReason value)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                txbReceive.AppendText($"客户端{session.SessionID}已断开，原因：{value.ToString()}" + '\n');
            }));
        }
        //客户端连接
        void app_NewSessionConnected(MySession session)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                txbReceive.AppendText($"客户端{session.SessionID}已连接" + '\n');
            }));
        }
        //接收客户端消息
        private void App_NewRequestReceived(MySession session, MyRequestInfo requestInfo)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (requestInfo == null) return;
                if (!requestInfo.IsHeart)
                    txbReceive.AppendText($"收到{session.SessionID}消息：{requestInfo.Body}" + '\n');
                //是否显示心跳包
                else if (cbIgnoreHeart.IsChecked == false)
                {
                    txbReceive.AppendText($"收到{session.SessionID}心跳：{requestInfo.Body}" + '\n');
                }
                //发送心跳反馈
                if (requestInfo.IsHeart && cbSendHeart.IsChecked == true)
                {
                    var msg = CommandBuilder.BuildHeartCmd();
                    if (session.Connected)
                        session.Send(msg, 0, msg.Length);
                }
            }));
        }
        //发送消息
        protected bool Send(string message)
        {
            if (appServer != null && appServer.State == SuperSocket.SocketBase.ServerState.Running && !string.IsNullOrEmpty(message))
            {
                foreach (var item in appServer.GetAllSessions())
                {
                    if (item.Connected)
                    {
                        var msg = CommandBuilder.BuildMsgCmd(message);
                        item.Send(msg, 0, msg.Length);
                    }
                }
                return true;
            }
            return false;
        }

        private void btnSendClear_Click(object sender, RoutedEventArgs e)
        {
            txbSend.Text = string.Empty;
        }
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            Send(txbSend.Text);
        }

        private void btnRecClear_Click(object sender, RoutedEventArgs e)
        {
            txbReceive.Text = String.Empty;
        }

        private void TxbReceive_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            txbReceive.ScrollToEnd();
        }
    }
}
