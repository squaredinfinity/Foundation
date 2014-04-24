using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using System.Xml.Linq;
using System.Data.SqlTypes;

namespace SquaredInfinity.Foundation.Data.Sql
{
    public class SqlDataAccessService : DataAccessService<SqlConnection, SqlCommand, SqlParameter, SqlDataReader>
    {
        public SqlDataAccessService(string connectionString, TimeSpan defaultCommandTimeout)
            : this(new ConnectionFactory<SqlConnection>(connectionString), defaultCommandTimeout)
        { }

        public SqlDataAccessService(ConnectionFactory<SqlConnection> connectionFactory, TimeSpan defaultCommandTimeout)
            : base(connectionFactory, defaultCommandTimeout)
        {
            RetryPolicy = new RetryPolicy();
            RetryPolicy.DefaultTransientFaultFilters.Add(new SqlTransientFaultFilter());
        }

        public override SqlParameter MapToParameter(string parameterName, object clrValue)
        {
            var result = new SqlParameter();

            result.ParameterName = parameterName;

            if (clrValue == null)
            {
                result.Value = DBNull.Value;
                return result;
            }

            var is_ts = clrValue is TimeSpan;
            if (is_ts)
            {
                result.Value = ((TimeSpan) clrValue).Ticks;
                return result;
            }

            var xDocument = clrValue as XDocument;
            if (xDocument != null)
            {
                result.Value = xDocument.ToString(SaveOptions.None);
                return result;
            }

            var xElement = clrValue as XElement;
            if (xElement != null)
            {
                result.Value = xElement.ToString(SaveOptions.None);
                return result;
            }

            result.Value = clrValue;
            return result;
        }
    }
}
