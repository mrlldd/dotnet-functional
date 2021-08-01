using System;
using System.Threading;
using System.Threading.Tasks;

namespace Functional.Object.Extensions
{
    /// <summary>
    /// The class that contains side effect extension methods for real instances of any type.
    /// </summary>
    public static class EffectObjectExtensions
    {
        /// <summary>
        /// Performs an effect on given object.
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
        /// Performs an effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="effect">The effect action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static T Effect<T>(this T obj, Action<T, CancellationToken> effect, CancellationToken cancellationToken)
        {
            effect(obj, cancellationToken);
            return obj;
        }

        /// <summary>
        /// Performs an effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="effect">The async effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectAsync<T>(this T obj, Func<T, Task> effect)
        {
            await effect(obj);
            return obj;
        }

        /// <summary>
        /// Performs an effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="effect">The async effect action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectAsync<T>(this T obj, Func<T, CancellationToken, Task> effect,
            CancellationToken cancellationToken)
        {
            await effect(obj, cancellationToken);
            return obj;
        }
    }
}