using System;
using System.Linq;

namespace mrlldd.Functional.Object.Extensions
{
    public static class ObjectExtensions
    {
        public static TResult Map<T, TResult>(this T obj, Func<T, TResult> mapper) 
            => mapper(obj);

        public static TResult? MapIfPresent<T, TResult>(this T obj, Func<T, TResult> mapper)
            => obj == null ? default : mapper(obj);
        
        public static T Effect<T>(this T obj, Action<T> effect)
        {
            effect(obj);
            return obj;
        }

        public static bool In<T>(this T obj, params T[] values)
            => values.Contains(obj);
    }
}