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

                if (tb.Text == placeholder)
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

                if (string.IsNullOrWhiteSpace(tb.Text))
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
        // Guardar (placeholder de acción)
        // ================================
        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Guardado.", "Login", MessageBoxButton.OK, MessageBoxImage.Information);
            // Aquí puedes validar y navegar a Homepage si quieres:
            // new Lumina.Views.Homepage().Show();
            // this.Close();
        }
    }
}
