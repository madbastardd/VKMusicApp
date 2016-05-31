using MahApps.Metro.Controls;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

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
                    username.Foreground = new SolidColorBrush(Color.FromRgb(55, 55, 55));
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

            username.Foreground = new SolidColorBrush(Color.FromRgb(55, 55, 55));
        }

        private void submit_Click(object sender, RoutedEventArgs e) {
            connector = new VKConnector(this, username.Text, password.Password);
            username.Text = password.Password = "";

            username.Foreground = new SolidColorBrush(Color.FromRgb(55, 55, 55));
            username.Text = "Username";

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
                load.RenderTransform = new RotateTransform(angle -= 10);
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

                    errorMessage.Dispatcher.Invoke(() => errorMessage.Text = "Incorrect SMS key");
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

        private void username_textChanged(object sender, TextChangedEventArgs e) {
            if (logInBtn != null)
                logInBtn.Visibility = (!string.IsNullOrEmpty(username.Text) 
                    && !string.IsNullOrEmpty(password.Password)
                    && username.Text != "Username")
                    ? Visibility.Visible
                    : Visibility.Hidden;
        }

        private void password_PasswordChanged(object sender, RoutedEventArgs e) {
            if (logInBtn != null)
                logInBtn.Visibility = (!string.IsNullOrEmpty(username.Text) 
                    && !string.IsNullOrEmpty(password.Password)
                    && username.Text != "Username")
                    ? Visibility.Visible
                    : Visibility.Hidden;
        }
    }
}
