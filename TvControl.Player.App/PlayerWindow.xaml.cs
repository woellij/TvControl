using System.Windows;

namespace TvControl.Player.App
{
    /// <summary>
    ///     Interaction logic for PlayerWindow.xaml
    /// </summary>
    public partial class PlayerWindow : Window
    {

        private bool fullscreen;

        public PlayerWindow()
        {
            this.InitializeComponent();
            this.fullscreen = false;
        }

        private void ToggleFullscreenOnClick(object sender, RoutedEventArgs e)
        {
            this.WindowStyle = this.fullscreen ? WindowStyle.SingleBorderWindow : WindowStyle.None;
            this.WindowState = this.fullscreen ? WindowState.Normal : WindowState.Maximized;
            this.fullscreen = !this.fullscreen;
        }

    }
}