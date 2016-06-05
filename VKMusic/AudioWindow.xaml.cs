using MahApps.Metro.Controls;
using System.IO;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
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

            this.connector = connector;

            string gridXaml = XamlWriter.Save(basic);

            StringReader sr = new StringReader(gridXaml);
            XmlReader xr = XmlReader.Create(sr);
            Grid newGrid = (Grid)XamlReader.Load(xr);

            mainPanel.Children.Add(newGrid);

            User user;
            var audios = connector.VK.Audio.Get((long)connector.VK.UserId, out user, null, null, 21, showFrom + 1);

            for (int i = 0; i < 20; i++) {
                //AddChild songs
            }
        }
    }
}
