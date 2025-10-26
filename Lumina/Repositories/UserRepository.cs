using Lumina.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


//Se nececitar ajustar segun lo que lleva la base de datos
namespace Lumina.Repositories
{ 
    public class UserRepository : RepositoryBase, IUserRepository
    {

    public bool AutenticateUser(NetworkCredential credential)
    {
        bool validUser;
        using (var connection = GetConnection())
        using (var command = new SqlCommand())
        {
            connection.Open();
            command.Connection = connection;
            command.CommandText = "select * from ¨[User] where username = @username and [password]=@password";
            command.Parameters.Add("@username", System.Data.SqlDbType.NVarChar).Value = credential.UserName;
            command.Parameters.Add("@password", System.Data.SqlDbType.NVarChar).Value = credential.Password;
            validUser = command.ExecuteScalar() == null ? false : true;

            return validUser;
        }
    }

    public void Delete(string username)
    {
        throw new NotImplementedException();
    }

    public UserModel GetByUsername(string username)
    {
        throw new NotImplementedException();
    }

    public void Update(UserModel useModel)
    {
        throw new NotImplementedException();
    }

    public void Add(UserModel userModel)
    {
        using (var connection = GetConnection())
        using (var command = new SqlCommand())
        {
            connection.Open();
            command.Connection = connection;
            command.CommandText = "INSERT INTO [User] VALUES (@username +" +
                " @password, @name, @lastname, @email)";
            command.Parameters.AddWithValue("@Id", userModel.Id);
            command.Parameters.AddWithValue("@username", userModel.Username);
            command.Parameters.AddWithValue("@password", userModel.Password);
            command.Parameters.AddWithValue("@email", userModel.Email);
            command.ExecuteNonQuery();
            connection.Close();

        }
    }
}
}