using MahApps.Metro.Controls;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using VkNet.Model;

namespace VKMusic {
    /// <summary>
    /// Interaction logic for AudioWindow.xaml
    /// </summary>
    public partial class AudioWindow : MetroWindow {
        VKConnector                 connector  { get; set; }
        uint?                       currentSong = 1;
        Grid                        copyBasic;
        public AudioWindow(VKConnector connector) {
            //init UI elements
            InitializeComponent();

            //save basic audio grid
            copyBasic = basic;

            //assign VKconnector
            this.connector = connector;

            //load first songs
            loadMore_Click(loadMore, null);

            //remove basic audio grid
            mainPanel.Children.Remove(basic);
        }

        private void loadMore_Click(object sender, System.Windows.RoutedEventArgs e) {
            User user;

#pragma warning disable CS0618 // Type or member is obsolete
            var audios = connector.VK.Audio.Get((long)connector.VK.UserId, out user, null, null, 21, currentSong);
#pragma warning restore CS0618 // Type or member is obsolete

            foreach (var audio in audios) {
                //AddChild songs
                //copy XAML of basic grid
                string gridXaml = XamlWriter.Save(copyBasic);

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
                        btn.Name += currentSong;

                        //link download URL with button
                        btn.Tag = audio.Url;

                        //add click event handler
                        if ((item as System.Windows.Controls.Button)?.Name == "playSong")
                            btn.Click += playSong_Click;
                        else
                            btn.Click += downloadSong_Click;
                    }
                    
                }

                //add new children
                mainPanel.Children.Add(newGrid);
                //increment currentSong
                ++currentSong;
            }
        }

        private void playSong_Click(object sender, System.Windows.RoutedEventArgs e) {
            //get Button that was clicked
            var btn = sender as System.Windows.Controls.Button;

            if (sender == playGlobalSong) {
                //if clicked playGlobalSong
            }
            else {
                //get song from tag (URL)
                media.Source = btn.Tag as Uri;

                //play it
                media.Play();
            }
        }

        private void downloadSong_Click(object sender, System.Windows.RoutedEventArgs e) {
            //get Button that was clicked
            var btn = sender as System.Windows.Controls.Button;

            //Download file
            using (WebClient client = new WebClient()) {
                //thread to download file async
                Thread thread = new Thread(() => {
                    //make visible progress bar
                    downloadProgressGrid.Dispatcher.Invoke(() => {
                        downloadProgressGrid.Visibility = System.Windows.Visibility.Visible;
                    });

                    //method to change download progress
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(
                        (object objSender, DownloadProgressChangedEventArgs ev) => {
                            //get downloaded bytes
                            double bytesIn = double.Parse(ev.BytesReceived.ToString());
                            //get total bytes
                            double totalBytes = double.Parse(ev.TotalBytesToReceive.ToString());
                            //get part that was downloaded
                            double percentage = bytesIn / totalBytes * 100;

                            downloadProgress.Dispatcher.Invoke(() => downloadProgress.Value = percentage);
                        });

                    //method would call when file would be downloaded
                    client.DownloadFileCompleted += new AsyncCompletedEventHandler(
                        (object objSender, AsyncCompletedEventArgs ev) => {
                            //hide progress bar
                            downloadProgressGrid.Dispatcher.Invoke(() => {
                                downloadProgressGrid.Visibility = System.Windows.Visibility.Hidden;
                            });
                    });

                    //start download async
                    btn.Dispatcher.Invoke(() => client.DownloadFileAsync(btn.Tag as Uri, "1.mp3"));
                });

                //start download
                thread.Start();
            }
            
        }
    }
}
