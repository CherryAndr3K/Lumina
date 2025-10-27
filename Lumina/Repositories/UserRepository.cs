using Lumina.Model;
using System;
using System.Data.SqlClient;
using System.Net;

namespace Lumina.Repositories
{
    // Ajusta los nombres de tabla/columnas según tu BD real.
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public bool AutenticateUser(NetworkCredential credential)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(1) FROM [User] WHERE Username = @username AND [Password] = @password";
                command.Parameters.AddWithValue("@username", credential.UserName);
                command.Parameters.AddWithValue("@password", credential.Password);
                var result = command.ExecuteScalar();
                int count = (result is int) ? (int)result : Convert.ToInt32(result);
                return count > 0;
            }
        }

        public void Add(UserModel userModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "INSERT INTO [User](Id, Username, [Password], Email) VALUES (@Id, @username, @password, @email)";
                command.Parameters.AddWithValue("@Id", string.IsNullOrWhiteSpace(userModel.Id) ? Guid.NewGuid().ToString() : userModel.Id);
                command.Parameters.AddWithValue("@username", userModel.Username);
                command.Parameters.AddWithValue("@password", userModel.Password);
                command.Parameters.AddWithValue("@email", (object?)userModel.Email ?? DBNull.Value);
                command.ExecuteNonQuery();
            }
        }

        public void Update(UserModel userModel)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "UPDATE [User] SET [Password] = @password, Email = @email WHERE Username = @username";
                command.Parameters.AddWithValue("@username", userModel.Username);
                command.Parameters.AddWithValue("@password", userModel.Password);
                command.Parameters.AddWithValue("@email", (object?)userModel.Email ?? DBNull.Value);
                command.ExecuteNonQuery();
            }
        }

        public void Delete(string username)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "DELETE FROM [User] WHERE Username = @username";
                command.Parameters.AddWithValue("@username", username);
                command.ExecuteNonQuery();
            }
        }

        public UserModel? GetByUsername(string username)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT TOP 1 Id, Username, [Password], Email FROM [User] WHERE Username = @username";
                command.Parameters.AddWithValue("@username", username);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UserModel
                        {
                            Id = reader["Id"]?.ToString(),
                            Username = reader["Username"]?.ToString(),
                            Password = reader["Password"]?.ToString(),
                            Email = reader["Email"]?.ToString()
                        };
                    }
                }
            }
            return null;
        }
    }
}
