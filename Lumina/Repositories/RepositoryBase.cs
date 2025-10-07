using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Repositories
{
    internal class RepositoryBase
    {
        private readonly string _connectionString;

        public RepositoryBase()
        {
            _connectionString =
                "Server = LAPTOP-8IR410OL\\NET;" +
                "Database = DB_EjemploEsco; " +
                "Integrated Security = true";

        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
