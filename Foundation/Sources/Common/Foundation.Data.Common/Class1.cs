//    public static class ArrayExtensions
//    {
//        public static bool ArrayContentsEqual<T>(this Array a1, Array a2, IEqualityComparer<T> comparer = null)
//        {
//            if (ReferenceEquals(a1, a2))
//                return true;

//            if (a1 == null || a2 == null)
//                return false;

//            if (a1.Length != a2.Length)
//                return false;

//            if (a1.Rank != a2.Rank)
//                return false;

//            if (comparer == null)
//                comparer = EqualityComparer<T>.Default;

//            if (a1.Rank == 1)
//            {
//                for (int i = 0; i < a1.Length; i++)
//                {
//                    var e1 = (T)a1.GetValue(i);
//                    var e2 = (T)a2.GetValue(i);

//                    if (!comparer.Equals(e1, e2))
//                        return false;
//                }
//            }
//            else if (a1.Rank == 2)
//            {
//                var r0LB = a1.GetLowerBound(0);
//                var r0UB = a1.GetUpperBound(0);

//                var r1LB = a1.GetLowerBound(1);
//                var r1UB = a1.GetUpperBound(1);

//                for (int i = r0LB; i <= r0UB; i++)
//                {
//                    for (int j = r1LB; j <= r1UB; j++)
//                    {
//                        var e1 = (T)a1.GetValue(new int[] { i, j });
//                        var e2 = (T)a2.GetValue(new int[] { i, j });

//                        if (!comparer.Equals(e1, e2))
//                            return false;
//                    }
//                }
//            }

//            return true;
//        }
//    }
//}

//public static class ObjectExtensions
//{
//    public static object ToDbValue(this object obj)
//    {
//        if (obj == null) return DBNull.Value;

//        var ts = obj as TimeSpan?;
//        if (ts != null)
//        {
//            return ts.Value.Ticks;
//        }

//        if (obj.GetType() == typeof(XDocument))
//            return (obj as XDocument).ToString(SaveOptions.None);

//        return obj;
//    }

//    public static TValue ToClrValue<TValue>(this object obj, TValue defaultValue = default(TValue))
//    {
//        if (obj == null)
//            return defaultValue;

//        if (obj == DBNull.Value)
//            return defaultValue;

//        if (typeof(TValue) == typeof(TimeSpan?) || typeof(TValue) == typeof(TimeSpan))
//            return (TValue)(object)TimeSpan.FromTicks((long)obj);

//        if (typeof(TValue) == typeof(XDocument))
//            return (TValue)(object)XDocument.Parse(obj as string);

//        return (TValue)obj;
//    }
//}
