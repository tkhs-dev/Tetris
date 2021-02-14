using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using static TetrisPlayerWPF.PlaySettingPage;

namespace TetrisPlayerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += loaded;
            Frame.Navigate(new PlaySettingPage(PlayType.SINGLE));
        }
        private void loaded(object sender, EventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Page page = null;
            switch (button.Tag as string)
            {
                case "SinglePlay":
                    page = new PlaySettingPage(PlayType.SINGLE);
                    break;
                case "AIPlay":
                    page = new PlaySettingPage(PlayType.AI);
                    break;
                case "RePlay":
                    page = new PlaySettingPage(PlayType.REPLAY);
                    break;
                case "Setting":
                    break;
            }
            if (page != null) Frame.Navigate(page);

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Frame.Content is ITabablePage)
            {
                (Frame.Content as ITabablePage).Submit();
            }
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            Frame f = sender as Frame;
            if (f.CanGoBack)
            {
                f.RemoveBackEntry();
            }
            if (f.Content is ITabablePage) SubmitButton.Content = (f.Content as ITabablePage).GetSubmitText();
        }

        private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            object o = (sender as Frame).Content;
            if (o is IDisposable) (o as IDisposable).Dispose();
        }

        private void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left ||
                e.Key == Key.Right || e.Key == Key.Enter || e.Key == Key.Tab)
            {
                e.Handled = true;
            }
        }
    }
}
