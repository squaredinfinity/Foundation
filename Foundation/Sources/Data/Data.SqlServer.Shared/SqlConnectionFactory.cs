using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace SquaredInfinity.Data.SqlServer
{
    public class SqlConnectionFactory : ConnectionFactory<SqlConnection>
    {
        public SqlConnectionFactory()
            : base("<not set>")
        { }

        string ConnectionString { get; set; }

        public override void ChangeConnectionString(string newConnectionString)
        {
            ConnectionString = newConnectionString;
        }

        public override SqlConnection GetNewConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
