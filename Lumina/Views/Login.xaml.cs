using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Lumina.Models; // MediaAppDbContext, Usuario

namespace Lumina.Views
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent(); // No implementes este método manualmente
        }

        // ========== Barra de título / Ventana ==========
        private void Admin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var adminWindow = new AdminUsuarios
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                adminWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir la ventana de administración: " + ex.Message,
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    



private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { /* ignora */ }
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e)
            => WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;

        private void Close_Click(object sender, RoutedEventArgs e)
            => Close();

        // ========== Placeholders (TextBox usa Tag como placeholder) ==========
        private void Placeholder_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                string placeholder = tb.Tag as string ?? string.Empty;

                if (!string.IsNullOrEmpty(placeholder) && tb.Text == placeholder)
                {
                    tb.Text = string.Empty;
                    tb.Foreground = Brushes.Black;
                    tb.Opacity = 1.0;
                }

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
                    tb.Opacity = 0.6;
                }

                tb.BorderBrush = Brushes.Black;
                tb.BorderThickness = new Thickness(1);
            }
        }

        // ========== Navegación lateral ==========
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var w = new Homepage { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                w.Show();
                this.Hide();
            }
            catch (Exception)
            {
                MessageBox.Show("Ventana Home no disponible.", "Navegación", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Libros_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var w = new Libros { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                w.Show();
                this.Hide();
            }
            catch (Exception)
            {
                MessageBox.Show("Ventana Libros no disponible.", "Navegación", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Musica_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var w = new Musica { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                w.Show();
                this.Hide();
            }
            catch (Exception)
            {
                MessageBox.Show("Ventana Música no disponible.", "Navegación", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Peliculas_Click(object sender, RoutedEventArgs e)
        {
            // Ya estás en Películas (según tu XAML este botón está en la barra lateral).
            MessageBox.Show("Ya estás en la sección Películas.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Favoritos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var w = new Favoritos { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                w.Show();
                this.Hide();
            }
            catch (Exception)
            {
                MessageBox.Show("Ventana Favoritos no disponible.", "Navegación", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ========== Abrir Login desde otros controles (si aplica) ==========
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Si este botón abre la ventana de login desde otras vistas
            // simplemente aseguramos que la ventana esté visible/centrada.
            try
            {
                var w = new Login { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                w.Show();
                this.Hide();
            }
            catch (Exception)
            {
                MessageBox.Show("No se puede abrir la ventana de Login.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ========== Entrar (validaciones + verificación en DB) ==========
        private void Entrar_Click(object sender, RoutedEventArgs e)
        {
            string correo = txtCorreo?.Text?.Trim() ?? "";
            string password = txtPassword?.Text?.Trim() ?? "";

            // Quitar placeholders si se usan Tag
            if (correo == (txtCorreo?.Tag as string)) correo = "";
            if (password == (txtPassword?.Tag as string)) password = "";

            // Validaciones con mensajes específicos
            if (string.IsNullOrWhiteSpace(correo) && string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor ingresa tu correo y contraseña.",
                                "Campos vacíos", MessageBoxButton.OK, MessageBoxImage.Warning);
                // Opcional: marcar visualmente
                MarkControlInvalid(txtCorreo);
                MarkControlInvalid(txtPassword);
                return;
            }

            if (string.IsNullOrWhiteSpace(correo))
            {
                MessageBox.Show("Por favor ingresa tu correo.",
                                "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                MarkControlInvalid(txtCorreo);
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor ingresa tu contraseña.",
                                "Campo requerido", MessageBoxButton.OK, MessageBoxImage.Warning);
                MarkControlInvalid(txtPassword);
                return;
            }

            try
            {
                using (var context = new MediaAppDbContext())
                {
                    var usuario = context.Usuarios
                        .FirstOrDefault(u => u.Correo == correo && u.Contrasena == password);

                    if (usuario == null)
                    {
                        MessageBox.Show("Correo o contraseña incorrectos.",
                                        "Error de inicio de sesión", MessageBoxButton.OK, MessageBoxImage.Error);
                        // Opcional: marcar ambos controles
                        MarkControlInvalid(txtCorreo);
                        MarkControlInvalid(txtPassword);
                        return;
                    }
                }

                MessageBox.Show("Inicio de sesión exitoso.",
                                "Bienvenido", MessageBoxButton.OK, MessageBoxImage.Information);

                var w = new Homepage { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                w.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al iniciar sesión: " + ex.Message,
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ========== Abrir registro ==========
        private void BtnRegistro_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reg = new Register
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                reg.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el registro: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ========== Utilidades visuales (no obligatorias) ==========
        // Marca visualmente un TextBox como inválido (borde más grueso y rojo).
        // Si no quieres este comportamiento, puedes eliminar estas llamadas.
        private void MarkControlInvalid(Control control)
        {
            if (control is TextBox tb)
            {
                tb.BorderBrush = Brushes.Red;
                tb.BorderThickness = new Thickness(2);
                tb.Focus();
            }
        }
    }
}
