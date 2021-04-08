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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MarkersDemonstration
{
    public partial class LandingDemoWindow : Window
    {

        private DispatcherTimer videoTimer;
        private TimeSpan tickRate;
        private bool isPaused; // mediaElement state

        public bool TimelineSlider_ValueChanged { get; set; } = false;

        public LandingDemoWindow()
        {
            InitializeComponent();
            this.PreviewMouseUp += new MouseButtonEventHandler(slider_MouseUp);
            videoTimer = new DispatcherTimer();
            tickRate = new TimeSpan(0, 0, 0, 0, 85);
            videoTimer.Interval = tickRate;
            videoTimer.Tick += new EventHandler(videoTick);

            //Запуск видео
            myMediaElement.Play();
            videoTimer.Start();
            isPaused = false;
        }

        // Stop the media.
        void OnMouseDownStopMedia(object sender, MouseButtonEventArgs args)
        {
            videoTimer.Stop();
            myMediaElement.Stop();
            timelineSlider.Value = 0;
            this.playPause_button.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/play_button.png"));
            isPaused = true;
        }

        // Change the volume of the media.
        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            myMediaElement.Volume = (double)volumeSlider.Value;
        }

        // Change the speed of the media.
        private void ChangeMediaSpeedRatio(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            myMediaElement.SpeedRatio = (double)speedRatioSlider.Value;
        }

        private void Element_MediaOpened(object sender, EventArgs e)
        {
            timelineSlider.Maximum = myMediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
        }

        private void Element_MediaEnded(object sender, EventArgs e)
        {
            myMediaElement.Stop();
        }

        void InitializePropertyValues()
        {
            myMediaElement.Volume = (double)volumeSlider.Value;
            myMediaElement.SpeedRatio = (double)speedRatioSlider.Value;
        }

        private void myMediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("ОШИБКА: по некоторым причинам видео не загрузилось");
        }

        private void timelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimelineSlider_ValueChanged = true;
        }

        void slider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (TimelineSlider_ValueChanged)
            {
                //Обновление видео
                int SliderValue = (int)timelineSlider.Value;
                TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
                myMediaElement.Position = ts;
                TimelineSlider_ValueChanged = false;
            }
        }

        private void videoTick(object sender, EventArgs e)
        {
            // Update slider position
            timelineSlider.Value += tickRate.TotalMilliseconds;
        }

        private void onMousePlayPauseMedia(object sender, RoutedEventArgs e)
        {
            //If video is playing
            if (!isPaused)
            {
                videoTimer.Stop();
                myMediaElement.Pause();
                this.playPause_button.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/play_button.png"));
                isPaused = true;
            } else
            {
                videoTimer.Start();
                myMediaElement.Play();
                InitializePropertyValues();
                this.playPause_button.Source = new BitmapImage(new Uri("pack://application:,,,/Resources/pause_button.png"));
                isPaused = false;
            }
        }

    }
}
