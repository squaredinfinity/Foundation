using SquaredInfinity.Foundation.TypeReaderWriters;
using SquaredInfinity.Foundation.TypeReflecting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.TypeMapping
{
    public class TypeMapper : ITypeMapper
    {
        ConcurrentDictionary<Type, ITypeReflector> x;

        TypeReflectorFactory TypeReflectorFactory { get; set ;}

        public TypeMapper(TypeReflectorFactory typeReflectorFctory)
        {
            this.TypeReflectorFactory = typeReflectorFctory;
        }
        
        public void Map<TFrom, TTo>(TFrom from, TTo to)
        {
            var fromReflector =
                x.GetOrAdd(
                    typeof(TFrom),
                    (key) => TypeReflectorFactory(key));

            var toReflector =
                x.GetOrAdd(
                    typeof(TTo),
                    (key) => TypeReflectorFactory(key));

            
            // what is the mapping strategy?
            // default -> map properties with same names and types

            //var commonProperties =
            //    (from fp in fromReflector
            //         where toReflector.)
        }


    }
}
