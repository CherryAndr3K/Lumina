using Lumina.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

// Alias para evitar ambigüedad y usar los tipos correctos
using FavoritesStore = Lumina.Services.FavoritesStore;
using SMediaType = Lumina.Services.MediaType;
using MMediaType = Lumina.Model.MediaType;

namespace Lumina.Views
{
    public partial class Favoritos : Window
    {
        private const string PlaceholderText = "Buscar...";
        private bool _placeholderActive = true;

        public Favoritos()
        {
            InitializeComponent();
            
        }

        

        // Permite arrastrar la ventana al hacer click en la barra superior
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try { this.DragMove(); } catch { }
            }
        }

        // Minimizar
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Maximizar/Restaurar
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;

            // Actualizar el ícono después de cambiar el estado
            if (sender is Button btn && btn.Content is Image image)
            {
                SetMaximizeIcon(this.WindowState == WindowState.Maximized, image);
            }
        }

        // Cerrar
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Método para cambiar entre íconos de maximizar/restaurar
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

        // Navegación entre ventanas (ahora se abren nuevas ventanas en lugar de navegar)
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            var homeWindow = new Homepage();
            homeWindow.Owner = this;
            homeWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            homeWindow.Show();
            this.Hide();
        }

        private void Libros_Click(object sender, RoutedEventArgs e)
        {
            var librosWindow = new Libros();
            librosWindow.Owner = this;
            librosWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            librosWindow.Show();
            this.Hide();
        }

        private void Musica_Click(object sender, RoutedEventArgs e)
        {
            var musicaWindow = new Musica();
            musicaWindow.Owner = this;
            musicaWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            musicaWindow.Show();
            this.Hide();
        }

        private void Peliculas_Click(object sender, RoutedEventArgs e)
        {
            var peliculasWindow = new Peliculas();
            peliculasWindow.Owner = this;
            peliculasWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            peliculasWindow.Show();
            this.Hide();
        }

        private void Favoritos_Click(object sender, RoutedEventArgs e)
        {
            // Ya estamos en favoritos, no hacer nada
        }

        // ================================
        // Caja de búsqueda (placeholder)
        // ================================
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && _placeholderActive)
            {
                tb.Text = string.Empty;
                tb.Opacity = 1.0; // se ve como texto real
                _placeholderActive = false;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = PlaceholderText;
                tb.Opacity = 0.6; // aspecto de placeholder
                _placeholderActive = true;
            }
        }

        // Opcional: inicializar placeholder al cargar
        private void InitSearchPlaceholderIfNeeded(TextBox tb)
        {
            if (tb != null && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = PlaceholderText;
                tb.Opacity = 0.6;
                _placeholderActive = true;
            }
        }

        // -------- Lista / estado vacío --------
        private void ApplyFilter()
        {
            var q = (SearchBox?.Text ?? "").Trim().ToLower();
            List<FavoriteItem> items = string.IsNullOrEmpty(q) || q == "buscar..."
                ? FavoritesStore.Items.ToList()
                : FavoritesStore.Items.Where(it =>
                      (it.Title?.ToLower().Contains(q) ?? false) ||
                      it.Type.ToString().ToLower().Contains(q)).ToList();

            FavList.ItemsSource = null;
            FavList.ItemsSource = items;
            EmptyState.Visibility = items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

        // ⭐ quitar desde Favoritos
        private void Star_Remove_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is FavoriteItem it)
            {
                FavoritesStore.Toggle(it.Title, it.Type, it.ImagePath); // quita
                ApplyFilter(); // refresca
            }
        }

        
    }
}