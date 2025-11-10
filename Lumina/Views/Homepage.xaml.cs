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
    public partial class Homepage : Window
    {
        private const string PlaceholderText = "Buscar...";
        private bool _placeholderActive = true;

        // Repo de favoritos
        private readonly IFavoritoRepository _favRepo = new FavoritoRepository();

        public Homepage()
        {
            InitializeComponent();
            // Evita ejecutar lógica de datos en diseñador
            if (!DesignerProperties.GetIsInDesignMode(this))
                Loaded += Homepage_Loaded;
        }

        private void Homepage_Loaded(object sender, RoutedEventArgs e)
        {
            // Si necesitas inicializar algo al abrir la ventana, hazlo aquí.
            // (Dejé vacío para no tocar tu flujo actual)
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

        // ================================
        // Navegación del menú
        // ================================

        // Libros
        private void Libros_Click(object sender, RoutedEventArgs e)
        {
            var w = new Libros { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            w.Show();
            Hide();
        }

        // Música
        private void Musica_Click(object sender, RoutedEventArgs e)
        {
            var w = new Musica { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            w.Show();
            Hide();
        }

        // Películas
        private void Peliculas_Click(object sender, RoutedEventArgs e)
        {
            var w = new Peliculas { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            w.Show();
            Hide();
        }

        // Favoritos
        private void Favoritos_Click(object sender, RoutedEventArgs e)
        {
            var w = new Favoritos { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            w.Show();
            Hide();
        }

        // Login
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var w = new Login { Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner };
            w.Show();
            Hide();
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
                return null; // no revientes la UI si hay un fallo de conexión
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
            var path = isFav
                ? "/Images/Iconos/star_filled.png"
                : "/Images/Iconos/star_outline.png";

            var uri = new Uri(path, UriKind.Relative);
            if (btn.Content is Image img)
            {
                img.Source = new BitmapImage(uri);
            }
            else
            {
                btn.Content = new Image
                {
                    Source = new BitmapImage(uri),
                    Width = 18,
                    Height = 18
                };
            }
        }

        // ================= Botón ⭐ por tarjeta =================
        private void Star_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this)) return;
            if (sender is not Button btn || btn.Tag is not string tag) return;

            try
            {
                ParseTag(tag, out var tipo, out var titulo, out _);
                if (!string.Equals(tipo, "Music", StringComparison.OrdinalIgnoreCase)) return;

                var id = ResolveReferenciaId_AlbumPorTitulo(titulo);
                var isFav = id.HasValue && ExisteFavorito(AppSession.CurrentUserId, "Album", id.Value);
                SetStarIcon(btn, isFav);
            }
            catch
            {
                // No bloquear la UI por un fallo de BD
                SetStarIcon(btn, false);
            }
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

        // ================================
        // Botones de ventana
        // ================================

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

        // Compatibilidad si en XAML usas Maximize_Click
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
            var uri = new Uri(
                isMaximized
                    ? "pack://application:,,,/Images/Iconos/restaurar.png"
                    : "pack://application:,,,/Images/Iconos/maximizar.png",
                UriKind.Absolute);

            try { targetImage.Source = new BitmapImage(uri); }
            catch { /* Ignorar si el recurso no existe */ }
        }

        // ================================
        // Caja de búsqueda (placeholder)
        // ================================
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

        // ================================
        // Links
        // ================================
        private static void OpenUrlFrom(object sender)
        {
            string url = null;

            if (sender is Button btn && btn.Tag is string t1) url = t1;
            else if (sender is Image img && img.Tag is string t2) url = t2;

            if (!string.IsNullOrWhiteSpace(url) && url != "#")
            {
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
                    MessageBox.Show($"No se pudo abrir el enlace:\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Para imágenes (MouseDown en XAML)
        private void Poster_Click(object sender, MouseButtonEventArgs e) => OpenUrlFrom(sender);

        // Para botones (Click en XAML)
        private void Poster_Click(object sender, RoutedEventArgs e) => OpenUrlFrom(sender);

        private void MusicLink_Click(object sender, RoutedEventArgs e) => OpenUrlFrom(sender);

        private void BookLink_Click(object sender, RoutedEventArgs e) => OpenUrlFrom(sender)
        ;
    }
}
