using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Lumina.Views
{
    public partial class Peliculas : Page
    {
        public Peliculas()
        {
            InitializeComponent();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var win = Window.GetWindow(this);
                if (win != null) { try { win.DragMove(); } catch { } }
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this);
            if (win != null) win.WindowState = WindowState.Minimized;
        }

        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this);
            if (win != null)
                win.WindowState = (win.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.Text == "Buscar...")
            {
                tb.Text = string.Empty;
                tb.Foreground = Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Buscar...";
                tb.Foreground = Brushes.Gray;
            }
        }
    }
}
