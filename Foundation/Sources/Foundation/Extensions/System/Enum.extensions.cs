using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Extensions
{
    /// <summary>
    /// Extensions for Enum types.
    /// </summary>
    public static class EnumExtensions
    {
        public static IEnumerable<T> GetAllSetFlags<T>(this Enum mask)
        {
            return Enum.GetValues(typeof(T))
                .Cast<Enum>()
                .Where(flag => mask.HasFlag(flag))
                .Cast<T>();
        }

        public static TEnum Set<TEnum>(this Enum me, TEnum flag)
        {
             var underlyingType = Enum.GetUnderlyingType(me.GetType());

             if (underlyingType == typeof(int))
             {
                var enumValue = (int)(object)me;
                var flagValue = (int)(object)flag;

                var result = (TEnum)Enum.ToObject(typeof(TEnum), (object)(enumValue | flagValue));

                return result;
             }
             else
             if(underlyingType == typeof(uint))
             {
                var enumValue = (uint)(object)me;
                var flagValue = (uint)(object)flag;

                var result = (TEnum)Enum.ToObject(typeof(TEnum), (object)(enumValue | flagValue));

                return result;
             }
            else
            if (underlyingType == typeof(byte))
            {
                var enumValue = (byte)(object)me;
                var flagValue = (byte)(object)flag;

                var result = (TEnum)Enum.ToObject(typeof(TEnum), (object)(enumValue | flagValue));

                return result;
            }
            else
            {
                throw new NotSupportedException($"Enums with underlying type of {underlyingType.Name} are not supported.");
            }
        }

        public static Enum Set(this Enum me, Enum flag)
        {
            var underlyingType = Enum.GetUnderlyingType(me.GetType());

            if (underlyingType == typeof(int))
            {
                var enumValue = (int)(object)me;
                var flagValue = (int)(object)flag;

                var result = (Enum)Enum.ToObject(flag.GetType(), (object)(enumValue | flagValue));

                return result;
            }
            else
            {
                var enumValue = (uint)(object)me;
                var flagValue = (uint)(object)flag;

                var result = (Enum)Enum.ToObject(flag.GetType(), (object)(enumValue | flagValue));

                return result;
            }
        }

        public static TEnum Unset<TEnum>(this Enum me, TEnum flag)
        {
            var underlyingType = Enum.GetUnderlyingType(me.GetType());

            if (underlyingType == typeof(int))
            {
                var enumValue = (int)(object)me;
                var flagValue = (int)(object)flag;

                var result = (TEnum)(object)(enumValue & (~flagValue));

                return result;
            }
            else
            {
                var enumValue = (uint)(object)me;
                var flagValue = (uint)(object)flag;

                var result = (TEnum)(object)(enumValue & (~flagValue));

                return result;
            }
        }

        public static Enum Unset(this Enum me, Enum flag)
        {
            var underlyingType = Enum.GetUnderlyingType(me.GetType());

            if (underlyingType == typeof(int))
            {
                var enumValue = (int)(object)me;
                var flagValue = (int)(object)flag;

                var result = (Enum)Enum.ToObject(flag.GetType(), (object)(enumValue & (~flagValue)));

                return result;
            }
            else
            {
                var enumValue = (uint)(object)me;
                var flagValue = (uint)(object)flag;

                var result = (Enum)Enum.ToObject(flag.GetType(), (object)(enumValue & (~flagValue)));

                return result;
            }
        }

        /// <summary>
        /// Performs logical OR on multiple enum values
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="initalValue"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static TEnum Or<TEnum>(TEnum initalValue, IEnumerable<TEnum> values)
            //  Can't do this, constraint cannot be special class Enum
            //- where TEnum : Enum
        {
            var result = initalValue as Enum;

            if (result == null)
                throw new ArgumentException("TEnum must be an Enum");

            foreach (var v in values)
            {
                result = (Enum)Enum.ToObject(typeof(TEnum), (object)(result.Set(v)));
            }

            return (TEnum)Enum.ToObject(typeof(TEnum), (object)(result));
        }
    }
}
