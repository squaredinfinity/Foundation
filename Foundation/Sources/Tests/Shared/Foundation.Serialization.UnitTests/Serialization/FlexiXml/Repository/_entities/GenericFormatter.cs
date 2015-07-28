using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.Repository._entities
{
    public class GenericFormatter : IFormatter
    {

        string _content;
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }
    }
}
