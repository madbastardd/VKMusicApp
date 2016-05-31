using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Exception;

namespace VKMusic {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow {
        VKConnector connector = null;
        DispatcherTimer loadTimer;
        public MainWindow() {
            InitializeComponent();

            username.GotFocus += (object sender, RoutedEventArgs e) => {
                username.Text = "";
                username.Foreground = Brushes.Black;
                usernameBorder.Background = Brushes.White;
            };
            username.LostFocus += (object sender, RoutedEventArgs e) => {
                if (string.IsNullOrEmpty(username.Text)) {
                    username.Text = "Username";
                    username.Foreground = Brushes.Gray;
                }
                usernameBorder.Background = new SolidColorBrush(Color.FromArgb(75, 255, 255, 255));
            };

            password.GotFocus += (object sender, RoutedEventArgs e) => {
                password.Password = "";
                passwordBorder.Background = Brushes.White;
            };
            password.LostFocus += (object sender, RoutedEventArgs e) => {
                passwordBorder.Background = new SolidColorBrush(Color.FromArgb(75, 255, 255, 255));
            };

            username.Foreground = Brushes.Gray;
        }

        private void submit_Click(object sender, RoutedEventArgs e) {
            connector = new VKConnector(this, username.Text, password.Password);
            username.Text = password.Password = "";

            wallIMG.Visibility = Visibility.Hidden;
            loadAnimation();

            Thread tempThread = new Thread(() => {
                if (!connector.OneAuth()) {
                    loadTimer?.Stop();

                    wallIMG.Dispatcher.Invoke(() => wallIMG.Visibility = Visibility.Visible);
                    loadIMG.Dispatcher.Invoke(() => loadIMG.Visibility = Visibility.Hidden);

                    mainAuth.Dispatcher.Invoke(() => mainAuth.Visibility = Visibility.Hidden);
                    secondAuth.Dispatcher.Invoke(() => secondAuth.Visibility = Visibility.Visible);
                }
                else {
                    loadTimer?.Stop();

                    Dispatcher.Invoke(() => {
                        AudioWindow audioWindow = new AudioWindow(connector);
                        audioWindow.Show();

                        Close();
                    });
                }
            });

            tempThread.SetApartmentState(ApartmentState.STA);
            tempThread.Start();
        }

        private void loadAnimation() {
            loadIMG.Visibility = Visibility.Visible;

            int angle = 360;
            loadTimer = new DispatcherTimer();
            loadTimer.Tick += (object o, EventArgs ev) => {
                load.RenderTransform = new RotateTransform(angle -= 5);
                if (angle < 0)
                    angle += 360;
            };
            loadTimer.Interval = new TimeSpan(0, 0, 0, 0, 35);
            loadTimer.Start();
        }

        private void successAuth_Click(object sender, RoutedEventArgs e) {
            connector.KeyValue = keyAuth.Text;

            wallIMG.Visibility = Visibility.Hidden;
            loadAnimation();

            Thread tempThread = new Thread(() => {
                if (!connector.TwoAuth()) {
                    loadTimer?.Stop();

                    wallIMG.Dispatcher.Invoke(() => wallIMG.Visibility = Visibility.Visible);
                    loadIMG.Dispatcher.Invoke(() => loadIMG.Visibility = Visibility.Hidden);

                    mainAuth.Dispatcher.Invoke(() => mainAuth.Visibility = Visibility.Visible);
                    secondAuth.Dispatcher.Invoke(() => secondAuth.Visibility = Visibility.Hidden);

                    errorMessage.Text = "Incorrect SMS key";
                }
                else {
                    loadTimer?.Stop();

                    Dispatcher.Invoke(() => {
                        AudioWindow audioWindow = new AudioWindow(connector);
                        audioWindow.Show();

                        Close();
                    });
                }
            });

            tempThread.SetApartmentState(ApartmentState.STA);
            tempThread.Start();
        }

        private void back_Click(object sender, RoutedEventArgs e) {
            secondAuth.Visibility = Visibility.Hidden;
            mainAuth.Visibility = Visibility.Visible;

            connector = null;
        }
    }
}
