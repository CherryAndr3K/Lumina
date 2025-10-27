using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lumina.Views
{
    public partial class Homepage : Window
    {
        private const string PlaceholderText = "Buscar...";
        private bool _placeholderActive = true;

        public Homepage()
        {
            InitializeComponent();

            // Ajusta la altura máxima para que, al maximizar con WindowStyle=None,
            // no cubra la barra de tareas.
            MaxHeight = SystemParameters.WorkArea.Height;
            MaxWidth = SystemParameters.WorkArea.Width;
        }

        // ================================
        // Barra de título personalizada
        // ================================
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    // Doble clic: maximizar/restaurar
                    if (e.ClickCount == 2)
                    {
                        ToggleMaximizeRestore();
                        // Si agregas x:Name="MaxIcon" a la imagen del botón,
                        // también actualizamos el ícono aquí.
                        UpdateMaxIconIfNamed();
                    }
                    else if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        DragMove();
                    }
                }
            }
            catch
            {
                // Evita fallos al hacer DragMove en estados no válidos.
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximizeRestore();

            // Actualiza el ícono del propio botón (su Content es un <Image/>)
            if (sender is Button btn && btn.Content is Image img)
            {
                SetMaximizeIcon(WindowState == WindowState.Maximized, img);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ToggleMaximizeRestore()
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        // Intenta encontrar una imagen llamada "MaxIcon" en XAML para actualizarla
        // cuando maximices/restaures con doble clic en la barra.
        private void UpdateMaxIconIfNamed()
        {
            var named = FindName("MaxIcon") as Image;
            if (named != null)
            {
                SetMaximizeIcon(WindowState == WindowState.Maximized, named);
            }
        }

        private void SetMaximizeIcon(bool isMaximized, Image targetImage)
        {
            // Cambia entre "maximizar.png" y "restaurar.png"
            var uri = new Uri(
                isMaximized
                    ? "pack://application:,,,/Images/Iconos/restaurar.png"
                    : "pack://application:,,,/Images/Iconos/maximizar.png",
                UriKind.Absolute);

            try
            {
                targetImage.Source = new BitmapImage(uri);
            }
            catch
            {
                // Si la imagen no existe o hay un problema de recurso, no rompas la app.
            }
        }

        // ================================
        // Caja de búsqueda (placeholder)
        // ================================
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (_placeholderActive)
                {
                    tb.Text = string.Empty;
                    tb.Opacity = 1.0;
                    _placeholderActive = false;
                }
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                if (string.IsNullOrWhiteSpace(tb.Text))
                {
                    tb.Text = PlaceholderText;
                    tb.Opacity = 0.6; // se nota como placeholder
                    _placeholderActive = true;
                }
            }
        }

        // Opcional: si quieres inicializar el placeholder al cargar,
        // llama a esto desde el constructor si tu TextBox empieza vacío.
        private void InitSearchPlaceholderIfNeeded(TextBox tb)
        {
            if (tb != null && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = PlaceholderText;
                tb.Opacity = 0.6;
                _placeholderActive = true;
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
    }
}
