using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Input;
using System.Xml;
using VkNet.Model;
using VkNet.Model.Attachments;
using System.Windows.Media;

namespace VKMusic {
    /// <summary>
    /// Interaction logic for AudioWindow.xaml
    /// </summary>
    public partial class AudioWindow : MetroWindow {
        VKConnector connector { get; set; } //VKApi connector
        uint? lastLoadedSong = 0;  //current song that showed in window
        uint currentSongNumber; //current song that plays
        Grid copyBasicAudioGrid; //copy of basic audio grid
        MediaElement media; //media element to play music
        DispatcherTimer playTimer;  //timer that handles slider
        bool isSliderUnactive = false;  //true if slider is unactive
        public AudioWindow(VKConnector connector) {
            //init UI elements
            InitializeComponent();

            //set tag of playGlobalSong just to identify
            //image of this button
            //true if plays music
            playGlobalSong.Tag = false;

            //save basic audio grid
            copyBasicAudioGrid = basic;

            //assign VKconnector
            this.connector = connector;

            //load first songs
            loadMore_Click(loadMore, null);

            //remove basic audio grid
            mainPanel.Children.Remove(basic);

            search.GotFocus += (object sender, RoutedEventArgs e) => {
                //Search got focus - change background
                searchBorder.Background = Brushes.White;
            };
            search.LostFocus += (object sender, RoutedEventArgs e) => {
                //Search lost focus - change background
                searchBorder.Background = new SolidColorBrush(Color.FromArgb(191, 255, 255, 255));
            };

            //variable for closing
            bool locker;

            //action mouse down delegate for remote song buttons
            MouseButtonEventHandler actionDown = (object sender, MouseButtonEventArgs e) => {
                //thread locker make true
                locker = true;
                Thread thread = new Thread(() => {
                    int secondsToAdd = 1;
                    //while button down - remote song
                    while (locker) {
                        //check what button pressed
                        if (sender == forwardRemote)
                            //remote forward
                            media.Dispatcher.Invoke(() => {
                                media.Position = media.Position.Add(new TimeSpan(0, 0, secondsToAdd));
                            });
                        else
                            //remote back
                            media.Dispatcher.Invoke(() => {
                                media.Position = media.Position.Subtract(new TimeSpan(0, 0, secondsToAdd));
                            });
                        //stop this thread
                        Thread.Sleep(new TimeSpan(0, 0, 0, 0, 250));
                        //make remote longer
                        secondsToAdd += 1;
                    }
                });
                //start remote thread
                thread.Start();
            };

            //action mouse up delegate for remote song buttons
            MouseButtonEventHandler actionUp = (object sender, MouseButtonEventArgs e) => {
                //unlock thread
                locker = false;
            };

            //add delegates to back remote
            backRemote.PreviewMouseDown += actionDown;
            backRemote.PreviewMouseUp += actionUp;

            //and to forward remote
            forwardRemote.PreviewMouseDown += actionDown;
            forwardRemote.PreviewMouseUp += actionUp;
        }

        private void loadMore_Click(object sender, RoutedEventArgs e) {
            User user;

#pragma warning disable CS0618 // Type or member is obsolete
            var audios = connector.VK.Audio.Get((long)connector.VK.UserId, out user, null, null, 21, lastLoadedSong);
#pragma warning restore CS0618 // Type or member is obsolete

            foreach (var audio in audios) {
                //AddChild songs
                //copy XAML of basic grid
                string gridXaml = XamlWriter.Save(copyBasicAudioGrid);

                //convert it to string, then to XML
                //and create grid
                StringReader sr = new StringReader(gridXaml);
                XmlReader xr = XmlReader.Create(sr);
                Grid newGrid = (Grid)XamlReader.Load(xr);

                //need to get songName field and button
                foreach (var item in newGrid.GetChildObjects()) {
                    if ((item as TextBlock)?.Text == "Song")
                        //songName field
                        (item as TextBlock).Text = audio.Artist + " - "
                            + audio.Title;
                    else if ((item as System.Windows.Controls.Button)?.Name == "playSong" ||
                        (item as System.Windows.Controls.Button)?.Name == "downloadSong") {
                        //play button or download button
                        //get this button
                        var btn = item as System.Windows.Controls.Button;

                        //set another button name
                        btn.Name += lastLoadedSong;

                        //add click event handler and URL or all Audio
                        if (btn.Name.StartsWith("playSong")) {
                            //link event
                            btn.Click += playSong_Click;
                        }
                        else {
                            //link event
                            btn.Click += downloadSong_Click;
                            //link audio with button
                            btn.Tag = audio;
                        }
                    }

                }

                //add new children
                mainPanel.Children.Add(newGrid);
                //increment last loaded song
                ++lastLoadedSong;
            }

            //check for filter
            search_TextChanged(null, null);
        }

