using MahApps.Metro.Controls;
using System.Windows.Controls;
using VkNet.Model;

namespace VKMusic {
    /// <summary>
    /// Interaction logic for AudioWindow.xaml
    /// </summary>
    public partial class AudioWindow : MetroWindow {
        private VKConnector connector  { get; set; }
        private uint? showFrom = 0;
        public AudioWindow(VKConnector connector) {
            InitializeComponent();
            //Style = (Style)FindResource(typeof(Window));

            this.connector = connector;

            User user;
            var audios = connector.VK.Audio.Get((long)connector.VK.UserId, out user, null, null, 21, showFrom + 1);

            for (int i = 0; i < 20; i++) {
                OneSong oneSong = new OneSong(audios[i].Artist + " - " + audios[i].Title);
                Grid grid = new Grid();

                mainPanel.Children.Add(oneSong);
            }
        }
    }
}
