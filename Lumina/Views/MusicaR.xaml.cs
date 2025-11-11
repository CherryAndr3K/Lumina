using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Lumina.Models;
using Lumina.Repositories;

namespace Lumina.Views
{
    public partial class MusicaR : Window
    {
        private readonly IAlbumRepository _albumRepo = new AlbumRepository();
        private ObservableCollection<Albumes> _albumes = new();

        public MusicaR()
        {
            InitializeComponent();
            Loaded += (_, __) => CargarAlbumes();
        }

        private void CargarAlbumes()
        {
            var data = _albumRepo.GetAll().ToList();
            _albumes = new ObservableCollection<Albumes>(data);
            dgAlbumes.ItemsSource = _albumes;
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            var w = new Musica();
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            w.Show();

            this.Close(); // Cierra solo esta ventana, sin cerrar la nueva
        }

        private void LimpiarFormAlbumes()
        {
            txtAlbumTitulo.Text = "";
            txtAlbumArtista.Text = "";
            txtAlbumGenero.Text = "";
            dgAlbumes.SelectedItem = null;
        }

        private Albumes? GetAlbumSel() => dgAlbumes.SelectedItem as Albumes;

        private void DgAlbumes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GetAlbumSel() is Albumes a)
            {
                txtAlbumTitulo.Text = a.Titulo;
                txtAlbumArtista.Text = a.Artista;
                txtAlbumGenero.Text = a.Genero;
            }
        }

        private void BtnAlbumAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAlbumTitulo.Text))
            {
                MessageBox.Show("El título es obligatorio.");
                return;
            }

            var nuevo = new Albumes
            {
                Titulo = txtAlbumTitulo.Text.Trim(),
                Artista = string.IsNullOrWhiteSpace(txtAlbumArtista.Text) ? null : txtAlbumArtista.Text.Trim(),
                Genero = string.IsNullOrWhiteSpace(txtAlbumGenero.Text) ? null : txtAlbumGenero.Text.Trim()
            };

            var id = _albumRepo.Add(nuevo);
            nuevo.AlbumesId = id;
            _albumes.Insert(0, nuevo);
            LimpiarFormAlbumes();
        }

        private void BtnAlbumModificar_Click(object sender, RoutedEventArgs e)
        {
            var sel = GetAlbumSel();
            if (sel == null) { MessageBox.Show("Selecciona un álbum."); return; }

            sel.Titulo = txtAlbumTitulo.Text.Trim();
            sel.Artista = string.IsNullOrWhiteSpace(txtAlbumArtista.Text) ? null : txtAlbumArtista.Text.Trim();
            sel.Genero = string.IsNullOrWhiteSpace(txtAlbumGenero.Text) ? null : txtAlbumGenero.Text.Trim();

            if (_albumRepo.Update(sel))
            {
                CargarAlbumes();
                MessageBox.Show("Álbum actualizado.");
                LimpiarFormAlbumes();
            }
            else MessageBox.Show("No se pudo actualizar.");
        }

        private void BtnAlbumEliminar_Click(object sender, RoutedEventArgs e)
        {
            var sel = GetAlbumSel();
            if (sel == null) { MessageBox.Show("Selecciona un álbum."); return; }

            if (MessageBox.Show($"¿Eliminar \"{sel.Titulo}\"?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (_albumRepo.Delete(sel.AlbumesId))
                {
                    _albumes.Remove(sel);
                    LimpiarFormAlbumes();
                }
                else MessageBox.Show("No se pudo eliminar.");
            }
        }
    }
}
