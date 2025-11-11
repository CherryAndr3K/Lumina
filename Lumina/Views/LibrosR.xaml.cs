using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Lumina.Models;
using Lumina.Repositories;

namespace Lumina.Views
{
    public partial class LibrosR : Window
    {
        private readonly ILibroRepository _libroRepo = new LibroRepository();
        private Libro? _seleccionado;

        public LibrosR()
        {
            InitializeComponent();
            CargarLibros();
        }

        private void BtnRegresar_Click(object sender, RoutedEventArgs e)
        {
            var w = new Libros();
            w.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            w.Show();

            this.Close(); // Cierra solo esta ventana, sin cerrar la nueva
        }

        private void CargarLibros()
        {
            try
            {
                IEnumerable<Libro> data = _libroRepo.GetAll();
                dgLibros.ItemsSource = data.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar libros: " + ex.Message);
            }
        }

        private void LimpiarFormulario()
        {
            txtLibroTitulo.Text = string.Empty;
            txtLibroAutor.Text = string.Empty;
            txtLibroGenero.Text = string.Empty;
            txtLibroLink.Text = string.Empty;
            txtLibroImagen.Text = string.Empty;
            _seleccionado = null;
        }

        // =================== SELECCIÓN ===================
        private void DgLibros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _seleccionado = dgLibros.SelectedItem as Libro;
            if (_seleccionado == null)
            {
                LimpiarFormulario();
                return;
            }

            txtLibroTitulo.Text = _seleccionado.Titulo;
            txtLibroAutor.Text = _seleccionado.Autor;
            txtLibroGenero.Text = _seleccionado.Genero;
            txtLibroLink.Text = _seleccionado.Link;
            txtLibroImagen.Text = _seleccionado.Imagen;
        }

        // =================== AGREGAR ===================
        private void BtnLibroAgregar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var nuevo = new Libro
                {
                    Titulo = txtLibroTitulo.Text?.Trim(),
                    Autor = txtLibroAutor.Text?.Trim(),
                    Genero = txtLibroGenero.Text?.Trim(),
                    Link = txtLibroLink.Text?.Trim(),
                    Imagen = txtLibroImagen.Text?.Trim()
                };

                _libroRepo.Add(nuevo);
                CargarLibros();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al agregar libro: " + ex.Message);
            }
        }

        // =================== MODIFICAR ===================
        private void BtnLibroModificar_Click(object sender, RoutedEventArgs e)
        {
            if (_seleccionado == null)
            {
                MessageBox.Show("Selecciona un libro primero.");
                return;
            }

            try
            {
                _seleccionado.Titulo = txtLibroTitulo.Text?.Trim();
                _seleccionado.Autor = txtLibroAutor.Text?.Trim();
                _seleccionado.Genero = txtLibroGenero.Text?.Trim();
                _seleccionado.Link = txtLibroLink.Text?.Trim();
                _seleccionado.Imagen = txtLibroImagen.Text?.Trim();

                _libroRepo.Update(_seleccionado);
                CargarLibros();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar libro: " + ex.Message);
            }
        }

        // =================== ELIMINAR ===================
        private void BtnLibroEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (_seleccionado == null)
            {
                MessageBox.Show("Selecciona un libro primero.");
                return;
            }

            if (MessageBox.Show("¿Seguro que quieres eliminar este libro?",
                                "Confirmar",
                                MessageBoxButton.YesNo,
                                MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            try
            {
                _libroRepo.Delete(_seleccionado.LibroId);
                CargarLibros();
                LimpiarFormulario();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar libro: " + ex.Message);
            }
        }
    }
}
