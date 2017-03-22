using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Data
{
    public class ConnectionFactory<TConnection>
        where TConnection : DbConnection, new()
    {
        string ConnectionString { get; set; }

        public ConnectionFactory(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Changes underlying connection string.
        /// All newly created connection will use new connection string.
        /// </summary>
        /// <param name="newConnectionString"></param>
        public virtual void ChangeConnectionString(string newConnectionString)
        {
            this.ConnectionString = newConnectionString;
        }

        /// <summary>
        /// Returns new connection initialized with underlying connection string.
        /// Returned connection will not be automatically open.
        /// </summary>
        /// <returns></returns>
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
