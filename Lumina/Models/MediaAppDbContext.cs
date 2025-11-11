using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Lumina.Models;

public partial class MediaAppDbContext : DbContext
{
    public MediaAppDbContext()
    {
    }

    public MediaAppDbContext(DbContextOptions<MediaAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Albumes> Albumes { get; set; }

    public virtual DbSet<Favorito> Favoritos { get; set; }

    public virtual DbSet<Libro> Libros { get; set; }

    public virtual DbSet<Pelicula> Peliculas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=BALKIRIA\\VSGESTION;Database=MediaAppDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Albumes>(entity =>
        {
            entity.HasKey(e => e.AlbumesId).HasName("PK__Albumes__97B4BE17E6E4BEBB");

            entity.HasIndex(e => e.Titulo, "IX_Albumes_Titulo");

            entity.Property(e => e.AlbumesId).HasColumnName("AlbumID");
            entity.Property(e => e.Artista).HasMaxLength(150);
            entity.Property(e => e.Genero).HasMaxLength(100);
            entity.Property(e => e.Titulo).HasMaxLength(200);
        });

        modelBuilder.Entity<Favorito>(entity =>
        {
            entity.HasKey(e => e.FavoritoId).HasName("PK__Favorito__CFF71185BB49ED04");

            entity.HasIndex(e => new { e.UsuarioId, e.Tipo }, "IX_Favoritos_UsuarioTipo");

            entity.Property(e => e.FavoritoId).HasColumnName("FavoritoID");
            entity.Property(e => e.FechaMarcado).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ReferenciaId).HasColumnName("ReferenciaID");
            entity.Property(e => e.Tipo).HasMaxLength(50);
            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Favoritos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Favoritos_Usuarios");
        });

        modelBuilder.Entity<Libro>(entity =>
        {
            entity.HasKey(e => e.LibroId).HasName("PK__Libros__35A1EC8D94AF9D72");

            entity.HasIndex(e => e.Titulo, "IX_Libros_Titulo");

            entity.Property(e => e.LibroId).HasColumnName("LibroID");
            entity.Property(e => e.Autor).HasMaxLength(150);
            entity.Property(e => e.Genero).HasMaxLength(100);
            entity.Property(e => e.Titulo).HasMaxLength(200);
        });

        modelBuilder.Entity<Pelicula>(entity =>
        {
            entity.HasKey(e => e.PeliculaId).HasName("PK__Pelicula__5AC6F32C5ECC0910");

            entity.HasIndex(e => e.Titulo, "IX_Peliculas_Titulo");

            entity.Property(e => e.PeliculaId).HasColumnName("PeliculaID");
            entity.Property(e => e.Genero).HasMaxLength(100);
            entity.Property(e => e.Titulo).HasMaxLength(200);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.UsuarioId).HasName("PK__Usuarios__2B3DE7987CA4FB5B");

            entity.Property(e => e.UsuarioId).HasColumnName("UsuarioID");
            entity.Property(e => e.Avatar).HasMaxLength(255);
            entity.Property(e => e.Contrasena).HasMaxLength(255);
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .HasDefaultValue("sin_correo");
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
