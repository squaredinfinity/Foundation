using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1.Types.Mapping.TestEntities
{
    public class LinkedListNode
    {
        public int Id { get; set; }
        public LinkedListNode Previous { get; set; }
        public LinkedListNode Next { get; set; }
    }
}
