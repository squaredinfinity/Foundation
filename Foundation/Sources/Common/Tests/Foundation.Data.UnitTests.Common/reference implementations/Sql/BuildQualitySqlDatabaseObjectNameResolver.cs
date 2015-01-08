using SquaredInfinity.Foundation.Data.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Data.UnitTests.Common.reference_implementations.Sql
{
     public class BuildQualitySqlDatabaseObjectNameResolver : SqlDatabaseObjectNameResolver
    {
         string BuildQuality { get; set; }

         public BuildQualitySqlDatabaseObjectNameResolver(SqlDataAccessService dataAccessService, string buildQuality)
             : base(dataAccessService)
        {
            this.BuildQuality = buildQuality;
         }

        public virtual string GetActualStoredProcedureName(string storedProcedureName)
        {
            // check if storedprocedure with build quality suffix exists (e.g. MyStoredProcedure__BETA)
            // if it does, use it instead

            var name = storedProcedureName + "__" + BuildQuality;

            if (base.DataAccessService.CheckStoredProcedureExists(name))
                return name;

            return storedProcedureName;
        }
    }
}
