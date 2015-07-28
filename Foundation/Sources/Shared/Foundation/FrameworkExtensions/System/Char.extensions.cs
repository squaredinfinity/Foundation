using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class charExtensions
    {
        static Dictionary<char, string> CharToVerbatimStringMapping;

        static charExtensions()
        {
            CharToVerbatimStringMapping = new Dictionary<char, string>()
            {
                 { '\t', @"\t" },
                 { '\r', @"\r" },
                 { '\n', @"\n" },
                 { '\0', @"\0" },
                 { '\a', @"\a" },
                 { '\b', @"\b" },
                 { '\f', @"\f" },
                 { '\v', @"\v" }
            };
        }

        public static string ToVerbatimString(this char c)
        {
            if (CharToVerbatimStringMapping.ContainsKey(c))
                return CharToVerbatimStringMapping[c];

            return new string(c, 1);
        }
    }
}
