using System;

namespace Lumina.Models
{
    // Modelo auxiliar para mostrar los favoritos con nombres legibles
    public class FavoritoView
    {
        public int FavoritoId { get; set; }

        public int UsuarioId { get; set; }

        public string UsuarioNombre { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty; // "Album" | "Libro" | "Pelicula"

        public int ReferenciaId { get; set; }

        public string ReferenciaTitulo { get; set; } = string.Empty; // título de álbum/libro/película

        public DateTime FechaMarcado { get; set; }
    }
}
