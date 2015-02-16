using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using System.Data.SqlClient;

namespace SquaredInfinity.Foundation.Data.UnitTests.Common.Sql
{
    [TestClass]
    public partial class SqlDataAccessServiceTests
    {
        [TestMethod]
        public void ExecuteNonQuery()
        {
            var dac = GetDataAccessService();

            dac.ExecuteNonQuery("dbo.NonQuery");
        }

        [TestMethod]
        public void ExecuteNonQuery_WithOutput()
        {
            var dac = GetDataAccessService();

            var number_param = dac.CreateOutParameter("number", System.Data.SqlDbType.Int);

            dac.ExecuteNonQuery("dbo.NonQuery_WithOutput",
                new[]
                {
                    number_param
                });

            Assert.AreEqual(13, number_param.Value.DbToClr<int>());
        }
    }
}
