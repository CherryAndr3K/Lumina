namespace Lumina.Model
{
    public class Usuario
    {
        public int Id { get; set; }          // Clave primaria
        public string Correo { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
    }
}