        private void playSong_Click(object sender, RoutedEventArgs e) {
            //get Button that was clicked
            var btn = sender as System.Windows.Controls.Button;
            
            if (sender == playGlobalSong) {
                //if clicked playGlobalSong
                if (!(bool)btn.Tag) {
                    //change play button to stop button
                    var imagePlay = playGlobalSong.FindChild<Image>("playGlobalSongImage") as Image;
                    imagePlay.Source = new BitmapImage(new Uri("pack://application:,,,/stop.png"));
                    //set its tag
                    btn.Tag = true;
                    //play first song
                    if (media == null)
                        playSong_Click(this.FindChild<System.Windows.Controls.Button>("playSong0"), null);
                    //continue play
                    else
                        media.Play();
                }
                else if (media != null) {
                    //change play button to play button
                    var imagePlay = playGlobalSong.FindChild<Image>("playGlobalSongImage") as Image;
                    imagePlay.Source = new BitmapImage(new Uri("pack://application:,,,/play.png"));
                    //change tag
                    btn.Tag = false;
                    //pause song
                    media.Pause();
                }
            }
            else {
                //create media
                if (media == null) {
                    //create new media element
                    media = new MediaElement();
                    //set his loaded and unloaded behaviour
                    media.LoadedBehavior = MediaState.Manual;
                    media.UnloadedBehavior = MediaState.Manual;
                    //set it unvisible
                    media.Visibility = Visibility.Hidden;
                }
                //stop previous media
                media.Stop();
                //get num of song
                currentSongNumber = UInt32.Parse(btn.Name.Replace("playSong", string.Empty));
                //get current audio
                var currentAudio = this.FindChild<System.Windows.Controls.Button>("downloadSong" + currentSongNumber).Tag as Audio;
                //get song from URL
                media.Source = currentAudio.Url;
                //play it
                try {
                    //try play current song
                    media.Play();
                } catch (InvalidOperationException) {
                    //if cant - play next
                    playSong_Click(this.FindChild<System.Windows.Controls.Button>("playSong" + ++currentSongNumber), e);
                }

                //action that happends when media stops
                media.MediaEnded += new RoutedEventHandler((object objSender, RoutedEventArgs ev) => {
                    //call next song play
                    playSong_Click(this.FindChild<System.Windows.Controls.Button>("playSong" + ++currentSongNumber), null);
                });
                //create play music timer
                if (playTimer == null) {
                    //create new timer for update slider value while music plays
                    playTimer = new DispatcherTimer();
                    //set timer tick event
                    playTimer.Tick += (object o, EventArgs ev) => {
                        //set maximum slider value equals to duration of audio
                        //set text timer to current position of audio
                        if (media.NaturalDuration.HasTimeSpan) {
                            remoteSongSlider.Maximum = media.NaturalDuration.TimeSpan.TotalSeconds;
                            songDuration.Text = media.Position.ToString(@"mm\:ss") + @"\" +
                                media.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
                        } else
                            songDuration.Text = "00:00";
                        //set its current position
                        if (!isSliderUnactive)
                            remoteSongSlider.Value = media?.Position.TotalSeconds??0.0;
                    };
                    //change interval to 250 miliseconds
                    playTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
                    //start timer
                    playTimer.Start();
                }

                //change play button to stop button
                var imagePlay = playGlobalSong.FindChild<Image>("playGlobalSongImage") as Image;
                imagePlay.Source = new BitmapImage(new Uri("pack://application:,,,/stop.png"));

                //change playGlobalSong tag
                playGlobalSong.Tag = true;

                //change window title to title of music played
                Title = currentAudio.Artist + " - " + currentAudio.Title;
            }
        }

