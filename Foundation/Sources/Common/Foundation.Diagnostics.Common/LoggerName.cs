using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics
{
    [DebuggerDisplay("{RawName}")]
    internal class LoggerName : ILoggerName
    {
        static readonly char NamePartSeparator = '.';

        string RawName { get; set; }

        // todo: is it still needed?
        // could be used to allow/disable logs based on something other than namespace
        // for example: a module name, user defined name, etc.
        //List<string> _aliases = new List<string>();
        //public List<string> Aliases
        //{
        //    get { return _aliases; }
        //}

        List<string> NamePartHierarchy { get; set; }


        public LoggerName(string rawName)
        {
            this.RawName = rawName;

            this.NamePartHierarchy = GetNamePartHierarchy(RawName);
        }

        /// <summary>
        /// Gets hierarchy of a name parts.
        /// For example, System.Diagnostics.Trace contains following hierarchy:
        ///     - System
        ///     - System.Diagnostics
        ///     - System.Diagnostics.Trace
        /// </summary>
        /// <param name="rawName"></param>
        public static List<string> GetNamePartHierarchy(string rawName)
        {
            var namePartHierarchy = new List<string>();

            namePartHierarchy.Add(rawName);

            var copyOfRawName = rawName;

            var lastSeparatorIndex = copyOfRawName.LastIndexOf(NamePartSeparator);

            while (lastSeparatorIndex >= 0)
            {
                copyOfRawName = copyOfRawName.Substring(0, lastSeparatorIndex);

                namePartHierarchy.Add(copyOfRawName);

                lastSeparatorIndex = copyOfRawName.LastIndexOf(NamePartSeparator);
            }

            return namePartHierarchy;
        }

        ReadOnlyCollection<string> ILoggerName.NamePartHierarchy
        {
            get { return NamePartHierarchy.ToReadOnly(); }
        }

        public override string ToString()
        {
            return RawName;
        }
    }
}
