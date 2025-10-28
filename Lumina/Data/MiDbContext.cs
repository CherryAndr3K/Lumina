using Lumina.Model;
using Microsoft.EntityFrameworkCore;

namespace Lumina.Data
{
    public class MiDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    @"Server=BALKIRIA\VSGESTION;Database=MediaAppDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }
    }
}
