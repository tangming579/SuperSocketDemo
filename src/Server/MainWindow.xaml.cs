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

namespace Server
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private MyServer appServer;
        private int port;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (appServer != null && appServer.State == SuperSocket.SocketBase.ServerState.Running)
                return;
            else if (appServer != null)
            {
                appServer.Dispose();
            }

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
                
            }
        }
        //客户端断开
        void app_SessionClosed(MySession session, CloseReason value)
        {
            
        }
        //客户端连接
        void app_NewSessionConnected(MySession session)
        {
           
        }
        //接收客户端消息
        private void App_NewRequestReceived(MySession session, MyRequestInfo requestInfo)
        {
            if (requestInfo == null) return;

        }
        //发送消息
        protected bool Send(string message)
        {
            if (appServer != null && appServer.State == SuperSocket.SocketBase.ServerState.Running && !string.IsNullOrEmpty(message))
            {
                foreach (var item in appServer.GetAllSessions())
                {
                    if (item.Connected)
                        item.Send(message);
                }
                return true;
            }
            else return false;
        }
        protected bool Send(byte[] data)
        {
            if (appServer != null && appServer.State == SuperSocket.SocketBase.ServerState.Running && data.Length > 0)
            {
                foreach (var item in appServer.GetAllSessions())
                {
                    try
                    {
                        if (item.Connected)
                            item.Send(data, 0, data.Length);
                    }
                    catch
                    {

                    }
                    finally
                    {
                        //Console.WriteLine($"发送的消息：" + '\n' + DataConverter.ByteToHex(data, data.Length));
                    }
                }
                return true;
            }
            else return false;
        }
    }
}
