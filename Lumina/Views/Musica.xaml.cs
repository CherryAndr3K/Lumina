using Lumina.Infra;
using Lumina.Models;
using Lumina.Repositories;
using Microsoft.Data.SqlClient;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Lumina.Views
{
    public partial class Musica : Window
    {
        private const string PlaceholderText = "Buscar...";
        private bool _placeholderActive = true;

        // Repo de Favoritos
        private readonly IFavoritoRepository _favRepo = new FavoritoRepository();

        public Musica()
        {
            InitializeComponent();
            if (!DesignerProperties.GetIsInDesignMode(this))
                Loaded += (_, __) => { /* carga diferida si la necesitas */ };
        }

        // ================= Helpers BD / Favoritos =================
        private static string Cnn() =>
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private static void ParseTag(string tag, out string tipo, out string titulo, out string imagePath)
        {
            tipo = titulo = imagePath = "";
            if (string.IsNullOrWhiteSpace(tag)) return;
            var p = tag.Split('|');
            if (p.Length > 0) tipo = p[0].Trim();
            if (p.Length > 1) titulo = p[1].Trim();
            if (p.Length > 2) imagePath = p[2].Trim();
        }

        private static int? ResolveReferenciaId_AlbumPorTitulo(string titulo)
        {
            try
            {
                using var cn = new SqlConnection(Cnn());
                using var cmd = new SqlCommand("SELECT TOP 1 AlbumID FROM dbo.Albumes WHERE Titulo=@t", cn);
                cmd.Parameters.AddWithValue("@t", (object?)titulo ?? DBNull.Value);
                cn.Open();
                var o = cmd.ExecuteScalar();
                return (o == null || o == DBNull.Value) ? (int?)null : Convert.ToInt32(o);
            }
            catch
            {
                return null;
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
            var relative = isFav ? "/Images/Iconos/star_filled.png" : "/Images/Iconos/star_outline.png";
            var uri = new Uri($"pack://application:,,,{relative}", UriKind.Absolute);

            try
            {
                var bmp = new BitmapImage(uri);
                if (btn.Content is Image img) img.Source = bmp;
                else btn.Content = new Image { Source = bmp, Width = 18, Height = 18 };
            }
            catch { /* no romper UI si falta el recurso */ }
        }

        // ================= Barra superior / navegación =================
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) { try { DragMove(); } catch { } }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
            if (sender is Button btn && btn.Content is Image image)
            {
                var uri = new Uri(
                    WindowState == WindowState.Maximized
                        ? "pack://application:,,,/Images/Iconos/restaurar.png"
                        : "pack://application:,,,/Images/Iconos/maximizar.png",
                    UriKind.Absolute);
                try { image.Source = new BitmapImage(uri); } catch { }
            }
        }

        private void MusicaR(object sender, RoutedEventArgs e)
        {
            var w = new MusicaR { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            w.Show(); Hide();
        }

        private void Close_Click(object sender, RoutedEventArgs e) => Close();

        private void Home_Click(object sender, RoutedEventArgs e)
        { var w = new Homepage { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner }; w.Show(); Hide(); }

        private void Libros_Click(object sender, RoutedEventArgs e)
        { var w = new Libros { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner }; w.Show(); Hide(); }

        private void Peliculas_Click(object sender, RoutedEventArgs e)
        { var w = new Peliculas { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner }; w.Show(); Hide(); }

        private void Favoritos_Click(object sender, RoutedEventArgs e)
        { var w = new Favoritos { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner }; w.Show(); Hide(); }

        private void Login_Click(object sender, RoutedEventArgs e)
        { var w = new Login { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner }; w.Show(); Hide(); }

        // ================= Placeholder búsqueda =================
        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && _placeholderActive) { tb.Text = string.Empty; tb.Opacity = 1.0; _placeholderActive = false; }
        }
        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text)) { tb.Text = PlaceholderText; tb.Opacity = 0.6; _placeholderActive = true; }
        }
        private void InitSearchPlaceholderIfNeeded(TextBox tb)
        {
            if (tb != null && string.IsNullOrWhiteSpace(tb.Text)) { tb.Text = PlaceholderText; tb.Opacity = 0.6; _placeholderActive = true; }
        }

        // ================= Botón ⭐ por tarjeta =================
        private void Star_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            if (sender is not Button btn || btn.Tag is not string tag) return;

            ParseTag(tag, out var tipo, out var titulo, out _);
            if (!string.Equals(tipo, "Music", StringComparison.OrdinalIgnoreCase)) return;

            var id = ResolveReferenciaId_AlbumPorTitulo(titulo);
            var isFav = id.HasValue && ExisteFavorito(AppSession.CurrentUserId, "Album", id.Value);
            SetStarIcon(btn, isFav);
        }

        private void Star_Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            if (sender is not Button btn || btn.Tag is not string tag) return;

            ParseTag(tag, out var tipo, out var titulo, out _);
            if (!string.Equals(tipo, "Music", StringComparison.OrdinalIgnoreCase)) return;

            var id = ResolveReferenciaId_AlbumPorTitulo(titulo);
            if (!id.HasValue) { MessageBox.Show("No se encontró el álbum en la BD."); return; }

            var isFav = ExisteFavorito(AppSession.CurrentUserId, "Album", id.Value);
            if (isFav) { MessageBox.Show("Ya está en Favoritos."); SetStarIcon(btn, true); return; }

            try
            {
                _ = _favRepo.Add(new Favorito
                {
                    UsuarioId = AppSession.CurrentUserId,
                    Tipo = "Album",
                    ReferenciaId = id.Value
                });
                SetStarIcon(btn, true);
                MessageBox.Show($"Añadido a Favoritos: {titulo}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo agregar: " + ex.Message);
            }
        }

        private void MusicLink_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string url)
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch
                {
                    MessageBox.Show("No se pudo abrir el enlace.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (Owner == null) Application.Current.Shutdown();
        }
    }
}
