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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;

namespace VKMusic {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        VKConnector connector = null;
        public MainWindow() {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));

            username.GotFocus += (object sender, RoutedEventArgs e) => {
                username.Text = "";
                username.Foreground = Brushes.Black;
            };
            username.LostFocus += (object sender, RoutedEventArgs e) => {
                if (string.IsNullOrEmpty(username.Text)) {
                    username.Text = "Username";
                    username.Foreground = Brushes.Gray;
                }
            };

            password.GotFocus += (object sender, RoutedEventArgs e) => {
                password.Password = "";
            };

            username.Foreground = Brushes.Gray;

            //AudioWindow audioWindow = new AudioWindow();

            //audioWindow.Show();
        }

        private void submit_Click(object sender, RoutedEventArgs e) {
            connector = new VKConnector(this, username.Text, password.Password);

            username.Text = password.Password = "";

            if (!connector.OneAuth()) {
                mainAuth.Visibility = Visibility.Hidden;
                secondAuth.Visibility = Visibility.Visible;
            } else {
                AudioWindow audioWindow = new AudioWindow(connector);
                audioWindow.Show();

                Close();
            }
        }

        private void successAuth_Click(object sender, RoutedEventArgs e) {
            connector.KeyValue = keyAuth.Text;

            if (!connector.TwoAuth()) {
                mainAuth.Visibility = Visibility.Visible;
                secondAuth.Visibility = Visibility.Hidden;

                errorMessage.Text = "Incorrect SMS key";
            } else {
                AudioWindow audioWindow = new AudioWindow(connector);
                audioWindow.Show();

                Close();
            }
        }

        private void back_Click(object sender, RoutedEventArgs e) {
            secondAuth.Visibility = Visibility.Hidden;
            mainAuth.Visibility = Visibility.Visible;

            connector = null;
        }
    }
}
