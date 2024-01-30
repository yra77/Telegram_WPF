

using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;


namespace Telegram_WPF.Controls.Medias
{
    internal class MediaMixList : ListBox
    {

        public MediaMixList():base()
        {
            SelectionMode = SelectionMode.Single;
            Width = 750;
            HorizontalAlignment = HorizontalAlignment.Center; 
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalContentAlignment = HorizontalAlignment.Center;
            ClipToBounds = false;
            Padding = new Thickness(0, 10, 0, 10);
            
            Loaded += MediaMixList_Loaded;

            IsSynchronizedWithCurrentItem = true;
        }


        public static readonly DependencyProperty ImgsProperty =
                               DependencyProperty.RegisterAttached("Imgs",
                               typeof(ObservableCollection<byte[]?>),
                               typeof(MediaMixList));

        public ObservableCollection<byte[]?>? Imgs
        {
            get { return GetValue(ImgsProperty) as ObservableCollection<byte[]?>; }
            set { SetValue(ImgsProperty, value); }
        }


        public static readonly DependencyProperty VideosProperty =
                               DependencyProperty.RegisterAttached("Videos",
                               typeof(ObservableCollection<string?>),
                               typeof(MediaMixList));

        public ObservableCollection<string?>? Videos
        {
            get { return GetValue(VideosProperty) as ObservableCollection<string?>; }
            set { SetValue(VideosProperty, value); }
        }


        private void MediaMixList_Loaded(object sender, RoutedEventArgs e)
        {
            if (Imgs != null && Imgs.Count > 0) CreateImgs();
            if (Videos != null && Videos.Count > 0) CreateVideos();
        }

        private void CreateImgs()
        {

            ObservableCollection<Image> images = new ObservableCollection<Image>();
            ObservableCollection<Border> border = new ObservableCollection<Border>();

            for (int i = 0; i < Imgs.Count; i++)
            {
                images.Add(new Image());
                border.Add(new Border()
                {
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1, 1, 1, 1),
                    Width = 700,
                    Margin = new Thickness(0, 0, 0, 15),
                    Padding = new Thickness(0, 0, 0, 10)
                });
            }


            for (int i = 0; i < Imgs.Count; i++)
            {
                images[i].Stretch = System.Windows.Media.Stretch.Fill;
                images[i].HorizontalAlignment = HorizontalAlignment.Center;
                // images[i].Margin = new Thickness() { Bottom=10 };
                BitmapImage myBitmapImage = new BitmapImage();

                myBitmapImage.BeginInit();

                myBitmapImage.StreamSource = new MemoryStream(Imgs[i]);
                myBitmapImage.DecodePixelWidth = 450;
                myBitmapImage.EndInit();
                images[i].Source = myBitmapImage;

                border[i].Child = images[i];

                AddChild(border[i]);
            }
        }

        private void CreateVideos()
        {
            ObservableCollection<MediaPlayer> mediaElemnts = new ObservableCollection<MediaPlayer>();
            ObservableCollection<Border> border = new ObservableCollection<Border>();

            for (int i = 0; i < Videos.Count; i++)
            {
                mediaElemnts.Add(new MediaPlayer());
                border.Add(new Border()
                {
                    BorderBrush = Brushes.LightGray,
                    BorderThickness = new Thickness(1, 1, 1, 1),
                    Width = 700,
                    Margin = new Thickness(0, 0, 0, 15),
                    Padding = new Thickness(0, 0, 0, 10)
                });
            }

            for (int i = 0; i < Videos.Count; i++)
            {
                mediaElemnts[i].Source = new System.Uri(Videos[i]);
                mediaElemnts[i].Width = 700;
                mediaElemnts[i].Height = 600;
                border[i].Child = mediaElemnts[i];
                AddChild(border[i]);
            }
        }

//        Decorator bbborder = VisualTreeHelper.GetChild(this, 0) as Decorator;
//            if (bbborder != null)
//            {
//                // Get scrollviewer
//                ScrollViewer scrollViewer = bbborder.Child as ScrollViewer;
//                if (scrollViewer != null)
//                {
//                    // center the Scroll Viewer...
//                    // double center = scrollViewer.ScrollableHeight / 2.0;
//                    //scrollViewer.ScrollToVerticalOffset(center);
                 
//                    scrollViewer.ScrollChanged += (s, e) => { Debug.WriteLine("PPPPPPPPp " + e.OriginalSource); };
//}
//            }


//         this.SelectionChanged += (sender, e) =>
//            {
//                Selector selector = sender as Selector;
//                if (selector is ListBox)
//                {
//                    (selector as ListBox).ScrollIntoView(selector.SelectedItem);

//    };
//};

    }
}
