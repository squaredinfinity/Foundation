using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Data
{
    public class ConnectionFactory<TConnection>
        where TConnection : DbConnection, new()
    {
        readonly string ConnectionString;

        public ConnectionFactory(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public virtual TConnection GetNewConnection()
        {
            var connection = new TConnection();
            
            connection.ConnectionString = ConnectionString;

            return connection;
        }

        public override string ToString()
        {
            return ConnectionString.ToString(valueWhenNull: "NULL");
        }
    }
}
