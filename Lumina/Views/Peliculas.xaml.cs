using Lumina.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

// Alias para evitar ambigüedad y usar los tipos correctos
using FavoritesStore = Lumina.Services.FavoritesStore;
using SMediaType = Lumina.Services.MediaType;
using MMediaType = Lumina.Model;

namespace Lumina.Views
{
    public partial class Peliculas : Window
    {
        private const string PlaceholderText = "Buscar...";
        private bool _placeholderActive = true;

        public Peliculas()
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
            // Ya estamos en películas, no hacer nada
        }

        private void Favoritos_Click(object sender, RoutedEventArgs e)
        {
            
            var favoritosWindow = new Favoritos();
            favoritosWindow.Owner = this;
            favoritosWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            favoritosWindow.Show();
            this.Hide();
            
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

        // ======= FAVORITOS: helpers y handlers =======

        private static (SMediaType type, string title, string? image) ParseTag(string tag)
        {
            // Divide en máximo 3 partes para no romper títulos con '|'
            var p = (tag ?? "").Split(new[] { '|' }, 3, StringSplitOptions.None);

            var type = SMediaType.Movie; // Por defecto "películas" para esta ventana
            if (p.Length > 0)
            {
                switch ((p[0] ?? "").Trim().ToLowerInvariant())
                {
                    case "movie": type = SMediaType.Movie; break;
                    case "music": type = SMediaType.Music; break;
                    case "book": type = SMediaType.Book; break;
                }
            }

            var title = p.Length > 1 ? (p[1] ?? "").Trim() : "";
            var image = p.Length > 2 ? (p[2] ?? "").Trim() : null;

            if (string.IsNullOrWhiteSpace(image)) image = null;
            return (type, title, image);
        }

        private static void SetStarIcon(Button btn, bool fav)
        {
            // Tu XAML usa <Button><Image .../></Button>, así que Content es Image
            if (btn.Content is Image img)
            {
                var uri = new Uri(
                    fav
                        ? "pack://application:,,,/Images/Iconos/star_filled.png"
                        : "pack://application:,,,/Images/Iconos/star_outline.png",
                    UriKind.Absolute);

                img.Source = new BitmapImage(uri);
            }
        }

        private void Star_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                var (type, title, _) = ParseTag(tag);
                SetStarIcon(btn, FavoritesStore.IsFavorite(title, type));
            }
        }

        private void Star_Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                var (type, title, image) = ParseTag(tag);
                var nowFav = FavoritesStore.Toggle(title, type, image);
                SetStarIcon(btn, nowFav);
            }
        }

        // Evento para manejar cuando se cierra la ventana
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Si esta ventana era la owner de otras, cierra la aplicación
            if (this.Owner == null)
            {
                Application.Current.Shutdown();
            }
        }
    }
}