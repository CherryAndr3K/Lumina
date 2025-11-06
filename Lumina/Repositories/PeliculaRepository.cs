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
    public class PeliculaRepository : RepositoryBase, IPeliculaRepository
    {
        public IEnumerable<Pelicula> GetAll()
        {
            var list = new List<Pelicula>();
            using var cn = GetConnection();
            using var cmd = new SqlCommand("SELECT PeliculaID, Titulo, Genero, Anio FROM dbo.Peliculas ORDER BY PeliculaID DESC", cn);
            cn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new Pelicula
                {
                    PeliculaId = rd.GetInt32(0),
                    Titulo = rd.GetString(1),
                    Genero = rd.IsDBNull(2) ? null : rd.GetString(2),
                    Anio = rd.IsDBNull(3) ? (int?)null : rd.GetInt32(3)
                });
            }
            return list;
        }

        public Pelicula? GetById(int id)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand("SELECT PeliculaID, Titulo, Genero, Anio FROM dbo.Peliculas WHERE PeliculaID=@id", cn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
            cn.Open();
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return new Pelicula
            {
                PeliculaId = rd.GetInt32(0),
                Titulo = rd.GetString(1),
                Genero = rd.IsDBNull(2) ? null : rd.GetString(2),
                Anio = rd.IsDBNull(3) ? (int?)null : rd.GetInt32(3)
            };
        }

        public int Add(Pelicula pelicula)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand(
                "INSERT INTO dbo.Peliculas (Titulo, Genero, Anio) OUTPUT INSERTED.PeliculaID VALUES (@t, @g, @a);", cn);
            cmd.Parameters.AddWithValue("@t", pelicula.Titulo);
            cmd.Parameters.AddWithValue("@g", (object?)pelicula.Genero ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@a", (object?)pelicula.Anio ?? DBNull.Value);
            cn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public bool Update(Pelicula pelicula)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand(
                "UPDATE dbo.Peliculas SET Titulo=@t, Genero=@g, Anio=@a WHERE PeliculaID=@id;", cn);
            cmd.Parameters.AddWithValue("@t", pelicula.Titulo);
            cmd.Parameters.AddWithValue("@g", (object?)pelicula.Genero ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@a", (object?)pelicula.Anio ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", pelicula.PeliculaId);
            cn.Open();
            return cmd.ExecuteNonQuery() == 1;
        }

        public bool Delete(int id)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand("DELETE FROM dbo.Peliculas WHERE PeliculaID=@id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cn.Open();
            return cmd.ExecuteNonQuery() == 1;
        }
    }
}
