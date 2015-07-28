using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.KnownTypesLookup
{
    public class TestRoot
    {
        public ITestEntity Entity { get; set; }
    }

    public interface ITestEntity
    {
        int Id { get; }
    }

    public class TestEntityA : ITestEntity
    {
        public int Id { get; set; }
    }

    public class TestEntityB : ITestEntity
    {
        public int Id { get; set; }
    }

}
