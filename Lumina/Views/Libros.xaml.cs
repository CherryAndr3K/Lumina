using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Lumina.Infra;
using Lumina.Models;
using Lumina.Repositories;


namespace Lumina.Views
{
    public partial class Libros : Window
    {
        private const string PlaceholderText = "Buscar...";
        private bool _placeholderActive = true;
        public Libros()
        {
            InitializeComponent();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try { this.DragMove(); } catch { }
            }
        }


        // ================================
        // Ventana (ahora métodos directos de Window)
        // ================================
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = (this.WindowState == WindowState.Maximized)
                              ? WindowState.Normal
                              : WindowState.Maximized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
            //No hacer nada estamos en ventana de libros
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
            var favoritosWindow = new Favoritos();
            favoritosWindow.Owner = this;
            favoritosWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            favoritosWindow.Show();
            this.Hide();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new Login();
            loginWindow.Owner = this;
            loginWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            loginWindow.Show();
            this.Hide();
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

        private string Cnn() => ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private int? ResolveReferenciaId_LibroPorTitulo(string titulo)
        {
            using var cn = new SqlConnection(Cnn());
            using var cmd = new SqlCommand("SELECT TOP 1 LibroID FROM dbo.Libros WHERE Titulo=@t", cn);
            cmd.Parameters.AddWithValue("@t", titulo);
            cn.Open();
            var o = cmd.ExecuteScalar();
            return o == null ? (int?)null : Convert.ToInt32(o);
        }

        private bool ExisteFavorito(int userId, string tipo, int referenciaId)
        {
            using var cn = new SqlConnection(Cnn());
            using var cmd = new SqlCommand("SELECT 1 FROM dbo.Favoritos WHERE UsuarioID=@u AND Tipo=@t AND ReferenciaID=@r", cn);
            cmd.Parameters.AddWithValue("@u", userId);
            cmd.Parameters.AddWithValue("@t", tipo);
            cmd.Parameters.AddWithValue("@r", referenciaId);
            cn.Open();
            using var rd = cmd.ExecuteReader();
            return rd.Read();
        }

        private void SetStarIcon(Button btn, bool isFav)
        {
            // Usa las rutas REALES del proyecto
            var path = isFav
                ? "/Images/Iconos/star_filled.png"
                : "/Images/Iconos/star_outline.png";

            // Pack URI más robusto para recursos embebidos
            var uri = new Uri($"pack://application:,,,{path}", UriKind.Absolute);

            try
            {
                var bmp = new BitmapImage(uri);
                if (btn.Content is Image img)
                {
                    img.Source = bmp;
                }
                else
                {
                    btn.Content = new Image { Source = bmp, Width = 18, Height = 18 };
                }
            }
            catch
            {
                // Si por cualquier cosa no encuentra la imagen, evita que crashee la ventana
            }
        }

        private void Star_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                var (tipo, titulo, _) = ParseTag(tag);  // "Book"
                if (!string.Equals(tipo, "Book", StringComparison.OrdinalIgnoreCase)) return;

                var id = ResolveReferenciaId_LibroPorTitulo(titulo);
                var isFav = (id.HasValue && ExisteFavorito(AppSession.CurrentUserId, "Libro", id.Value));
                SetStarIcon(btn, isFav);
            }
        }

        private void Star_Toggle_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                var (tipo, titulo, _) = ParseTag(tag);
                if (!string.Equals(tipo, "Book", StringComparison.OrdinalIgnoreCase)) return;

                var id = ResolveReferenciaId_LibroPorTitulo(titulo);
                if (!id.HasValue) { MessageBox.Show("No se encontró el libro en la BD."); return; }

                var isFav = ExisteFavorito(AppSession.CurrentUserId, "Libro", id.Value);
                if (isFav)
                {
                    MessageBox.Show("Ya está en Favoritos.");
                    SetStarIcon(btn, true);
                    return;
                }

                _ = _favRepo.Add(new Favorito
                {
                    UsuarioId = AppSession.CurrentUserId,
                    Tipo = "Libro",
                    ReferenciaId = id.Value
                });

                SetStarIcon(btn, true);
                MessageBox.Show($"Añadido a Favoritos: {titulo}");
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
                catch (Exception ex)
                {
                    MessageBox.Show($"No se pudo abrir el enlace:\n{ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
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