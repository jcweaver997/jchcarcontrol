using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace jchcarcontrol
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpNetworking net;
        private static MainWindow instance;
        private Thread loopThread;

        public MainWindow()
        {

        }

        public void OnMessageReceive(string message)
        {
            Log(message);
        }


        public static void Log(string message)
        {
            instance.Dispatcher.Invoke(() =>
            {
                instance.textLog.AppendText(message + "\n");
                instance.textLog.ScrollToEnd();
            });
        }



        public void Loop()
        {
            Controller c = new Controller(UserIndex.One);
            JcRobotNetworking jcRobotNetworking = new JcRobotNetworking(JcRobotNetworking.ConnectionType.Controller);
            jcRobotNetworking.Connect(1296,"10.9.0.3");
            
            if (c == null)
            {
                Log("controller not connected");
            }else if (c.IsConnected)
            {
                Log("controller is connected");
            }
            else
            {
                Log("controller??");
            }
            while (c.IsConnected)
            {
                State s = c.GetState();
                jcRobotNetworking.SendCommand(new JcRobotNetworking.Command(1, BitConverter.GetBytes(s.Gamepad.LeftThumbX / (float)short.MaxValue / 2f + .5f)));
                jcRobotNetworking.SendCommand(new JcRobotNetworking.Command(2, BitConverter.GetBytes(((float)s.Gamepad.RightTrigger-s.Gamepad.LeftTrigger) / 255f)));
                Console.WriteLine(s.Gamepad.LeftThumbX / (float)short.MaxValue / 2f + .5f);
                Console.WriteLine(((float)s.Gamepad.RightTrigger - s.Gamepad.LeftTrigger) / 255f);
                Thread.Sleep(20);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            net.Send("scan");
            Log("sent scan request");
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            instance = this;
            net = new TcpNetworking(new IPEndPoint(IPAddress.Parse("10.9.0.3"), 1296), OnMessageReceive);
            new Thread(() => {
                try
                {
                    net.Start();
                    Log("Done connecting");
                }
                catch (SocketException se)
                {
                    Log("Error connecting: " + se.Message);
                }


            }).Start();
            loopThread = new Thread(Loop);
            loopThread.Start();
            Log("Started ");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            net.Close();
            loopThread.Abort();
            Environment.Exit(0);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            net.Send("crack "+crackNumber.Text);
            Log("sent crack request");
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            net.Send("stop");
            Log("sent stop request");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            textLog.Text = "";
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            net.Send("list");
            Log("sent list request");
        }
    }
}
