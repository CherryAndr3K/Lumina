using Lumina.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Lumina.Models;

namespace Lumina.Repositories
{
    public class AlbumRepository : RepositoryBase, IAlbumRepository
    {
        public IEnumerable<Albume> GetAll()
        {
            var list = new List<Albume>();
            using var cn = GetConnection();
            using var cmd = new SqlCommand("SELECT AlbumID, Titulo, Artista, Genero FROM dbo.Albumes ORDER BY AlbumID DESC", cn);
            cn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new Albume
                {
                    AlbumId = rd.GetInt32(0),
                    Titulo = rd.GetString(1),
                    Artista = rd.IsDBNull(2) ? null : rd.GetString(2),
                    Genero = rd.IsDBNull(3) ? null : rd.GetString(3)
                });
            }
            return list;
        }

        public Albume? GetById(int id)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand("SELECT AlbumID, Titulo, Artista, Genero FROM dbo.Albumes WHERE AlbumID = @id", cn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
            cn.Open();
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return new Albume
            {
                AlbumId = rd.GetInt32(0),
                Titulo = rd.GetString(1),
                Artista = rd.IsDBNull(2) ? null : rd.GetString(2),
                Genero = rd.IsDBNull(3) ? null : rd.GetString(3)
            };
        }

        public int Add(Albume album)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand(
                "INSERT INTO dbo.Albumes (Titulo, Artista, Genero) OUTPUT INSERTED.AlbumID VALUES (@t, @a, @g);", cn);
            cmd.Parameters.AddWithValue("@t", album.Titulo);
            cmd.Parameters.AddWithValue("@a", (object?)album.Artista ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@g", (object?)album.Genero ?? DBNull.Value);
            cn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public bool Update(Albume album)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand(
                "UPDATE dbo.Albumes SET Titulo=@t, Artista=@a, Genero=@g WHERE AlbumID=@id;", cn);
            cmd.Parameters.AddWithValue("@t", album.Titulo);
            cmd.Parameters.AddWithValue("@a", (object?)album.Artista ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@g", (object?)album.Genero ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", album.AlbumId);
            cn.Open();
            return cmd.ExecuteNonQuery() == 1;
        }

        public bool Delete(int id)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand("DELETE FROM dbo.Albumes WHERE AlbumID=@id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cn.Open();
            return cmd.ExecuteNonQuery() == 1;
        }

    }
}
