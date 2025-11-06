using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Lumina.Models;
using Lumina.Repositories;

namespace Lumina.Views
{
    public partial class PeliculasR : Window
    {
        private readonly IPeliculaRepository _peliRepo = new PeliculaRepository();
        private ObservableCollection<Pelicula> _pelis = new();

        public PeliculasR()
        {
            InitializeComponent();
            Loaded += (_, __) => CargarPeliculas();
        }

        private void CargarPeliculas()
        {
            var data = _peliRepo.GetAll().ToList();
            _pelis = new ObservableCollection<Pelicula>(data);
            dgPeliculas.ItemsSource = _pelis;
        }

        private void LimpiarFormPelis()
        {
            txtPeliTitulo.Text = "";
            txtPeliGenero.Text = "";
            txtPeliAnio.Text = "";
            dgPeliculas.SelectedItem = null;
        }

        private Pelicula? GetPeliSel() => dgPeliculas.SelectedItem as Pelicula;

        private void DgPeliculas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GetPeliSel() is Pelicula p)
            {
                txtPeliTitulo.Text = p.Titulo;
                txtPeliGenero.Text = p.Genero;
                txtPeliAnio.Text = p.Anio?.ToString() ?? "";
            }
        }

        private void BtnPeliAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPeliTitulo.Text))
            {
                MessageBox.Show("El título es obligatorio.");
                return;
            }

            int? anio = null;
            if (!string.IsNullOrWhiteSpace(txtPeliAnio.Text))
            {
                if (int.TryParse(txtPeliAnio.Text.Trim(), out var a)) anio = a;
                else { MessageBox.Show("Año inválido."); return; }
            }

            var nuevo = new Pelicula
            {
                Titulo = txtPeliTitulo.Text.Trim(),
                Genero = string.IsNullOrWhiteSpace(txtPeliGenero.Text) ? null : txtPeliGenero.Text.Trim(),
                Anio = anio
            };

            var id = _peliRepo.Add(nuevo);
            nuevo.PeliculaId = id;
            _pelis.Insert(0, nuevo);
            LimpiarFormPelis();
        }

        private void BtnPeliModificar_Click(object sender, RoutedEventArgs e)
        {
            var sel = GetPeliSel();
            if (sel == null) { MessageBox.Show("Selecciona una película."); return; }

            int? anio = null;
            if (!string.IsNullOrWhiteSpace(txtPeliAnio.Text))
            {
                if (int.TryParse(txtPeliAnio.Text.Trim(), out var a)) anio = a;
                else { MessageBox.Show("Año inválido."); return; }
            }

            sel.Titulo = txtPeliTitulo.Text.Trim();
            sel.Genero = string.IsNullOrWhiteSpace(txtPeliGenero.Text) ? null : txtPeliGenero.Text.Trim();
            sel.Anio = anio;

            if (_peliRepo.Update(sel))
            {
                CargarPeliculas();
                MessageBox.Show("Película actualizada.");
                LimpiarFormPelis();
            }
            else MessageBox.Show("No se pudo actualizar.");
        }

        private void BtnPeliEliminar_Click(object sender, RoutedEventArgs e)
        {
            var sel = GetPeliSel();
            if (sel == null) { MessageBox.Show("Selecciona una película."); return; }

            if (MessageBox.Show($"¿Eliminar \"{sel.Titulo}\"?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (_peliRepo.Delete(sel.PeliculaId))
                {
                    _pelis.Remove(sel);
                    LimpiarFormPelis();
                }
                else MessageBox.Show("No se pudo eliminar.");
            }
        }
    }
}
