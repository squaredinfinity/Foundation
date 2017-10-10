using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Extensions
{
    public static class ObjectExtensions
    {
        public static bool TryConvert(this object obj, Type targetType, out object result)
        {
            result = null;

            if (obj != null)
            {
                var objType = obj.GetType();

                if (objType.IsTypeEquivalentTo(targetType, treatNullableAsEquivalent: true) || objType.ImplementsOrExtends(targetType))
                {
                    result = obj;
                    return true;
                }
            }

            if (targetType.IsEnum)
            {
                if (obj is string)
                {
                    result = Enum.Parse(targetType, obj as string);
                }
                else
                {
                    result = Enum.ToObject(targetType, obj);
                }

                return true;
            }

            // todo: test if conversion enum -> string, enum -> int etc are supported 

            if (obj is IConvertible)
            {
                result = System.Convert.ChangeType(obj, targetType);
                return true;
            }

            var converter = TypeDescriptor.GetConverter(obj);

            var cx = new _ConverterContext(obj);

            if (converter != null && converter.CanConvertTo(cx, targetType))
            {
                result = converter.ConvertTo(obj, targetType);
                return true;
            }

            return false;
        }

        public static object Convert(this object obj, Type targetType)
        {
            var result = (object)null;

            if (TryConvert(obj, targetType, out result))
                return result;

            throw new InvalidOperationException($"Unable to convert object to type {targetType.FullName}");
        }


        public static TTarget Convert<TTarget>(this object obj)
        {
            return (TTarget)Convert(obj, typeof(TTarget));
        }

    }
}
