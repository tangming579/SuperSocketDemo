using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using SuperSocket.ClientEngine;

namespace Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static EasyClient<MyPackageInfo> client = null;
        static System.Timers.Timer timer = null;
        private int port = 8080;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            client = new EasyClient<MyPackageInfo>();
            client.Initialize(new MyReceiveFilter());
            client.Connected += OnClientConnected;
            client.NewPackageReceived += Client_NewPackageReceived;
            client.Error += OnClientError;
            client.Closed += OnClientClosed;

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += new System.Timers.ElapsedEventHandler((s, x) =>
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //心跳包
                    if (client.IsConnected && cbSendHeart.IsChecked == true)
                    {
                        var heartMsg = CommandBuilder.BuildHeartCmd();
                        client.Send(heartMsg);
                    }
                    //断线重连
                    else if (!client.IsConnected)
                    {
                        client.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
                    }
                }));

            });
            timer.Enabled = true;
            timer.Start();
        }
        private void OnClientConnected(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                txbReceive.AppendText("已连接" + '\n');
            }));
        }
        private void OnClientClosed(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                txbReceive.AppendText("已断开" + '\n');
            }));
        }
        private void OnClientError(object sender, ErrorEventArgs e)
        {

        }
        private void Client_NewPackageReceived(object sender, PackageEventArgs<MyPackageInfo> e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {

            }));
        }
        private void btnSendClear_Click(object sender, RoutedEventArgs e)
        {
            txbSend.Clear();
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnRecClear_Click(object sender, RoutedEventArgs e)
        {
            txbReceive.Text = String.Empty;
        }
    }
}
