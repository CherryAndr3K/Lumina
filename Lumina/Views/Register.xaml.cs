using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Lumina.Models; // MediaAppDbContext, Usuario

namespace Lumina.Views
{
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent(); // No implementes este método manualmente
        }

        // ========== Barra de título / Ventana ==========
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { /* ignore */ }
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e)
            => WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;

        private void Close_Click(object sender, RoutedEventArgs e)
            => Close();

        // ========== Navegación lateral (opcional si existen esas ventanas) ==========
        private void Libros_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var w = new Libros { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                w.Show(); this.Hide();
            }
            catch { MessageBox.Show("Ventana Libros no disponible."); }
        }

        private void Musica_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var w = new Musica { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                w.Show(); this.Hide();
            }
            catch { MessageBox.Show("Ventana Música no disponible."); }
        }

        private void Peliculas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var w = new Peliculas { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
                w.Show(); this.Hide();
            }
            catch { MessageBox.Show("Ventana Películas no disponible."); }
        }

        private void Favoritos_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Favoritos (pendiente).");
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var w = new Login { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            w.Show(); Hide();
        }

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

        // ========== Registrar ==========
        private void BtnRegistro_Click(object sender, RoutedEventArgs e)
        {
            // Estos nombres vienen de tu XAML:
            // txtNombre, txtCorreo, txtPassword, txtPassword2 (todos TextBox)
            string nombre = txtNombre?.Text?.Trim() ?? "";
            string correo = txtCorreo?.Text?.Trim() ?? "";
            string pass1 = txtPassword?.Text?.Trim() ?? "";
            string pass2 = txtPassword2?.Text?.Trim() ?? "";

            // Si están en placeholder, vaciarlos
            if (nombre == (txtNombre?.Tag as string)) nombre = "";
            if (correo == (txtCorreo?.Tag as string)) correo = "";
            if (pass1 == (txtPassword?.Tag as string)) pass1 = "";
            if (pass2 == (txtPassword2?.Tag as string)) pass2 = "";

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(pass1) ||
                string.IsNullOrWhiteSpace(pass2))
            {
                MessageBox.Show("Por favor completa todos los campos.",
                                "Registro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.Equals(pass1, pass2, StringComparison.Ordinal))
            {
                MessageBox.Show("Las contraseñas no coinciden.",
                                "Registro", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new MediaAppDbContext())
                {
                    var nuevoUsuario = new Usuario
                    {
                        Correo = correo,
                        Nombre = nombre,
                        Contrasena = pass1
                    };

                    context.Usuarios.Add(nuevoUsuario);
                    context.SaveChanges();
                }

                MessageBox.Show("Usuario registrado correctamente.",
                                "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar: " + ex.Message,
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
