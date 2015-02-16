using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace SquaredInfinity.Foundation.Data.UnitTests.Common.Sql
{
    public partial class SqlDataAccessServiceTests
    {
        [TestMethod]
        public void Execute()
        {
            var dac = GetDataAccessService();

            var n1 = (IReadOnlyList<int>) null;
            var n2 = (IReadOnlyList<int>) null;
            var n3 = (IReadOnlyList<int>)null;
            var n4 = (IReadOnlyList<int>) null;

            dac.Execute((c) =>
                {
                    using (var cmd = c.CreateCommand())
                    {
                        cmd.CommandText = "dbo.MultipleResultSets";
                        cmd.CommandType = CommandType.StoredProcedure;

                        using(var reader = cmd.ExecuteReader())
                        {
                            n1 =  reader.ReadAll(x => (int) x.GetValue(0));

                            reader.NextResult();

                            n2 = reader.ReadAll(x => (int)x.GetValue(0));

                            reader.NextResult();

                            n3 = reader.ReadAll(x => (int)x.GetValue(0));

                            reader.NextResult();

                            n4 = reader.ReadAll(x => (int)x.GetValue(0));
                        }
                    }
                });

            Assert.IsTrue(n1.SequenceEqual(new int[] { 1, 2, 3, 4 }));
            Assert.IsTrue(n2.SequenceEqual(new int[] { 5, 6, 7, 8 }));
            Assert.IsTrue(n3.SequenceEqual(new int[] { 9, 10, 11, 12 }));
            Assert.IsTrue(n4.SequenceEqual(new int[] { 13 }));
        }
    }
}
