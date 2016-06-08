using MahApps.Metro.Controls;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
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

            media.Source = btn.Tag as Uri;

            media.Play();
        }

        private void downloadSong_Click(object sender, System.Windows.RoutedEventArgs e) {
            //get Button that was clicked
            var btn = sender as System.Windows.Controls.Button;

            //Download file
            using (WebClient web = new WebClient()) {
                web.DownloadFile((btn.Tag as Uri).AbsoluteUri, "1.mp3");
            }
            
        }
    }
}
