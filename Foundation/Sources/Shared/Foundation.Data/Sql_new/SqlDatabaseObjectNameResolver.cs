using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Data.Sql
{
    public class SqlDatabaseObjectNameResolver : IDatabaseObjectNameResolver
    {
        SqlDataAccessService _dataAccessService;
        protected SqlDataAccessService DataAccessService
        {
            get { return _dataAccessService; }
            private set { _dataAccessService = value; }
        }

        public SqlDatabaseObjectNameResolver(SqlDataAccessService dataAccessService)
        {
            this.DataAccessService = dataAccessService;
        }

        public virtual string GetActualStoredProcedureOrFunctionName(string storedProcedureOrFunctionName)
        {
            return storedProcedureOrFunctionName;
        }
    }
}
