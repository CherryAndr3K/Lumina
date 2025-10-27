using System.Configuration;
using System.Data.SqlClient;

namespace Lumina.Repositories
{
    public class RepositoryBase
    {
        private readonly string _connectionString;

        public RepositoryBase()
        {
            _connectionString = ConfigurationManager
                .ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}

