using Lumina.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;


namespace Lumina.Repositories
{ 
    public class FavoritoRepository : RepositoryBase, IFavoritoRepository
    {
        public IEnumerable<FavoritoView> GetAllViews()
        {
            var list = new List<FavoritoView>();
            using var cn = GetConnection();
            using var cmd = new SqlCommand(@"
SELECT f.FavoritoID, f.UsuarioID, u.Nombre AS UsuarioNombre, f.Tipo, f.ReferenciaID,
       COALESCE(a.Titulo, l.Titulo, p.Titulo) AS ReferenciaTitulo, f.FechaMarcado
FROM dbo.Favoritos f
JOIN dbo.Usuarios u ON u.UsuarioID = f.UsuarioID
LEFT JOIN dbo.Albumes   a ON f.Tipo='Album'    AND a.AlbumesId  = f.ReferenciaId
LEFT JOIN dbo.Libros    l ON f.Tipo='Libro'    AND l.LibroId    = f.ReferenciaId
LEFT JOIN dbo.Peliculas p ON f.Tipo='Pelicula' AND p.PeliculaId = f.ReferenciaId
ORDER BY f.FavoritoID DESC;", cn);

            cn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new FavoritoView
                {
                    FavoritoId = rd.GetInt32(0),
                    UsuarioId = rd.GetInt32(1),
                    UsuarioNombre = rd.GetString(2),
                    Tipo = rd.GetString(3),
                    ReferenciaId = rd.GetInt32(4),
                    ReferenciaTitulo = rd.IsDBNull(5) ? "" : rd.GetString(5),
                    FechaMarcado = rd.GetDateTime(6)
                });
            }
            return list;
        }

        public int Add(Favorito fav)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.Favoritos (UsuarioID, Tipo, ReferenciaID, FechaMarcado)
OUTPUT INSERTED.FavoritoID
VALUES (@u, @t, @r, SYSUTCDATETIME());", cn);

            cmd.Parameters.AddWithValue("@u", fav.UsuarioId);
            cmd.Parameters.AddWithValue("@t", fav.Tipo);
            cmd.Parameters.AddWithValue("@r", fav.ReferenciaId);

            cn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public bool Delete(int id)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand("DELETE FROM dbo.Favoritos WHERE FavoritoID=@id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cn.Open();
            return cmd.ExecuteNonQuery() == 1;
        }

        // 🔥 Nuevo método: eliminar favorito por usuario/tipo/referencia
        public bool DeleteFavorito(int usuarioId, string tipo, int referenciaId)
        {
            using var cn = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            using var cmd = new SqlCommand(
                "DELETE FROM Favoritos WHERE UsuarioId = @u AND Tipo = @t AND ReferenciaId = @r", cn);

            cmd.Parameters.AddWithValue("@u", usuarioId);
            cmd.Parameters.AddWithValue("@t", tipo);
            cmd.Parameters.AddWithValue("@r", referenciaId);

            cn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

    }
}

