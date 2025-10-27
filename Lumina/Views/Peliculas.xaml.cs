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

namespace Lumina.Views
{
    /// <summary>
    /// Lógica de interacción para Peliculas.xaml
    /// </summary>
    public partial class Peliculas : Page
    {
        public Peliculas()
        {
            InitializeComponent();
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

        //Minimizar
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this);
            if (win != null)
                win.WindowState = WindowState.Minimized;
        }

        // Maximizar
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this);
            if (win != null)
            {
                if (win.WindowState == WindowState.Maximized)
                    win.WindowState = WindowState.Normal;
                else
                    win.WindowState = WindowState.Maximized;
            }
        }


        // Cerrar
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            var win = Window.GetWindow(this);
            win?.Close();

        }
    }
}
