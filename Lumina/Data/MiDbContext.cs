using Lumina.Models;
using Microsoft.EntityFrameworkCore;

namespace Lumina.Data
{
    public class MiDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Favorito> Favoritos { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<Albume> Albumes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=BALKIRIA\VSGESTION;Database=MediaAppDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relacion Usuario - Favoritos
            modelBuilder.Entity<Favorito>()
                .HasOne(f => f.Usuario)
                .WithMany(u => u.Favoritos)
                .HasForeignKey(f => f.UsuarioId);
        }
    }
}
