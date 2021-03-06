﻿using System;
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
                .Where(flag => mask.IsFlagSet(flag))
                .Cast<T>();
        }


        /// <summary>
        /// Determines whether [is flag set].
        /// </summary>
        /// <param name="me">The enum.</param>
        /// <param name="bitFlag">The bit flag.</param>
        /// <returns>
        /// 	<c>true</c> if [is flag set]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFlagSet(this Enum me, Enum bitFlag)
        {
            if (me.GetType() != bitFlag.GetType())
                throw new ArgumentException("Types of parameters have to be the same");

            return 
                (me as IConvertible).ToInt32(null).IsFlagSet((bitFlag as IConvertible).ToInt32(null));
        }

        public static TEnum Set<TEnum>(this Enum me, TEnum flag)
        {
             var underlyingType = Enum.GetUnderlyingType(me.GetType());

             if (underlyingType == typeof(int))
             {
                 var enumValue = (int)(object)me;
                 var flagValue = (int)(object)flag;

                 var result = (TEnum)(object)(enumValue | flagValue);

                 return result;
             }
             else
             {
                 var enumValue = (uint)(object)me;
                 var flagValue = (uint)(object)flag;

                 var result = (TEnum)(object)(enumValue | flagValue);

                 return result;
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
    }
}
