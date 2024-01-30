

using Telegram_WPF.Models;
using Telegram_WPF.Controls.Medias;

using TL;

using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Interop;
using System.Windows.Documents;
using System;
using System.Text;
using System.Diagnostics;


namespace Telegram_WPF.Controls.Cards
{
    internal class CardPost : StackPanel
    {


        private WrapPanel _wrapPanel;
        private int _width = 0;
        private int _height = 0;


        public CardPost() : base()
        {
            this.Orientation = Orientation.Vertical;
            this.Width = 440;
            this.MaxHeight = 530;
            this.HorizontalAlignment = HorizontalAlignment.Center;
            this.Visibility = Visibility.Visible;
            this.IsEnabled = true;

            _wrapPanel = new WrapPanel()
            {
                Width = 440,
                MaxHeight = 350,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            this.Children.Add(_wrapPanel);

            this.Loaded += CardPost_Loaded;
        }


        #region property

        public static readonly DependencyProperty PostProperty =
                               DependencyProperty.RegisterAttached("Post",
                               typeof(MessageModel),
                               typeof(CardPost));

        public MessageModel? Post
        {
            get { return GetValue(PostProperty) as MessageModel; }
            set { SetValue(PostProperty, value); }
        }


        public static readonly DependencyProperty ForwardImgProperty =
                              DependencyProperty.RegisterAttached("ForwardImg",
                              typeof(string),
                              typeof(CardPost));

        public string? ForwardImg
        {
            get { return GetValue(ForwardImgProperty) as string; }
            set { SetValue(ForwardImgProperty, value); }
        }

        #endregion


        private void CardPost_Loaded(object sender, RoutedEventArgs e)
        {
            WidthHeight();
            if (Post.Imgs != null && Post.Imgs.Count > 0) CreateImgs();
            if (Post.Videos != null && Post.Videos.Count > 0) CreateVideos();
            if (Post.Poll != null && Post.Poll.Id != null) CreatePoll();
            if (!string.IsNullOrEmpty(Post.Message)) CreateText();
            CreateArrowForward();
        }

        private void CreatePoll()
        {

            List<ProgressBar> progressBars = new List<ProgressBar>();
            List<TextBlock> textBlocks = new List<TextBlock>();

            for (int i = 0; i < Post.Poll.PollAnswers.Count; i++)
            {
                progressBars.Add(new ProgressBar());
                textBlocks.Add(new TextBlock());
            }

            TextBlock question = new TextBlock()
            {
                Text = Post.Poll.PollQuestion,
                FontSize = 12,
                MaxWidth = 430,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                TextWrapping = TextWrapping.WrapWithOverflow
            };

            Children.Add(question);

            for (int i = 0; i < Post.Poll.PollAnswers.Count; i++)
            {

                double percents = 0.0;
                string str = "";
                double total = 0.0;
                double resultvoters = 0.0;

                if (Post.Poll.PollResults != null && Post.Poll.PollResults.Count > 0
                    && Post.Poll.PollTotalVoters != null && Post.Poll.PollTotalVoters > 0)
                {
                    percents = ((double)Post.Poll.PollResults[i] / (int)Post.Poll.PollTotalVoters) * 100;
                    str = Post.Poll.PollResults[i].ToString() + " - " + percents.ToString(".00") + "%";
                    total = (double)Post.Poll.PollTotalVoters;
                    resultvoters = (double)Post.Poll.PollResults[i];
                }

                textBlocks[i].Text = Post.Poll.PollAnswers[i] + "   " + str;
                textBlocks[i].FontSize = 10;
                textBlocks[i].MaxWidth = 430;
                textBlocks[i].TextAlignment = TextAlignment.Left;
                textBlocks[i].HorizontalAlignment = HorizontalAlignment.Left;
                textBlocks[i].VerticalAlignment = VerticalAlignment.Bottom;
                textBlocks[i].Margin = new Thickness() { Left = 20, Top = 10 };
                textBlocks[i].TextWrapping = TextWrapping.WrapWithOverflow;

                progressBars[i].HorizontalAlignment = HorizontalAlignment.Left;
                progressBars[i].IsIndeterminate = false;
                progressBars[i].Orientation = Orientation.Horizontal;
                progressBars[i].Margin = new Thickness() { Left = 20, Top = 2 };
                progressBars[i].Height = 15;
                progressBars[i].Width = 300;
                progressBars[i].Minimum = 0.0;
                progressBars[i].Maximum = total;
                progressBars[i].Value = resultvoters;

                Children.Add(textBlocks[i]);
                Children.Add(progressBars[i]);
            }

            TextBlock totalVoters = new TextBlock()
            {
                Text = "Total - " + Post.Poll.PollTotalVoters.ToString(),
                FontSize = 10,
                MaxWidth = 430,
                TextAlignment = TextAlignment.Left,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness() { Left = 20, Top = 20, Bottom = 10 },
                TextWrapping = TextWrapping.WrapWithOverflow
            };

            Children.Add(totalVoters);
        }

        private void CreateArrowForward()
        {
            if (ForwardImg != null)
            {
                Image arrow = new Image();

                arrow.Width = 24;
                arrow.Height = 24;
                arrow.Stretch = System.Windows.Media.Stretch.Fill;
                arrow.HorizontalAlignment = HorizontalAlignment.Right;
                arrow.VerticalAlignment = VerticalAlignment.Bottom;
                // images[i].Margin = new Thickness() { Bottom=10 };
                BitmapImage myBitmapImage = new BitmapImage();

                myBitmapImage.BeginInit();

                Uri resourceUri = new Uri(ForwardImg, UriKind.RelativeOrAbsolute);
                arrow.Source = new BitmapImage(resourceUri);

                Children.Add(arrow);
            }
        }

        private void CreateImgs()
        {

            ObservableCollection<Image> images = new ObservableCollection<Image>();

            for (int i = 0; i < Post.Imgs.Count; i++)
            {
                images.Add(new Image());
            }

            for (int i = 0; i < Post.Imgs.Count; i++)
            {
                images[i].Width = _width;
                images[i].Height = _height;
                images[i].Stretch = System.Windows.Media.Stretch.Fill;
                images[i].HorizontalAlignment = HorizontalAlignment.Center;
                // images[i].Margin = new Thickness() { Bottom=10 };
                BitmapImage myBitmapImage = new BitmapImage();

                myBitmapImage.BeginInit();

                myBitmapImage.StreamSource = new MemoryStream(Post.Imgs[i]);
                myBitmapImage.DecodePixelWidth = 450;
                myBitmapImage.EndInit();
                images[i].Source = myBitmapImage;

                _wrapPanel.Children.Add(images[i]);
            }
        }

        private void CreateVideos()
        {
            ObservableCollection<MediaPlayer> mediaElemnts = new ObservableCollection<MediaPlayer>();

            for (int i = 0; i < Post.Videos.Count; i++)
            {
                mediaElemnts.Add(new MediaPlayer());
            }

            for (int i = 0; i < Post.Videos.Count; i++)
            {
                mediaElemnts[i].Width = _width;
                mediaElemnts[i].Height = _height;
                mediaElemnts[i].Source = new System.Uri(Post.Videos[i]);
                _wrapPanel.Children.Add(mediaElemnts[i]);
            }
        }

        private void CreateText()
        {
            StackPanel stackPanel = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(0, 15, 0, 15)
            };

            ScrollViewer scrollViewer = new ScrollViewer() 
            {
              VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            TextBlock msg = new TextBlock()
            {
                MaxHeight = 200,
                FontSize = 10,
                MaxWidth = 430,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.WrapWithOverflow
            };

            TextFormate(ref msg);
            scrollViewer.Content = msg;

            TextBlock date = new TextBlock()
            {
                Text = Post.Date.ToString(),
                FontSize = 8,
                TextAlignment = TextAlignment.Center,
                HorizontalAlignment= HorizontalAlignment.Right,
                Margin = new Thickness(0,0,10,0)
            };

            stackPanel.Children.Add(scrollViewer);
            stackPanel.Children.Add(date);

            this.Children.Add(stackPanel);
        }

        private void TextFormate(ref TextBlock textBlock)
        {
            if (Post.TextFormat != null)
            {
                int offsetPrev = 0;
                for (int j = 0; j < Post.TextFormat.Count; j++)
                {
                    var taskFormat = Post.TextFormat[j].Split(" ");
                    int offset = int.Parse(taskFormat[1]);
                    int lettersCount = int.Parse(taskFormat[2]);
                    int count = offset + lettersCount;

                    if (count != offsetPrev)
                    {
                        Run run = new Run();
                        for (int i = offsetPrev; i < count; i++)
                        {
                            if (taskFormat[0] == "Url")
                            {
                                run.Text += Post.Message[i];
                                Italic italic = new Italic();
                                italic.Foreground = System.Windows.Media.Brushes.Blue;
                                italic.FontWeight = FontWeight.FromOpenTypeWeight(600);
                                italic.Inlines.Add(run);
                                textBlock.Inlines.Add(italic);
                            }
                            else if (taskFormat[0] == "Italic")
                            {
                                run.Text += Post.Message[i];
                                Italic italic = new Italic();
                                italic.Inlines.Add(run);
                                textBlock.Inlines.Add(italic);
                            }
                            else if (taskFormat[0] == "TextUrl")
                            {
                                run.Text += Post.Message[i];
                                textBlock.Inlines.Add(run);
                            }
                            else if (taskFormat[0] == "Bold")
                            {
                                run.Text += Post.Message[i];
                                run.Foreground = System.Windows.Media.Brushes.Black;
                                run.FontWeight = FontWeight.FromOpenTypeWeight(800);

                                textBlock.Inlines.Add(run);
                            }
                            else if (taskFormat[0] == "CustomEmoji")
                            {
                                break;
                            }
                            else
                            {
                                run.Text += Post.Message[i];
                                textBlock.Inlines.Add(run);
                            }
                        }
                    }
                    offsetPrev = count;
                }
            }
            else
                textBlock.Text = Post.Message;
        }

        private void WidthHeight()
        {
            int count = 0;
            if (Post.Imgs != null && Post.Imgs.Count > 0)
            {
                count += Post.Imgs.Count;
            }
            if (Post.Videos != null && Post.Videos.Count > 0)
            {
                count += Post.Videos.Count;
            }

            switch (count)
            {
                case 1:
                    _width = 440;
                    _height = 265;
                    break;

                case 2:
                    _width = 220;
                    _height = 265;
                    break;

                case 3:
                    _width = 220;
                    _height = 85;
                    break;

                case 4:
                    _width = 220;
                    _height = 85;
                    break;

                default:
                    _width = 140;
                    _height = 60;
                    break;
            }
        }

    }
}
