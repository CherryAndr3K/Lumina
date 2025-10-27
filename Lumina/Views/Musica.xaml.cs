using Lumina.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;


// Alias para evitar ambigüedad y usar los tipos correctos
using FavoritesStore = Lumina.Services.FavoritesStore;
using SMediaType = Lumina.Services.MediaType;  // el que usan los métodos de FavoritesStore
using MMediaType = Lumina.Model.MediaType;     // opcional si lo necesitas en otro lado
namespace Lumina.Views
{
    public partial class Musica : Page
    {
        public Musica()
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

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Homepage());
        }

        private void Libros_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Libros());
        }

        private void Musica_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Musica());
        }

        private void Pelicula_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Peliculas());
        }

        private void Favoritos_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Favoritos());
        }


        // ======= FAVORITOS: helpers y handlers =======

        private static (SMediaType type, string title, string? image) ParseTag(string tag)
        {
            // Divide en máximo 3 partes para no romper títulos con '|'
            var p = (tag ?? "").Split(new[] { '|' }, 3, StringSplitOptions.None);

            var type = SMediaType.Book;
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
                var (type, title, _) = ParseTag(tag);                     // type = SMediaType
                SetStarIcon(btn, FavoritesStore.IsFavorite(title, type)); // coincide el tipo
            }
        }

        private void Star_Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                var (type, title, image) = ParseTag(tag);                 // type = SMediaType
                var nowFav = FavoritesStore.Toggle(title, type, image);   // coincide el tipo
                SetStarIcon(btn, nowFav);
            }
        }
    }

}





        