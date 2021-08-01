using System;
using System.Threading;

namespace Functional.Object.Extensions
{
    /// <summary>
    /// The class that contains projection extension methods for any type.
    /// </summary>
    public static class MappingObjectExtensions
    {
        /// <summary>
        /// Projects the source object to the new object with given projector delegate.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult Map<T, TResult>(this T obj, Func<T, TResult> mapper)
            => mapper(obj);

        /// <summary>
        /// Projects the source object to the new object with given projector delegate.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult Map<T, TResult>(this T obj, Func<T, CancellationToken, TResult> mapper,
            CancellationToken cancellationToken)
            => mapper(obj, cancellationToken);
    }
}