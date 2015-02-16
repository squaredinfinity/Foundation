using SquaredInfinity.Foundation.Data.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Data.UnitTests.Common.Sql
{
    
    public partial class SqlDataAccessServiceTests
    {
        string ConnectionString = "Data Source=SI_SP3;Initial Catalog=Foundation.Data;Integrated Security=True";


        protected SqlDataAccessService GetDataAccessService()
        {
            return new SqlDataAccessService(ConnectionString, TimeSpan.FromSeconds(10));
        }
    }
}
