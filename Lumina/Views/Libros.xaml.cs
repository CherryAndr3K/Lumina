using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Lumina.Infra;
using Lumina.Models;
using Lumina.Repositories;
using Microsoft.Data.SqlClient;

namespace Lumina.Views
{
    public partial class Libros : Window
    {
        private const string PlaceholderText = "Buscar...";
        private bool _placeholderActive = true;

        // Lista que se enlaza al ItemsControl del XAML
        public ObservableCollection<Libro> LibrosList { get; } = new();

        // Repo de favoritos
        private readonly IFavoritoRepository _favRepo = new FavoritoRepository();

        public Libros()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = this;              // para los bindings
                Loaded += Libros_Loaded;         // carga al abrir la ventana
            }
        }

        private void Libros_Loaded(object? sender, RoutedEventArgs e)
        {
            CargarLibrosDesdeBD();
        }

        // ============= BARRA DE TÍTULO =============
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { /* ignore */ }
            }
        }

        // ============= SEARCHBOX PLACEHOLDER =============
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && _placeholderActive)
            {
                tb.Text = string.Empty;
                tb.Foreground = Brushes.Black;
                _placeholderActive = false;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = PlaceholderText;
                tb.Foreground = Brushes.Gray;
                _placeholderActive = true;
            }
        }

        // ============= VENTANA (MIN/MAX/CLOSE) =============
        private void Minimize_Click(object sender, RoutedEventArgs e) =>
            WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e) =>
            WindowState = (WindowState == WindowState.Maximized)
                ? WindowState.Normal
                : WindowState.Maximized;

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        // ============= NAVEGACIÓN =============
        private void LibrosR(object sender, RoutedEventArgs e)
        {
            var w = new LibrosR
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            w.Show();
            Hide();
        }

        private void Home_Click(object sender, RoutedEventArgs e)
        {
            var w = new Homepage
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            w.Show();
            Hide();
        }

        private void Libros_Click(object sender, RoutedEventArgs e)
        {
            // ya estás aquí
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
            var w = new Favoritos
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            w.Show();
            Hide();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var w = new Login
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            w.Show();
            Hide();
        }

        // ============= BASE DE DATOS =============

        private static string Cnn() =>
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private void CargarLibrosDesdeBD()
        {
            LibrosList.Clear();

            try
            {
                using var cn = new SqlConnection(Cnn());
                using var cmd = new SqlCommand(
                    "SELECT LibroID, Titulo, Autor, Genero, Link, Imagen FROM dbo.Libros ORDER BY LibroID",
                    cn);

                cn.Open();
                using var rd = cmd.ExecuteReader();
                while (rd.Read())
                {
                    var libro = new Libro
                    {
                        LibroId = rd.IsDBNull(0) ? 0 : rd.GetInt32(0),
                        Titulo = rd.IsDBNull(1) ? string.Empty : rd.GetString(1),
                        Autor = rd.IsDBNull(2) ? string.Empty : rd.GetString(2),
                        Genero = rd.IsDBNull(3) ? string.Empty : rd.GetString(3),
                        Link = rd.IsDBNull(4) ? string.Empty : rd.GetString(4),
                        Imagen = rd.IsDBNull(5) ? string.Empty : rd.GetString(5)
                    };

                    LibrosList.Add(libro);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar libros: " + ex.Message);
            }
        }

        private static bool ExisteFavorito(int userId, string tipo, int referenciaId)
        {
            try
            {
                using var cn = new SqlConnection(Cnn());
                using var cmd = new SqlCommand(
                    "SELECT 1 FROM dbo.Favoritos WHERE UsuarioID=@u AND Tipo=@t AND ReferenciaID=@r",
                    cn);
                cmd.Parameters.AddWithValue("@u", userId);
                cmd.Parameters.AddWithValue("@t", tipo);
                cmd.Parameters.AddWithValue("@r", referenciaId);
                cn.Open();
                using var rd = cmd.ExecuteReader();
                return rd.Read();
            }
            catch
            {
                return false;
            }
        }

        private static void SetStarIcon(Button btn, bool isFav)
        {
            var relative = isFav ? "/Images/Iconos/star_filled.png" : "/Images/Iconos/star_outline.png";
            var uri = new Uri($"pack://application:,,,{relative}", UriKind.Absolute);

            try
            {
                var bmp = new BitmapImage(uri);
                if (btn.Content is Image img)
                    img.Source = bmp;
                else
                    btn.Content = new Image { Source = bmp, Width = 18, Height = 18 };
            }
            catch
            {
                // Ignorar si el recurso no existe
            }
        }

        // ============= FAVORITOS =============

        private void Star_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            if (sender is not Button btn) return;
            if (btn.DataContext is not Libro libro) return;

            var isFav = ExisteFavorito(AppSession.CurrentUserId, "Libro", libro.LibroId);
            SetStarIcon(btn, isFav);
        }

        private void Star_Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            if (sender is not Button btn) return;
            if (btn.DataContext is not Libro libro) return;

            var id = libro.LibroId;
            var titulo = libro.Titulo ?? "(sin título)";
            var isFav = ExisteFavorito(AppSession.CurrentUserId, "Libro", id);

            try
            {
                if (isFav)
                {
                    var eliminado =
                        (_favRepo as FavoritoRepository)?.DeleteFavorito(AppSession.CurrentUserId, "Libro", id)
                        ?? false;

                    if (eliminado)
                    {
                        SetStarIcon(btn, false);
                        MessageBox.Show($"Eliminado de Favoritos: {titulo}");
                    }
                    else
                    {
                        MessageBox.Show("No se pudo eliminar de Favoritos.");
                    }
                }
                else
                {
                    _ = _favRepo.Add(new Favorito
                    {
                        UsuarioId = AppSession.CurrentUserId,
                        Tipo = "Libro",
                        ReferenciaId = id,
                        FechaMarcado = DateTime.Now
                    });

                    SetStarIcon(btn, true);
                    MessageBox.Show($"Añadido a Favoritos: {titulo}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar favoritos: " + ex.Message);
            }
        }

        // ============= LINK DEL LIBRO =============

        private void BookLink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            if (btn.DataContext is not Libro libro) return;

            var url = libro.Link;

            if (string.IsNullOrWhiteSpace(url))
            {
                MessageBox.Show("Este libro no tiene enlace configurado.");
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el enlace: " + ex.Message);
            }
        }
    }
}
