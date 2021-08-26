using System;
using System.Threading;
using System.Threading.Tasks;

namespace Functional.Object.Extensions
{
    /// <summary>
    /// The class that contains side effect extension methods for real instances of any type.
    /// </summary>
    public static class EffectPresentedObjectExtensions
    {
        /// <summary>
        /// Performs an effect on given object if given object is not null (present).
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="effect">The effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static T? EffectIfPresent<T>(this T? obj, Action<T> effect)
        {
            if (obj != null)
            {
                effect(obj);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object if given object is not null (present).
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="effect">The async effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T?> EffectIfPresentAsync<T>(this T? obj, Func<T, Task> effect)
        {
            if (obj != null)
            {
                await effect(obj);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object if given object is not null (present).
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="effect">The async effect action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T?> EffectIfPresentAsync<T>(this T? obj, Func<T, CancellationToken, Task> effect,
            CancellationToken cancellationToken)
        {
            if (obj != null)
            {
                await effect(obj, cancellationToken);
            }

            return obj;
        }
    }
}