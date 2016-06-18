using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.IO;
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
        uint? currentLoadedSong = 0;  //current song that showed in window
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
        }

        private void loadMore_Click(object sender, RoutedEventArgs e) {
            User user;

#pragma warning disable CS0618 // Type or member is obsolete
            var audios = connector.VK.Audio.Get((long)connector.VK.UserId, out user, null, null, 21, currentLoadedSong);
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
                        btn.Name += currentLoadedSong;

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
                //increment currentSong
                ++currentLoadedSong;
            }
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
                        playSong_Click(this.FindChild<System.Windows.Controls.Button>("playSong1"), null);
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
                //stop previouse media
                media.Stop();
                //get num of song
                var num = UInt32.Parse(btn.Name.Replace("playSong", string.Empty));
                //get current audio
                var playedSong = this.FindChild<System.Windows.Controls.Button>("downloadSong" + num).Tag as Audio;
                //get song from URL
                media.Source = playedSong.Url;
                //play it
                media.Play();
                //set media tag - current number of song played
                media.Tag = num;

                //action that happends when media stops
                media.MediaEnded += new RoutedEventHandler((object objSender, RoutedEventArgs ev) => {
                    uint currentSongPlayed = num + 1;   //get number of next song to play
                    //call next song play
                    playSong_Click(this.FindChild<System.Windows.Controls.Button>("playSong" + currentSongPlayed), null);
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
                            remoteSong.Maximum = media.NaturalDuration.TimeSpan.TotalSeconds;
                            songDuration.Text = media.Position.ToString(@"mm\:ss") + @"\" +
                                media.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
                        } else
                            songDuration.Text = "00:00";
                        //set its current position
                        if (!isSliderUnactive)
                            remoteSong.Value = media?.Position.TotalSeconds??0.0;
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
                Title = playedSong.Artist + " - " + playedSong.Title;
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
                media.Position = new TimeSpan((int)remoteSong.Value / 3600, (int)remoteSong.Value % 3600 / 60,
                    (int)remoteSong.Value % 3600 % 60);
            }
        }

        private void remoteSong_PreviewMouseMove(object sender, MouseEventArgs e) {
            //set audio timer text
            if (e.LeftButton == MouseButtonState.Pressed && 
                media != null && media.NaturalDuration.HasTimeSpan) {
                //set unactive slider
                isSliderUnactive = true;
                //change timer text
                songDuration.Text = String.Format("{0:00}:{1:00}", (int)remoteSong.Value / 60,
                    (int)remoteSong.Value % 60) + @"\" +
                    media.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            }
        }

        private void Remote_Click(object sender, RoutedEventArgs e) {
            //handle remote song
            if (media != null && media.Position != new TimeSpan()) {
                //if was clicked previous remote button
                if (sender == previousRemote) {
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
            //foreach for all grids in stackPanel
            foreach (var grid in mainPanel.GetChildObjects()) {
                //foreach for all elements in music grid
                foreach (var gridElement in grid.GetChildObjects()) {
                    //if song name contains word in search
                    if ((gridElement as TextBlock)?.Text.ToLower().Contains(search.Text.ToLower()) == true) {
                        //show it
                        (grid as Grid).Visibility = Visibility.Visible;
                    }
                    else if ((gridElement as TextBlock)?.Text.ToLower().Contains(search.Text.ToLower()) == false) {
                        //or hide it
                        (grid as Grid).Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
    }
}
