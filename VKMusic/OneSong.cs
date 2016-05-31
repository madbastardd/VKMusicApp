using System.Windows.Controls;
using System.Windows;

namespace VKMusic {
    class OneSong : Grid {
        public OneSong(string name = "") {
            Height = 30;

            ColumnDefinition column1 = new ColumnDefinition();
            column1.Width = new GridLength(1, GridUnitType.Star);

            ColumnDefinition column2 = new ColumnDefinition();
            column2.Width = new GridLength(1, GridUnitType.Star);

            ColumnDefinition column3 = new ColumnDefinition();
            column3.Width = new GridLength(5, GridUnitType.Star);

            ColumnDefinitions.Add(column1);
            ColumnDefinitions.Add(column2);
            ColumnDefinitions.Add(column3);

            Button buttonPlay = new Button();
            buttonPlay.SetValue(ColumnProperty, 0);
            buttonPlay.Width = Height;
            buttonPlay.HorizontalAlignment = HorizontalAlignment.Right;
            buttonPlay.Margin = new Thickness(0, 0, 5, 0);

            Button buttonStop = new Button();
            buttonStop.SetValue(ColumnProperty, 1);
            buttonStop.Width = Height;
            buttonStop.HorizontalAlignment = HorizontalAlignment.Left;
            buttonStop.Margin = new Thickness(5, 0, 0, 0);

            TextBlock songName = new TextBlock();
            songName.Text = name;
            songName.SetValue(ColumnProperty, 2);

            Children.Add(buttonPlay);
            Children.Add(buttonStop);
            Children.Add(songName);
        }
    }
}
