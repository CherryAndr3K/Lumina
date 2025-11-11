using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        public Libros()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
                Loaded += (_, __) => { /* carga diferida si la necesitas */ };
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { /* ignore */ }
            }
        }

        // ================================
        // Ventana (métodos directos de Window)
        // ================================
        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e) =>
            WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        // Botón de editar superior
        private void LibrosR(object sender, RoutedEventArgs e)
        {
            var w = new LibrosR { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            w.Show();
            Hide();
        }

        // Navegación entre ventanas
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            var w = new Homepage { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            w.Show();
            Hide();
        }

        private void Libros_Click(object sender, RoutedEventArgs e) { /* ya estás aquí */ }

        private void Musica_Click(object sender, RoutedEventArgs e)
        {
            var w = new Musica { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            w.Show();
            Hide();
        }

        private void Peliculas_Click(object sender, RoutedEventArgs e)
        {
            var w = new Peliculas { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            w.Show();
            Hide();
        }

        private void Favoritos_Click(object sender, RoutedEventArgs e)
        {
            var w = new Favoritos { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            w.Show();
            Hide();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var w = new Login { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            w.Show();
            Hide();
        }

        // ======= FAVORITOS: helpers y handlers =======
        private readonly IFavoritoRepository _favRepo = new FavoritoRepository();

        private (string tipo, string titulo, string? image) ParseTag(string tag)
        {
            var p = (tag ?? "").Split(new[] { '|' }, 3, StringSplitOptions.None);
            var tipo = (p.Length > 0 ? p[0] : "").Trim();
            var titulo = (p.Length > 1 ? p[1] : "").Trim();
            var image = (p.Length > 2 ? p[2] : null);
            return (tipo, titulo, image);
        }

        private static string Cnn() =>
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private static int? ResolveReferenciaId_LibroPorTitulo(string titulo)
        {
            try
            {
                using var cn = new SqlConnection(Cnn());
                using var cmd = new SqlCommand("SELECT TOP 1 LibroID FROM dbo.Libros WHERE Titulo=@t", cn);
                cmd.Parameters.AddWithValue("@t", (object?)titulo ?? DBNull.Value);
                cn.Open();
                var o = cmd.ExecuteScalar();
                return (o == null || o == DBNull.Value) ? (int?)null : Convert.ToInt32(o);
            }
            catch
            {
                return null; // no romper UI si hay problema de conexión
            }
        }

        private static bool ExisteFavorito(int userId, string tipo, int referenciaId)
        {
            try
            {
                using var cn = new SqlConnection(Cnn());
                using var cmd = new SqlCommand(
                    "SELECT 1 FROM dbo.Favoritos WHERE UsuarioID=@u AND Tipo=@t AND ReferenciaID=@r", cn);
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
            // Usa las rutas REALES del proyecto (recursos de contenido)
            var relative = isFav ? "/Images/Iconos/star_filled.png" : "/Images/Iconos/star_outline.png";

            // Pack URI robusto para recursos empacados
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
                // Ignorar si el recurso no existe para no romper UI
            }
        }

        private void Star_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            if (sender is Button btn && btn.Tag is string tag)
            {
                var (tipo, titulo, _) = ParseTag(tag);  // "Book"
                if (!string.Equals(tipo, "Book", StringComparison.OrdinalIgnoreCase)) return;

                var id = ResolveReferenciaId_LibroPorTitulo(titulo);
                var isFav = id.HasValue && ExisteFavorito(AppSession.CurrentUserId, "Libro", id.Value);
                SetStarIcon(btn, isFav);
            }
        }

        private void Star_Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            if (sender is Button btn && btn.Tag is string tag)
            {
                var (tipo, titulo, _) = ParseTag(tag);
                if (!string.Equals(tipo, "Book", StringComparison.OrdinalIgnoreCase)) return;

                var id = ResolveReferenciaId_LibroPorTitulo(titulo);
                if (!id.HasValue)
                {
                    MessageBox.Show("No se encontró el libro en la BD.");
                    return;
                }

                var isFav = ExisteFavorito(AppSession.CurrentUserId, "Libro", id.Value);

                try
                {
                    if (isFav)
                    {
                        // Quitar de favoritos
                        var eliminado = (_favRepo as FavoritoRepository)?.DeleteFavorito(AppSession.CurrentUserId, "Libro", id.Value) ?? false;
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
                        // Agregar a favoritos
                        _ = _favRepo.Add(new Favorito
                        {
                            UsuarioId = AppSession.CurrentUserId,
                            Tipo = "Libro",
                            ReferenciaId = id.Value,
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
        }

        // === NUEVO MÉTODO PARA ENLACES DE LIBROS ===
        private void BookLink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string url)
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
            }
        }
    }
}
