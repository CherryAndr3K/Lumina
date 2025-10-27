using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Lumina.Views
{
    public partial class Homepage : Window
    {
        private const string PlaceholderText = "Buscar...";
        private bool _placeholderActive = true;

        public Homepage()
        {
            InitializeComponent();

            // Para que la ventana sin borde no tape la barra de tareas al maximizar
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
                    if (e.ClickCount == 2)
                    {
                        ToggleMaximizeRestore();
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

        // Minimizar
        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        // Maximizar / Restaurar (handler principal)
        private void MaximizeRestore_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximizeRestore();

            // Si el botón tiene un <Image/> como Content, actualiza el ícono
            if (sender is Button btn && btn.Content is Image img)
            {
                SetMaximizeIcon(WindowState == WindowState.Maximized, img);
            }
        }

        // Compatibilidad si en XAML dejaste Maximize_Click
        private void Maximize_Click(object sender, RoutedEventArgs e)
            => MaximizeRestore_Click(sender, e);

        // Cerrar
        private void Close_Click(object sender, RoutedEventArgs e)
            => Close();

        private void ToggleMaximizeRestore()
        {
            WindowState = (WindowState == WindowState.Normal)
                          ? WindowState.Maximized
                          : WindowState.Normal;
        }

        // Intenta encontrar una imagen llamada "MaxIcon" en XAML para actualizarla
        private void UpdateMaxIconIfNamed()
        {
            if (FindName("MaxIcon") is Image named)
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
                // Si la imagen no existe o hay un problema de recurso, ignora.
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
                    tb.Opacity = 1.0; // texto real
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
                    tb.Opacity = 0.6; // aspecto placeholder
                    _placeholderActive = true;
                }
            }
        }

        // Inicializa el placeholder al cargar (si lo llamas desde Loaded)
        private void InitSearchPlaceholderIfNeeded(TextBox tb)
        {
            if (tb != null && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = PlaceholderText;
                tb.Opacity = 0.6;
                _placeholderActive = true;
            }
        }
    }
}
