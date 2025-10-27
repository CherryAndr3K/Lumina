using System.Data.SqlClient;

namespace Lumina.Repositories
{
    public abstract class RepositoryBase
    {
        private readonly string _connectionString;

        public RepositoryBase()
        {
            //cadena de conexión
            _connectionString = "Data Source=balkar\\sqlgestion;Initial Catalog=MediaAppDB;Integrated Security=True;TrustServerCertificate=True";
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
