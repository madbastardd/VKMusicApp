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
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VKMusic {
    /// <summary>
    /// Interaction logic for AudioWindow.xaml
    /// </summary>
    public partial class AudioWindow : Window {
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

                mainPanel.Children.Add(oneSong);
            }
        }
    }
}
