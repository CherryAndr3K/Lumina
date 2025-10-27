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

namespace Lumina
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb.Text == "Buscar...")
            {
                tb.Text = "";
                tb.Foreground = Brushes.Black;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Buscar...";
                tb.Foreground = Brushes.Gray;
            }
        }


        // Permite arrastrar la ventana al hacer click en la barra superior
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var win = Window.GetWindow(this); // obtiene la Window que hospeda el Page
                win?.DragMove();
            }
        }

        // Minimizar
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Maximizar / Restaurar
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        // Cerrar
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Placeholder_GotFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb.Text == "Ingrese su nombre")
            {
                tb.Text = "";
                tb.Foreground = Brushes.LightGray;
            }
        }

        private void Placeholder_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;

            // Si el campo quedó vacío o con espacios
            if (string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Ingrese su nombre"; 
                tb.Foreground = Brushes.Gray;  
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para guardar los cambios
            MessageBox.Show("Cambios guardados correctamente.", "Guardar", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
        