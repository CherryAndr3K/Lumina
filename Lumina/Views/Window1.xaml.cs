using System.Windows;
using System.Windows.Controls;

namespace ProyectoEjemploEsco.View
{
    public partial class RegistroView : Window
    {
        public RegistroView()
        {
            InitializeComponent();
        }

        // Minimizar ventana
        private void btnMinimizar_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Cerrar ventana
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private string realPassword = "";
        private void txtFakePassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            var box = (TextBox)sender;
            int caret = box.CaretIndex;

            // Calcular lo que se agregó o eliminó
            if (box.Text.Length < realPassword.Length)
            {
                // Borraron algo
                realPassword = realPassword.Substring(0, box.Text.Length);
            }
            else
            {
                // Agregaron texto
                var newChars = box.Text.Substring(realPassword.Length);
                realPassword += newChars;
            }

            // Reemplazar texto visible por puntos
            box.Text = new string('•', realPassword.Length);
            box.CaretIndex = caret;
        }
    }
}