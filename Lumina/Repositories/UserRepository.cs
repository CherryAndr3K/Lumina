using Lumina.Model;
using System;
using System.Data.SqlClient;
using System.Net;

namespace Lumina.Repositories
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        // LOGIN
        public bool AutenticateUser(NetworkCredential credential)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                // Verifica si existe un usuario con ese nombre y contraseña
                command.CommandText = "SELECT COUNT(*) FROM Usuarios WHERE Nombre = @nombre AND Contrasena = @contrasena";
                command.Parameters.AddWithValue("@nombre", credential.UserName);
                command.Parameters.AddWithValue("@contrasena", credential.Password);

                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        // REGISTRO
        public void Add(UserModel userModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;

                // Inserta un nuevo usuario (UsuarioID es autoincremental en la BD)
                command.CommandText = @"INSERT INTO Usuarios (Nombre, Contrasena, Avatar) 
                                        VALUES (@nombre, @contrasena, @avatar)";

                command.Parameters.AddWithValue("@nombre", userModel.Nombre);
                command.Parameters.AddWithValue("@contrasena", userModel.Constrasena);
                command.Parameters.AddWithValue("@avatar", (object?)userModel.Avatar ?? DBNull.Value);

                command.ExecuteNonQuery();
            }
        }

        // Obtener usuario por nombre
        public UserModel GetByUsername(string nombre)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT UsuarioID, Nombre, Contrasena, Avatar FROM Usuarios WHERE Nombre = @nombre";
                command.Parameters.AddWithValue("@nombre", nombre);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UserModel
                        {
                            UsuarioID = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Constrasena = reader.GetString(2),
                            Avatar = reader.IsDBNull(3) ? null : reader.GetString(3)
                        };
                    }
                }
            }
            return null;
        }

        // Métodos pendientes 
        public void Delete(string nombre)
        {
            throw new NotImplementedException();
        }

        public void Update(UserModel userModel)
        {
            throw new NotImplementedException();
        }
    }
}