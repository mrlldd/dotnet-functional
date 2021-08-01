using System;
using System.Threading;

namespace Functional.Object.Extensions
{
    /// <summary>
    /// The class that contains projection extension methods for real instances of any type.
    /// </summary>
    public static class MappingPresentedObjectExtensions
    {
        /// <summary>
        /// Projects the source object to the new object with given projector delegate if the source object is not null (present).
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/> or default value if the source object is not present.</returns>
        public static TResult? MapIfPresent<T, TResult>(this T? obj, Func<T, TResult> mapper)
            => obj == null ? default : mapper(obj);

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if the source object is not null (present).
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/> or default value if the source object is not present.</returns>
        public static TResult? MapIfPresent<T, TResult>(this T? obj, Func<T, CancellationToken, TResult> mapper, CancellationToken cancellationToken)
            => obj == null ? default : mapper(obj, cancellationToken);
    }
}