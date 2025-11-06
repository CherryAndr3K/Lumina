using Lumina.Repositories;
using Lumina.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Lumina.Views
{
    public partial class Favoritos : Window
    {
        private const string PlaceholderText = "Buscar...";
        private bool _placeholderActive = true;

        // Repositorio
        private readonly IFavoritoRepository _favRepo = new FavoritoRepository();

        // Clase auxiliar para mostrar los favoritos en la UI
        private class FavoriteCard
        {
            public int FavoritoId { get; set; }  // 👈 corregido
            public string Title { get; set; } = "";
            public string Type { get; set; } = "";   // Album | Libro | Pelicula
            public string ImagePath { get; set; } = "/Images/Iconos/Favoritos.png";
        }

        private ObservableCollection<FavoriteCard> _all = new();
        private ObservableCollection<FavoriteCard> _filtered = new();

        public Favoritos()
        {
            InitializeComponent(); // siempre primero
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitSearchPlaceholderIfNeeded(SearchBox);
            LoadFromRepository();
            ApplyFilter(); // inicializa lista e indicador vacío
        }

        private void LoadFromRepository()
        {
            try
            {
                var views = _favRepo.GetAllViews(); // FavoritoView
                var list = views.Select(v => new FavoriteCard
                {
                    FavoritoId = v.FavoritoId, // 👈 corregido
                    Title = string.IsNullOrWhiteSpace(v.ReferenciaTitulo)
                        ? "(Sin título)"
                        : v.ReferenciaTitulo,
                    Type = v.Tipo,
                    ImagePath = IconFor(v.Tipo)
                }).ToList();

                _all = new ObservableCollection<FavoriteCard>(list);
                _filtered = new ObservableCollection<FavoriteCard>(_all);
                FavList.ItemsSource = _filtered;

                EmptyState.Visibility = _filtered.Count == 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error cargando favoritos: " + ex.Message);
            }
        }

        private string IconFor(string tipo)
        {
            // Ajusta a tus iconos reales
            return tipo switch
            {
                "Album" => "/Images/Iconos/Musica.png",
                "Libro" => "/Images/Iconos/Libros.png",
                "Pelicula" => "/Images/Iconos/Pelicula.png",
                _ => "/Images/Iconos/Favoritos.png"
            };
        }

        // ===== Barra superior =====
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { }
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Maximized)
                ? WindowState.Normal
                : WindowState.Maximized;

            if (sender is Button btn && btn.Content is Image image)
            {
                SetMaximizeIcon(WindowState == WindowState.Maximized, image);
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
            => Close();

        private void SetMaximizeIcon(bool isMaximized, Image targetImage)
        {
            var uri = new Uri(
                isMaximized
                    ? "pack://application:,,,/Images/Iconos/restaurar.png"
                    : "pack://application:,,,/Images/Iconos/maximizar.png",
                UriKind.Absolute);
            try { targetImage.Source = new BitmapImage(uri); } catch { }
        }

        // ===== Navegación =====
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            var homeWindow = new Homepage
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            homeWindow.Show();
            Hide();
        }

        private void Libros_Click(object sender, RoutedEventArgs e)
        {
            var w = new Libros
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            w.Show();
            Hide();
        }

        private void Musica_Click(object sender, RoutedEventArgs e)
        {
            var w = new Musica
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            w.Show();
            Hide();
        }

        private void Peliculas_Click(object sender, RoutedEventArgs e)
        {
            var w = new Peliculas
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            w.Show();
            Hide();
        }

        private void Favoritos_Click(object sender, RoutedEventArgs e)
        {
            // Ya estás en favoritos
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Login();
            loginWindow.Owner = this;
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            loginWindow.Show();
            this.Hide();
        }

        // ===== Placeholder búsqueda =====
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && _placeholderActive)
            {
                tb.Text = string.Empty;
                tb.Opacity = 1.0;
                _placeholderActive = false;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = PlaceholderText;
                tb.Opacity = 0.6;
                _placeholderActive = true;
            }
        }

        private void InitSearchPlaceholderIfNeeded(TextBox tb)
        {
            if (tb != null && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = PlaceholderText;
                tb.Opacity = 0.6;
                _placeholderActive = true;
            }
        }

        // ===== Filtrado y acciones =====
        private void ApplyFilter()
        {
            if (FavList == null || EmptyState == null) return; // seguridad

            var q = (SearchBox?.Text ?? "").Trim().ToLower();
            bool useAll = string.IsNullOrEmpty(q) || q == PlaceholderText.ToLower();

            var src = useAll
                ? _all
                : new ObservableCollection<FavoriteCard>(
                    _all.Where(it =>
                        (it.Title?.ToLower().Contains(q) ?? false) ||
                        (it.Type?.ToLower().Contains(q) ?? false)));

            _filtered.Clear();
            foreach (var x in src) _filtered.Add(x);

            FavList.ItemsSource = _filtered;
            EmptyState.Visibility = _filtered.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyFilter();

        // ⭐ quitar desde Favoritos (usa repositorio)
        private void Star_Remove_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.Tag is FavoriteCard card)
            {
                var confirm = MessageBox.Show(
                    $"¿Quitar '{card.Title}' de favoritos?",
                    "Confirmar",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (confirm != MessageBoxResult.Yes) return;

                try
                {
                    if (_favRepo.Delete(card.FavoritoId)) 
                    {
                        // quita de _all y vuelve a filtrar
                        var item = _all.FirstOrDefault(x => x.FavoritoId == card.FavoritoId); 
                        if (item != null) _all.Remove(item);
                        ApplyFilter();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar el favorito.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            }
        }
    }
}
