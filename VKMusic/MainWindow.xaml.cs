using MahApps.Metro.Controls;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Input;

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
                //Username got focus - delete all text 
                //and change foreground color
                //and change background
                username.Text = "";
                username.Foreground = Brushes.Black;
                usernameBorder.Background = Brushes.White;
            };
            username.LostFocus += (object sender, RoutedEventArgs e) => {
                //Username lost focus - change foreground color 
                //if it's empty and change background
                if (string.IsNullOrEmpty(username.Text)) {
                    username.Text = "Username";
                    username.Foreground = new SolidColorBrush(Color.FromRgb(55, 55, 55));
                }
                usernameBorder.Background = new SolidColorBrush(Color.FromArgb(191, 255, 255, 255));
            };

            password.GotFocus += (object sender, RoutedEventArgs e) => {
                //Password field got focus - change
                //background color and clear it
                password.Password = "";
                passwordBorder.Background = Brushes.White;
            };
            password.LostFocus += (object sender, RoutedEventArgs e) => {
                //Password lost focus - change background only
                passwordBorder.Background = new SolidColorBrush(Color.FromArgb(191, 255, 255, 255));
            };

            keyAuth.GotFocus += (object sender, RoutedEventArgs e) => {
                //key auth field got focus - delete all text
                //and change background
                keyAuth.Text = "";
                keyAuthBorder.Background = Brushes.White;
            };
            keyAuth.LostFocus += (object sender, RoutedEventArgs e) => {
                //key auth lost focus - change background color 
                keyAuthBorder.Background = new SolidColorBrush(Color.FromArgb(191, 255, 255, 255));
            };

            username.Foreground = new SolidColorBrush(Color.FromRgb(55, 55, 55));
        }

        private void submit_Click(object sender, RoutedEventArgs e) {
            //first auth submit button click
            //create new connector
            connector = new VKConnector(username.Text, password.Password);
            //delete password and username for security
            username.Text = password.Password = "";

            //return start text and foreground
            username.Foreground = new SolidColorBrush(Color.FromRgb(55, 55, 55));
            username.Text = "Username";

            //hide wall image
            wallIMG.Visibility = Visibility.Hidden;
            //and load animation
            loadAnimation();

            //VK connect thread
            Thread tempThread = new Thread(() => {
                if (!connector.OneAuth()) {
                    //if cannot connect, then stop animation
                    loadTimer?.Stop();

                    //change background
                    wallIMG.Dispatcher.Invoke(() => wallIMG.Visibility = Visibility.Visible);
                    loadIMG.Dispatcher.Invoke(() => loadIMG.Visibility = Visibility.Hidden);

                    //view another auth window
                    mainAuth.Dispatcher.Invoke(() => mainAuth.Visibility = Visibility.Hidden);
                    secondAuth.Dispatcher.Invoke(() => secondAuth.Visibility = Visibility.Visible);
                }
                else {
                    //if all connected, then stop animation
                    loadTimer?.Stop();
                    
                    //show new window (audio window)
                    Dispatcher.Invoke(() => {
                        AudioWindow audioWindow = new AudioWindow(connector);
                        audioWindow.Show();

                        //close current window
                        Close();
                    });
                }
            });

            //just to show new window
            tempThread.SetApartmentState(ApartmentState.STA);
            //start thread
            tempThread.Start();
        }

        private void loadAnimation() {
            ///show animation, start new timer
            loadIMG.Visibility = Visibility.Visible;

            //start angle
            int angle = 360;
            loadTimer = new DispatcherTimer();
            loadTimer.Tick += (object o, EventArgs ev) => {
                //every tick change angle
                load.RenderTransform = new RotateTransform(angle -= 10);
                if (angle < 0)
                    angle += 360;
            };
            //set timer interval
            loadTimer.Interval = new TimeSpan(0, 0, 0, 0, 35);
            //start
            loadTimer.Start();
        }

        private void successAuth_Click(object sender, RoutedEventArgs e) {
            //second auth button submit click
            connector.keyValue = keyAuth.Text;

            //hide wall image
            wallIMG.Visibility = Visibility.Hidden;
            //and show animation
            loadAnimation();

            //connection thread
            Thread tempThread = new Thread(() => {
                if (!connector.TwoAuth()) {
                    //if cannot connect, then stop animation
                    loadTimer?.Stop();

                    //change background
                    wallIMG.Dispatcher.Invoke(() => wallIMG.Visibility = Visibility.Visible);
                    loadIMG.Dispatcher.Invoke(() => loadIMG.Visibility = Visibility.Hidden);

                    //view another auth window
                    mainAuth.Dispatcher.Invoke(() => mainAuth.Visibility = Visibility.Visible);
                    secondAuth.Dispatcher.Invoke(() => secondAuth.Visibility = Visibility.Hidden);

                    //show error message
                    errorMessage.Dispatcher.Invoke(() => errorMessage.Text = "Incorrect SMS key");
                }
                else {
                    //if all connected, then stop animation
                    loadTimer?.Stop();

                    //show new window (audio window)
                    Dispatcher.Invoke(() => {
                        AudioWindow audioWindow = new AudioWindow(connector);
                        audioWindow.Show();

                        //close current window
                        Close();
                    });
                }
            });

            //just to show new window
            tempThread.SetApartmentState(ApartmentState.STA);
            //start new thread
            tempThread.Start();
        }

        private void back_Click(object sender, RoutedEventArgs e) {
            //just back from second auth window
            //to first auth window
            secondAuth.Visibility = Visibility.Hidden;
            mainAuth.Visibility = Visibility.Visible;

            connector = null;
        }

        private void username_textChanged(object sender, TextChangedEventArgs e) {
            //shows button LogIn when
            //password has pass and username has user
            password_PasswordChanged(sender, null);
        }

        private void password_PasswordChanged(object sender, RoutedEventArgs e) {
            //shows button LogIn when
            //password has pass and username has user
            if (logInBtn != null)
                logInBtn.Visibility = (!string.IsNullOrEmpty(username.Text) 
                    && !string.IsNullOrEmpty(password.Password)
                    && username.Text != "Username")
                    ? Visibility.Visible
                    : Visibility.Hidden;
        }

        private void username_KeyDown(object sender, KeyEventArgs e) {
            //if key is enter - call button click
            if (e.Key == Key.Enter && logInBtn.Visibility == Visibility.Visible)
                submit_Click(logInBtn, null);
        }

        private void keyAuth_KeyDown(object sender, KeyEventArgs e) {
            //if key is enter - call button success click
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(keyAuth.Text))
                successAuth_Click(submit, null);
            //if key is not num - clear that key
            if (!(e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                e.Handled = true;
        }
    }
}
