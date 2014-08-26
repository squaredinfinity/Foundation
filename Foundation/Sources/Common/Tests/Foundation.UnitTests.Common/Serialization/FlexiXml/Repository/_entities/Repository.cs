using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.Repository._entities
{
    public class Repository
    {
        public IList<IFormatter> Formatters { get; private set; }

        public Repository()
        {
            Formatters = new List<IFormatter>();
        }
    }
}
