using Lumina.Data;
using Lumina.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Seeding
{
    public static class CrearLibros
    {
        public static void Inicializar()
        {
            using (var context = new MiDbContext())
            {
                if (!context.Libros.Any()) // Solo si la tabla está vacía
                {
                    var libros = new List<Libro>
            {
                new Libro { Titulo = "Abrazar el árbol de la conciencia", Autor = "Desconocido" },
                new Libro { Titulo = "La regeneración", Autor = "Hank Hiss" },
                new Libro { Titulo = "El dragón rojo", Autor = "Thomas Harris" },
                new Libro { Titulo = "Oficio de tinieblas", Autor = "Rudolfo Anaya" },
                new Libro { Titulo = "¡GRACIAS!", Autor = "Andrés Manuel López Obrador" },
                new Libro { Titulo = "The Lightning Thief", Autor = "Rick Riordan" },
                new Libro { Titulo = "El negociador", Autor = "Arturo Elías Ayub" }
            };

                    context.Libros.AddRange(libros);
                    context.SaveChanges();
                }
            }
        }
    }
}
        
