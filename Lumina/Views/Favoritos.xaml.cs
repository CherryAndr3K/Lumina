using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Lumina.Services;   

namespace Lumina.Views
{
    public partial class Favoritos : Page
    {
        public Favoritos()
        {
            InitializeComponent();
            Loaded += (_, __) => ApplyFilter();   // arranca vacío/actualizado
        }

        // -------- Barra superior (controla la ventana que hospeda la Page) --------
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                Window.GetWindow(this)?.DragMove();
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
            => Window.GetWindow(this)!.WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            var w = Window.GetWindow(this)!;
            w.WindowState = (w.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }
        private void Close_Click(object sender, RoutedEventArgs e)
            => Window.GetWindow(this)?.Close();

        // -------- Buscador --------
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.Text == "Buscar...")
            { tb.Text = ""; tb.Foreground = Brushes.Black; }
        }
        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            { tb.Text = "Buscar..."; tb.Foreground = Brushes.Gray; }
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

        // -------- Lista / estado vacío --------
        private void ApplyFilter()
        {
            var q = (SearchBox?.Text ?? "").Trim().ToLower();
            List<FavoriteItem> items = string.IsNullOrEmpty(q)
                ? FavoritesStore.Items.ToList()
                : FavoritesStore.Items.Where(it =>
                      (it.Title?.ToLower().Contains(q) ?? false) ||
                      it.Type.ToString().ToLower().Contains(q)).ToList();

            FavList.ItemsSource = null;
            FavList.ItemsSource = items;
            EmptyState.Visibility = items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        // ⭐ quitar desde Favoritos
        private void Star_Remove_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is FavoriteItem it)
            {
                FavoritesStore.Toggle(it.Title, it.Type, it.ImagePath); // quita
                ApplyFilter(); // refresca
            }
        }

        // -------- Navegación lateral --------
        private void GoHome_Click(object s, RoutedEventArgs e) => NavigationService?.Navigate(new Homepage());
        private void GoBooks_Click(object s, RoutedEventArgs e) => NavigationService?.Navigate(new Libros());
        private void GoMusic_Click(object s, RoutedEventArgs e) => NavigationService?.Navigate(new Musica());
        private void GoMovies_Click(object s, RoutedEventArgs e) => NavigationService?.Navigate(new Peliculas());
        private void GoFavs_Click(object s, RoutedEventArgs e) => NavigationService?.Navigate(new Favoritos()); 
    }
}
