using Lumina.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lumina.Views
{
    public partial class AdminUsuarios : Window
    {
        private MediaAppDbContext _context;

        public AdminUsuarios()
        {
            InitializeComponent();
            _context = new MediaAppDbContext();
            dgUsuarios.ItemsSource = _context.Usuarios.ToList();
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            _context.SaveChanges();
            MessageBox.Show("Cambios guardados correctamente");
        }

        private void Crear_Click(object sender, RoutedEventArgs e)
        {
            var nuevoUsuario = new Usuario
            {
                Nombre = "Nuevo Usuario",
                Correo = "nuevo@correo.com",
                Contrasena = "1234"
            };

            _context.Usuarios.Add(nuevoUsuario);
            _context.SaveChanges();

            dgUsuarios.ItemsSource = _context.Usuarios.ToList();
            MessageBox.Show("Usuario creado correctamente");
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (dgUsuarios.SelectedItem is Usuario usuarioSeleccionado)
            {
                _context.Usuarios.Remove(usuarioSeleccionado);
                _context.SaveChanges();

                dgUsuarios.ItemsSource = _context.Usuarios.ToList();
                MessageBox.Show("Usuario eliminado correctamente");
            }
            else
            {
                MessageBox.Show("Selecciona un usuario para eliminar.");
            }
        }
    }
}


