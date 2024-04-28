using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using static RobotUICSharp.SerialConnector;

namespace Candystorm2WPF
{
    /// <summary>
    /// Interaktionslogik für VideoPlayerWindow.xaml
    /// </summary>
    public partial class VideoPlayerWindow : Window
    {
        bool isSingleVideoPlaying = false;

        string loopFileName = "";
        string singleFileName = "";

        public event EventHandler PlaybackFinishedEvent;

        public VideoPlayerWindow(string loopVideoPath, string singleVideoPath)
        {
            InitializeComponent();
            loopFileName = loopVideoPath;
            singleFileName = singleVideoPath;
            playVideoLoop();



        }

        public void playVideoLoop()
        {
            isSingleVideoPlaying= false;
            VideoPlayer.Source = new Uri(System.IO.Directory.GetCurrentDirectory() + "/" + loopFileName);
            VideoPlayer.Play();
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
        }

        public void playVideoSingle()
        {
            isSingleVideoPlaying=true;
            VideoPlayer.Source = new Uri(System.IO.Directory.GetCurrentDirectory() + "/" + singleFileName);
            VideoPlayer.Play();
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
        }

        private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (isSingleVideoPlaying)
            {
                playVideoLoop();
                PlaybackFinishedEvent.Invoke(this, new EventArgs());
            }
            else
            {
                VideoPlayer.Position = new TimeSpan(0, 0, 1);
                VideoPlayer.Play();
            }
        }

        private void VideoPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Console.WriteLine(e.ToString());
        }

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(e.Source.ToString());
        }
    }
}
