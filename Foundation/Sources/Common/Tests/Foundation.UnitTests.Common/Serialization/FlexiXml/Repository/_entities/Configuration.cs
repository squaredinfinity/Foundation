using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.Repository._entities
{
    public class Configuration
    {
        public Repository Repository { get; set; }

        List<Email> _emails;
        public List<Email> Emails { get { return _emails; } }

        Configuration()
        {
            _emails = new List<Email>();
        }
    }
}   
