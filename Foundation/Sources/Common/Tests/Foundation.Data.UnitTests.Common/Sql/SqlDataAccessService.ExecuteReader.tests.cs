using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using System.Data.SqlClient;
using System.Data;
using Microsoft.SqlServer.Server;

namespace SquaredInfinity.Foundation.Data.UnitTests.Common.Sql
{
    public partial class SqlDataAccessServiceTests
    {
        [TestMethod]
        public void ExecuteReader_SimpleTableTypeInput()
        {
            var dac = GetDataAccessService();

            var numbers = new int[] { 1, 2, 3, 4, 5, 6 };

            var data = dac.ExecuteReader("dbo.SimpleTableTypeInput",
                new[] { dac.CreateTableParameter("input", source:numbers, columnName:"number", columnType:SqlDbType.Int) })
                .ToArray();

            Assert.AreEqual(numbers.Length, data.Length);
        }


        //[TestMethod]
        //public void ExecuteReader_ComplexTableTypeInput()
        //{
        //    var dac = GetDataAccessService();

        //    var numbers = new int[] { 1, 2, 3, 4, 5, 6 };

        //    var data = dac.ExecuteReader("dbo.ComplexTableTypeInput",
        //        new[] { dac.CreateTableParameter("input", source: numbers, (x) =>
        //        {
        //            var dbr = new SqlDataRecord( )
        //        })
        //        .ToArray();

        //    Assert.AreEqual(numbers.Length, data.Length);
        //}
    }
}
