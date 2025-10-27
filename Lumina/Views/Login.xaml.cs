using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Lumina
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        // ================================
        // Barra superior (drag/ventana)
        // ================================
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { }
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e)
            => WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;

        private void Close_Click(object sender, RoutedEventArgs e)
            => Close();

        // ================================
        // Placeholders y borde negro
        // (usa el Tag del TextBox como texto de placeholder)
        // ================================
        private void Placeholder_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                string placeholder = tb.Tag as string ?? string.Empty;

                if (!string.IsNullOrEmpty(placeholder) && tb.Text == placeholder)
                {
                    tb.Text = string.Empty;
                    tb.Foreground = Brushes.Black;
                }

                // Borde negro al enfocar
                tb.BorderBrush = Brushes.Black;
                tb.BorderThickness = new Thickness(2);
            }
        }

        private void Placeholder_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                string placeholder = tb.Tag as string ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(placeholder) && string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Text = placeholder;
                    tb.Foreground = Brushes.Gray;
                }

                // Borde negro más delgado al perder foco
                tb.BorderBrush = Brushes.Black;
                tb.BorderThickness = new Thickness(1);
            }
        }

        // ================================
        // Guardar
        // ================================
        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Cambios guardados correctamente.", "Guardar", MessageBoxButton.OK, MessageBoxImage.Information);

            // Si quieres navegar a Homepage tras guardar:
            // new Lumina.Views.Homepage().Show();
            // this.Close();
        }
    }
}
