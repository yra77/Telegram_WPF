

using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace Telegram_WPF.Controls.Medias
{
    internal class MediaPlayer : Grid
    {


        private Image? _image;


        public MediaPlayer() : base()
        {
            Width = 700;
            Height = 600;
            Loaded += MediaPlayer_Loaded;
        }


        public Uri? Source { get; set; }


        private void MediaPlayer_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Canvas canvas = new Canvas();
            canvas.Width = Width;
            canvas.Height = Height;
            MediaElement me = new MediaElement();

            _image = new Image()
            {
                Width = 32,
                Height = 32,
                Stretch = System.Windows.Media.Stretch.Fill,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                // images[i].Margin = new Thickness() { Bottom=10 };
                Source = new BitmapImage(new Uri(Path.GetFullPath
                                                 (Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."))
                                                 + "/Images/ic_mute.png"))
            };

            me.LoadedBehavior = MediaState.Manual;
            me.Play();
            me.MediaOpened += MediaPlayer_MediaOpened;
            //mediaElemnts[i].Loaded += (s,e) => { Debug.WriteLine("PPPPPPPPPPPPPPPPpp"); };
            me.MouseEnter += MediaPlayer_MouseEnter;
            me.MouseLeave += MediaPlayer_MouseLeave;
            me.MediaEnded += MediaPlayer_MediaEnded;
            me.Height = Height;
            me.Width = Width;
            me.IsMuted = true;
            me.Stretch = System.Windows.Media.Stretch.Fill;
            me.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            me.MouseDown += MediaPlayer_MouseDown;
            me.Source = Source;

            Canvas.SetRight(_image, Width/14);
            Canvas.SetTop(_image, Height/10*7.6);
            Canvas.SetZIndex(_image, 2);
            
            canvas.Children.Add(_image);
            canvas.Children.Add(me);

            Children.Add(canvas);
        }

        private void MediaPlayer_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is MediaElement me)
                if (me.IsMuted)
                {
                    me.IsMuted = false;
                    _image.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    me.IsMuted = true;
                    _image.Visibility = System.Windows.Visibility.Visible;
                } 
        }

        private void MediaPlayer_MediaEnded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is MediaElement me)
            {
                me.Position = System.TimeSpan.FromSeconds(1);
                me.Play();
            }
        }

        private void MediaPlayer_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is MediaElement me)
            {
                me.Pause();
                me.IsMuted = true;
                _image.Visibility = System.Windows.Visibility.Visible;
            } 
        }

        private void MediaPlayer_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is MediaElement me)
            {
                me.Play(); 
                me.IsMuted = false;
                _image.Visibility = System.Windows.Visibility.Hidden;
            } 
        }

        private void MediaPlayer_MediaOpened(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is MediaElement me)
            {
                me.Position = System.TimeSpan.FromSeconds(1);
                me.Pause();
                _image.Visibility = System.Windows.Visibility.Visible;
            }
        }
    }
}
