using System.Collections.Generic;
using System.Linq;

namespace Functional.Object.Extensions
{
    /// <summary>
    /// The class that contains equality extension methods for any type.
    /// </summary>
    public static class EqualityObjectExtensions
    {
        /// <summary>
        /// Checks if the given values collection contains source object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="values">The values.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The <see cref="bool"/> that indicates existence of given object in values collection.</returns>
        public static bool In<T>(this T? obj, params T?[] values)
            => values.Contains(obj);

        /// <summary>
        /// Checks if the given values collection contains source object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="values">The values.</param>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <returns>The <see cref="bool"/> that indicates existence of given object in values collection.</returns>
        public static bool In<T>(this T? obj, IEnumerable<T?> values)
            => values.Contains(obj);
        
        // todo extend with overloads and other methods.
    }
}