using System;
using System.Collections.Generic;
using System.Linq;

namespace Functional.Object.Extensions
{
    /// <summary>
    /// The class that contains extension methods for any type.
    /// </summary>
    public static class ObjectExtensions
    {
        
        /// <summary>
        /// Projects the source object to the new object with given projector delegate.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="mapper">The projector delegate</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <see cref="TResult"/>.</returns>
        public static TResult Map<T, TResult>(this T obj, Func<T, TResult> mapper) 
            => mapper(obj);
        
        
        /// <summary>
        /// Projects the source object to the new object with given projector delegate if the source object is not null (not present).
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="mapper">The projector delegate</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <see cref="TResult"/> or default value if the source object is not present.</returns>
        public static TResult? MapIfPresent<T, TResult>(this T obj, Func<T, TResult> mapper)
            => obj == null ? default : mapper(obj);
        
        
        /// <summary>
        /// Performs an effect on given object
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="effect">The effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static T Effect<T>(this T obj, Action<T> effect)
        {
            effect(obj);
            return obj;
        }

        /// <summary>
        /// Checks if the given values collection contains source object
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="values">The values.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The <see cref="bool"/> that indicates existence of given object in values collection</returns>
        public static bool In<T>(this T obj, params T[] values)
            => values.Contains(obj);
        
        /// <summary>
        /// Checks if the given values collection contains source object
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="values">The values.</param>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <returns>The <see cref="bool"/> that indicates existence of given object in values collection</returns>
        public static bool In<T>(this T obj, IEnumerable<T> values)
            => values.Contains(obj);
        
    }
}