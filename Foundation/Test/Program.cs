using SquaredInfinity.Foundation.Types.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var c1 = new C1();
            var t1 = new T1 { Id = 13, Name = "(. )( .)" };
            c1.Add(t1);


            var c2 = new C1();
            var t2 = new T1 { Id = 7, Name = "( ^ )( ^ )" };
            c2.Add(t2);

            var tm = new TypeMapper();

            var ms = tm.GetOrCreateTypeMappingStrategy<T1, T1>(autoMatchMembers: false);
            // todo: make it more user friendly
            ms.MapMember<T1, string>("Name", source => source.Name + ":p");

            tm.Map(c1, c2);

            var x = "";
        }
    }


    public class C1 : List<T1>
    {

    }

    public class T1
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
