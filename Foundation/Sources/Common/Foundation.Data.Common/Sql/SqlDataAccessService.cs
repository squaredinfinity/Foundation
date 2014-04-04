using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Data.Sql
{
    public class SqlDataAccessService : DataAccessService<SqlConnection, SqlCommand, SqlParameter, SqlDataReader>
    {
        public SqlDataAccessService(ConnectionFactory<SqlConnection> connectionFactory, TimeSpan defaultCommandTimeout)
            : base(connectionFactory, defaultCommandTimeout)
        { }

        public SqlDataAccessService(string connectionString, TimeSpan defaultCommandTimeout)
            : this(new ConnectionFactory<SqlConnection>(connectionString), defaultCommandTimeout)
        {}
    }
}
