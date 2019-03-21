using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public MainWindow()
        {

            new Thread(Loop).Start();
        }

        public void Loop()
        {
            Controller c = new Controller(UserIndex.One);
            JcRobotNetworking net = new JcRobotNetworking(JcRobotNetworking.ConnectionType.Controller);
            net.Connect(1296,"10.9.0.3");
            if (c == null)
            {
                
            }
            while (c.IsConnected)
            {
                State s = c.GetState();
                net.SendCommand(new JcRobotNetworking.Command(1, BitConverter.GetBytes(s.Gamepad.LeftThumbX / (float)short.MaxValue / 2f + .5f)));
                net.SendCommand(new JcRobotNetworking.Command(2, BitConverter.GetBytes(((float)s.Gamepad.RightTrigger-s.Gamepad.LeftTrigger) / 255f)));
                Console.WriteLine(s.Gamepad.LeftThumbX / (float)short.MaxValue / 2f + .5f);
                Console.WriteLine(((float)s.Gamepad.RightTrigger - s.Gamepad.LeftTrigger) / 255f);
                Thread.Sleep(20);
            }
        }
    }
}
