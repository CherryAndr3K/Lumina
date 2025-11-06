using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lumina.Views
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { }
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void Placeholder_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                string placeholder = tb.Tag as string ?? string.Empty;
                if (tb.Text == placeholder)
                {
                    tb.Text = string.Empty;
                    tb.Opacity = 1.0;
                }
            }
        }

        private void Placeholder_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                string placeholder = tb.Tag as string ?? string.Empty;
                tb.Text = placeholder;
                tb.Opacity = 0.6;
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Inicio de sesión simulado (aún sin lógica).");
        }

        private void BtnRegistro_Click(object sender, RoutedEventArgs e)
        {
            var reg = new Register();
            reg.Owner = this;
            reg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            reg.ShowDialog();
        }
    }
}