        private void downloadSong_Click(object sender, RoutedEventArgs e) {
            //get Button that was clicked
            var btn = sender as System.Windows.Controls.Button;

            //Download file
            using (WebClient client = new WebClient()) {
                //thread to download file async
                Thread downloadThread = new Thread(() => {
                    //make visible progress bar
                    downloadProgress.Dispatcher.Invoke(() => {
                        downloadProgress.Visibility = Visibility.Visible;
                    });

                    //hide button load more
                    loadMore.Dispatcher.Invoke(() => loadMore.Visibility = Visibility.Hidden);

                    //method to change download progress
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(
                        (object objSender, DownloadProgressChangedEventArgs ev) => {
                                //get downloaded bytes
                                double bytesIn = double.Parse(ev.BytesReceived.ToString());
                                //get total bytes
                                double totalBytes = double.Parse(ev.TotalBytesToReceive.ToString());
                                //get part that was downloaded
                                double percentage = bytesIn / totalBytes * 100;

                                //set this value on progress bar
                                downloadProgress.Dispatcher.Invoke(() => downloadProgress.Value = percentage);
                        });

                    //method would call when file would be downloaded
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(
                        (object objSender, AsyncCompletedEventArgs ev) => {
                                //hide progress bar
                                downloadProgress.Dispatcher.Invoke(() => {
                                downloadProgress.Visibility = Visibility.Hidden;
                                    //reset value
                                    downloadProgress.Value = 0;
                            });
                                //show button load more
                                loadMore.Dispatcher.Invoke(() => loadMore.Visibility = Visibility.Visible);
                        });

                    Audio currentAudio = null;
                    UInt32 num = 0;
                    //get number of song and current audio file
                    btn.Dispatcher.Invoke(() => {
                        currentAudio = btn.Tag as Audio;
                        num = UInt32.Parse(btn.Name.Replace("downloadSong", string.Empty));
                    });
                    //get path to MyMusic folder
                    var path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "\\";
                    //start download async
                    client.DownloadFileAsync(currentAudio.Url,
                        path + (currentAudio.Artist ?? "Artist " + num) + " - " + (currentAudio.Title ?? "Title " + num) + ".mp3");
                });

                //start download
                downloadThread.Start();
            }

        }

        private void remoteSong_PreviewMouseUp(object sender, MouseButtonEventArgs e) {
            //set active slider
            isSliderUnactive = false;
            //rewind or fast forward song
            if (media != null) {
                media.Position = new TimeSpan((int)remoteSongSlider.Value / 3600, (int)remoteSongSlider.Value % 3600 / 60,
                    (int)remoteSongSlider.Value % 3600 % 60);
            }
        }

        private void remoteSongSlider_PreviewMouseMove(object sender, MouseEventArgs e) {
            //set audio timer text
            if (e.LeftButton == MouseButtonState.Pressed && 
                media != null && media.NaturalDuration.HasTimeSpan) {
                //set unactive slider
                isSliderUnactive = true;
                //change timer text
                songDuration.Text = String.Format("{0:00}:{1:00}", (int)remoteSongSlider.Value / 60,
                    (int)remoteSongSlider.Value % 60) + @"\" +
                    media.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            }
        }

        private void Remote_Click(object sender, RoutedEventArgs e) {
            //handle remote song
            if (media != null && media.Position != new TimeSpan()) {
                //if was clicked back remote button
                if (sender == backRemote) {
                    //sub audio position
                    media.Position = media.Position.Subtract(new TimeSpan(0, 0, 1));
                }
                //if was clicked forward remote button
                else if (sender == forwardRemote) {
                    //add audio position
                    media.Position = media.Position.Add(new TimeSpan(0, 0, 1));
                }
            }
        }

        private void search_TextChanged(object sender, TextChangedEventArgs e) {
            //filter results
            //show songs that under filter condition
            mainPanel.FindChildren<Grid>()
                .SelectMany(grid => grid.FindChildren<TextBlock>())
                .Where(textBlock => textBlock.Text.ToLower().Contains(search.Text.ToLower()))
                .ToList()
                .ForEach(textBlock => (textBlock.Parent as Grid).Visibility = Visibility.Visible);
            //hide songs that not under filter condition
            mainPanel.FindChildren<Grid>()
                .SelectMany(grid => grid.FindChildren<TextBlock>())
                .Where(textBlock => !textBlock.Text.ToLower().Contains(search.Text.ToLower()))
                .ToList()
                .ForEach(textBlock => (textBlock.Parent as Grid).Visibility = Visibility.Collapsed);
        }
    }
}
