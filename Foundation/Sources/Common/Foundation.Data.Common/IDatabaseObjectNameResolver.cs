using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Data
{
    public interface IDatabaseObjectNameResolver
    {
        string GetActualStoredProcedureName(string storedProcedureName);
    }
}
