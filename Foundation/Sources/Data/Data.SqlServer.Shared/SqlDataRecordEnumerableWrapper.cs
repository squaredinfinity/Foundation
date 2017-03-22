using Microsoft.SqlServer.Server;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Data.SqlServer
{
    internal class SqlDataRecordEnumerableWrapper : IEnumerable<SqlDataRecord>
    {
        IEnumerable Source;
        SqlMetaData[] Metadata;

        public SqlDataRecordEnumerableWrapper(IEnumerable source, SqlMetaData[] md)
        {
            Source = source;
            Metadata = md;
        }

        public IEnumerator<SqlDataRecord> GetEnumerator()
        {
            foreach (var x in Source)
            {
                var r = new SqlDataRecord(Metadata[0]);
                r.SetValue(0, x);

                yield return r;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var x in Source)
            {
                var r = new SqlDataRecord(Metadata[0]);
                r.SetValue(0, x);

                yield return r;
            }
        }
    }
}
