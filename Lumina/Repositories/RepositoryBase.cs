using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumina.Repositories
{
    public abstract class RepositoryBase
    {
        private readonly string _connectionString;

        public RepositoryBase()
        {
            _connectionString =
                "Server = LAPTOP-8IR410OL\\NET;" +
                "Database = DB_EjemploEsco; " +
                "Integrated Security = true";  //Cambiar esto para cuando se cheque porfis

        }

        protected SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
