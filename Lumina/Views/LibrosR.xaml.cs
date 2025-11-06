using System;
using System.Collections.ObjectModel;
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
        private ObservableCollection<Libro> _libros = new();

        public LibrosR()
        {
            InitializeComponent();
            Loaded += (_, __) => CargarLibros();
        }

        private void CargarLibros()
        {
            var data = _libroRepo.GetAll().ToList();
            _libros = new ObservableCollection<Libro>(data);
            dgLibros.ItemsSource = _libros;
        }

        private void LimpiarFormLibros()
        {
            txtLibroTitulo.Text = "";
            txtLibroAutor.Text = "";
            txtLibroGenero.Text = "";
            dgLibros.SelectedItem = null;
        }

        private Libro? GetLibroSeleccionado() => dgLibros.SelectedItem as Libro;

        private void DgLibros_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GetLibroSeleccionado() is Libro l)
            {
                txtLibroTitulo.Text = l.Titulo;
                txtLibroAutor.Text = l.Autor;
                txtLibroGenero.Text = l.Genero;
            }
        }

        private void BtnLibroAgregar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLibroTitulo.Text))
            {
                MessageBox.Show("El título es obligatorio.");
                return;
            }

            var nuevo = new Libro
            {
                Titulo = txtLibroTitulo.Text.Trim(),
                Autor = string.IsNullOrWhiteSpace(txtLibroAutor.Text) ? null : txtLibroAutor.Text.Trim(),
                Genero = string.IsNullOrWhiteSpace(txtLibroGenero.Text) ? null : txtLibroGenero.Text.Trim()
            };

            var id = _libroRepo.Add(nuevo);
            nuevo.LibroId = id;
            _libros.Insert(0, nuevo);
            LimpiarFormLibros();
        }

        private void BtnLibroModificar_Click(object sender, RoutedEventArgs e)
        {
            var sel = GetLibroSeleccionado();
            if (sel == null)
            {
                MessageBox.Show("Selecciona un libro en la tabla.");
                return;
            }

            sel.Titulo = txtLibroTitulo.Text.Trim();
            sel.Autor = string.IsNullOrWhiteSpace(txtLibroAutor.Text) ? null : txtLibroAutor.Text.Trim();
            sel.Genero = string.IsNullOrWhiteSpace(txtLibroGenero.Text) ? null : txtLibroGenero.Text.Trim();

            if (_libroRepo.Update(sel))
            {
                CargarLibros();
                MessageBox.Show("Libro actualizado.");
                LimpiarFormLibros();
            }
            else
            {
                MessageBox.Show("No se pudo actualizar.");
            }
        }

        private void BtnLibroEliminar_Click(object sender, RoutedEventArgs e)
        {
            var sel = GetLibroSeleccionado();
            if (sel == null)
            {
                MessageBox.Show("Selecciona un libro a eliminar.");
                return;
            }

            if (MessageBox.Show($"¿Eliminar \"{sel.Titulo}\"?", "Confirmar",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (_libroRepo.Delete(sel.LibroId))
                {
                    _libros.Remove(sel);
                    LimpiarFormLibros();
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar.");
                }
            }
        }
    }
}
