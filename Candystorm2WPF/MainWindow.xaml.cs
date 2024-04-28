using RobotUICSharp;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace Candystorm2WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VideoPlayerWindow videoPlayerWindow;
        SerialConnector SerialConnect;
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Candystorm_V2_Loaded(object sender, RoutedEventArgs e)
        {
            videoPlayerWindow = new VideoPlayerWindow("AnimationA.mp4", "AnimationB.mp4");
            videoPlayerWindow.Show();
            videoPlayerWindow.PlaybackFinishedEvent += PlaybackFinishedHandler;


            SerialConnect = new SerialConnector();
            autoConnectSerial();
        }

        private void PlaybackFinishedHandler(object? sender, EventArgs e)
        {
            Console.WriteLine("Playbackfinished event processed");
            turnHardwareOff();
        }

        private void VideoTest_Click(object sender, RoutedEventArgs e)
        {
            turnHardwareOn();
            videoPlayerWindow.playVideoSingle();
        }

        private void HardwareOn_Click(object sender, RoutedEventArgs e)
        {
            turnHardwareOn();
        }

        private void HardwareOff_Click(object sender, RoutedEventArgs e)
        {
        turnHardwareOff();
        }

        void turnHardwareOn()
        {
            if (SerialConnect.isOpen())
            {
                SerialConnect.sendMessage("<SYS-001>");
            }
        }

        void turnHardwareOff()
        {
            if (SerialConnect.isOpen())
            {
                SerialConnect.sendMessage("<SYS-000>");
            }
        }


        private void autoConnectSerial()
        {
            String[] AvailablePorts = SerialPort.GetPortNames();
            try
            {
                SerialConnect.setPort(AvailablePorts[0]);

                SerialConnect.openPort();
                SerialConnect.MessageReceivedEvent += ReceiveMessage;

                ArduinoLabel.Background = Brushes.Green;
                ArduinoLabel.Text = "Connected on Port: " + SerialConnect.getPort();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ArduinoLabel.Background = Brushes.Red;
            }
            if (AvailablePorts.Length == 0)
            {
                ArduinoLabel.Background = Brushes.Red;
                ArduinoLabel.Text = "No COM Devices";
            }
        }

        private void ReceiveMessage(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message);
            String rec = ""; 
            try
            {
                rec = e.Message.Substring(0, 6);

         
                if (compareStrings(rec, "<0001>"))
                {
                    videoPlayerWindow.Dispatcher.Invoke(new Action(() => { videoPlayerWindow.playVideoSingle(); }));
                   
                    turnHardwareOn();
                    

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        bool compareStrings(String A, String B)
        {
            bool toReturn = false;
            if (A.Length == B.Length)
            {
                for (int i = 0; i < A.Length; i++)
                {
                    if (A[i] == B[i])
                    {
                        toReturn = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return toReturn;
            }
            else
            {
                return false;
            }
        }


    }
}
