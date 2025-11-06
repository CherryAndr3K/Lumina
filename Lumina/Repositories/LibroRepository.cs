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
    public class LibroRepository : RepositoryBase, ILibroRepository
    {
        public IEnumerable<Libro> GetAll()
        {
            var list = new List<Libro>();
            using var cn = GetConnection();
            using var cmd = new SqlCommand("SELECT LibroID, Titulo, Autor, Genero FROM dbo.Libros ORDER BY LibroID DESC", cn);
            cn.Open();
            using var rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                list.Add(new Libro
                {
                    LibroId = rd.GetInt32(0),
                    Titulo = rd.GetString(1),
                    Autor = rd.IsDBNull(2) ? null : rd.GetString(2),
                    Genero = rd.IsDBNull(3) ? null : rd.GetString(3)
                });
            }
            return list;
        }

        public Libro? GetById(int id)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand("SELECT LibroID, Titulo, Autor, Genero FROM dbo.Libros WHERE LibroID=@id", cn);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
            cn.Open();
            using var rd = cmd.ExecuteReader();
            if (!rd.Read()) return null;
            return new Libro
            {
                LibroId = rd.GetInt32(0),
                Titulo = rd.GetString(1),
                Autor = rd.IsDBNull(2) ? null : rd.GetString(2),
                Genero = rd.IsDBNull(3) ? null : rd.GetString(3)
            };
        }

        public int Add(Libro libro)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand(
                "INSERT INTO dbo.Libros (Titulo, Autor, Genero) OUTPUT INSERTED.LibroID VALUES (@t, @a, @g);", cn);
            cmd.Parameters.AddWithValue("@t", libro.Titulo);
            cmd.Parameters.AddWithValue("@a", (object?)libro.Autor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@g", (object?)libro.Genero ?? DBNull.Value);
            cn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public bool Update(Libro libro)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand(
                "UPDATE dbo.Libros SET Titulo=@t, Autor=@a, Genero=@g WHERE LibroID=@id;", cn);
            cmd.Parameters.AddWithValue("@t", libro.Titulo);
            cmd.Parameters.AddWithValue("@a", (object?)libro.Autor ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@g", (object?)libro.Genero ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@id", libro.LibroId);
            cn.Open();
            return cmd.ExecuteNonQuery() == 1;
        }

        public bool Delete(int id)
        {
            using var cn = GetConnection();
            using var cmd = new SqlCommand("DELETE FROM dbo.Libros WHERE LibroID=@id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cn.Open();
            return cmd.ExecuteNonQuery() == 1;
        }
    }
}
