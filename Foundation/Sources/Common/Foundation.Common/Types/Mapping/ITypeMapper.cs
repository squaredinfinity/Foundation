using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    public interface ITypeMapper
    {
        /// <summary>
        /// Creates a deep clone of *source*.
        /// The clone will be of *TTarget* type.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        TTarget DeepClone<TTarget>(TTarget source) where TTarget : class, new();

        /// <summary>
        /// Creates a deep clone of *source*.
        /// The clone will be of the same type as the *source*.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        object DeepClone(object source);

        /// <summary>
        /// Creates a deep clone of *source*.
        /// The clone will be of *targetType* type.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        object DeepClone(object source, Type targetType);



        /// <summary>
        /// Maps *source* to *target*.
        /// Only members of *TTarget* type will be mapped.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        void Map<TTarget>(object source, TTarget target) where TTarget : class, new();
        void Map<TTarget>(object source, TTarget target, MappingOptions options) where TTarget : class, new();

        /// <summary>
        /// Clones *source* into *target*.
        /// Only members of *targetType* will be mapped.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="targetType"></param>
        void Map(object source, object target, Type targetType);
        void Map(object source, object target, Type targetType, MappingOptions options);

        TTarget Map<TTarget>(object source) where TTarget : class, new();
        TTarget Map<TTarget>(object source, MappingOptions options) where TTarget : class, new();

        object Map(object source, Type targetType);
        object Map(object source, Type targetType, MappingOptions options);
    }
}
