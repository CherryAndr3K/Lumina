using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Lumina.Models;

namespace Lumina.Views
{
    /// <summary>
    /// Lógica de interacción para Window1.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }


        // Placeholders y borde negro (como en Login)
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

                tb.BorderBrush = Brushes.Black;
                tb.BorderThickness = new Thickness(1);
            }
        }

        private void BtnRegistrar_Click(object sender, RoutedEventArgs e)
        {
            string correo = txtCorreo.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Por favor completa todos los campos.",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new MediaAppDbContext()) // 👈 Usa tu contexto generado
                {
                    var nuevoUsuario = new Usuario
                    {
                        Correo = correo,
                        Nombre = nombre,
                        Contrasena = password
                    };

                    context.Usuarios.Add(nuevoUsuario);
                    context.SaveChanges();
                }

                MessageBox.Show("Usuario registrado correctamente.",
                                "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);

                this.Close(); // Cierra la ventana de registro
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar: " + ex.Message,
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}


