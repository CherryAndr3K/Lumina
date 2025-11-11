using Lumina.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Lumina.Data
{
    public class MiDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Favorito> Favoritos { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Albumes> Albumes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Leer la cadena de conexión desde App.config
                var connectionString = System.Configuration.ConfigurationManager
                    .ConnectionStrings["DefaultConnection"].ConnectionString;

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relación Usuario - Favoritos
            modelBuilder.Entity<Favorito>()
                .HasOne(f => f.Usuario)
                .WithMany(u => u.Favoritos)
                .HasForeignKey(f => f.UsuarioId);
        }
    }
}
