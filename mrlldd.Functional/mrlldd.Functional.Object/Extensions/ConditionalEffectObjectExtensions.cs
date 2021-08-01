using System;
using System.Threading;
using System.Threading.Tasks;

namespace Functional.Object.Extensions
{
    /// <summary>
    /// The class that contains conditional side effect extension methods for real instances of any type.
    /// </summary>
    public static class ConditionalEffectObjectExtensions
    {
        /// <summary>
        /// Performs an effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="condition">The condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static T EffectIf<T>(this T obj, bool condition, Action<T> effect)
        {
            if (condition)
            {
                effect(obj);
            }

            return obj;
        }

        /// <summary>
        /// Performs an effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="condition">The condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static T EffectIf<T>(this T obj, bool condition, Action<T, CancellationToken> effect, CancellationToken cancellationToken)
        {
            if (condition)
            {
                effect(obj, cancellationToken);
            }

            return obj;
        }

        /// <summary>
        /// Performs an effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static T EffectIf<T>(this T obj, Func<bool> conditionProvider, Action<T> effect)
        {
            if (conditionProvider())
            {
                effect(obj);
            }

            return obj;
        }

        /// <summary>
        /// Performs an effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static T EffectIf<T>(this T obj, Func<CancellationToken, bool> conditionProvider, Action<T> effect, CancellationToken cancellationToken)
        {
            if (conditionProvider(cancellationToken))
            {
                effect(obj);
            }

            return obj;
        }

        /// <summary>
        /// Performs an effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static T EffectIf<T>(this T obj, Func<bool> conditionProvider, Action<T, CancellationToken> effect, CancellationToken cancellationToken)
        {
            if (conditionProvider())
            {
                effect(obj, cancellationToken);
            }

            return obj;
        }

        /// <summary>
        /// Performs an effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static T EffectIf<T>(this T obj, Func<CancellationToken, bool> conditionProvider, Action<T, CancellationToken> effect, CancellationToken cancellationToken)
        {
            if (conditionProvider(cancellationToken))
            {
                effect(obj, cancellationToken);
            }

            return obj;
        }

        /// <summary>
        /// Performs an effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static T EffectIf<T>(this T obj, Func<T, bool> conditionProvider, Action<T> effect)
        {
            if (conditionProvider(obj))
            {
                effect(obj);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="condition">The condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The async effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectIfAsync<T>(this T obj, bool condition, Func<T, Task> effect)
        {
            if (condition)
            {
                await effect(obj);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="condition">The condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The async effect action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectIfAsync<T>(this T obj, bool condition,
            Func<T, CancellationToken, Task> effect, CancellationToken cancellationToken)
        {
            if (condition)
            {
                await effect(obj, cancellationToken);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The async effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectIfAsync<T>(this T obj, Func<bool> conditionProvider, Func<T, Task> effect)
        {
            if (conditionProvider())
            {
                await effect(obj);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The async effect action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectIfAsync<T>(this T obj, Func<bool> conditionProvider,
            Func<T, CancellationToken, Task> effect, CancellationToken cancellationToken)
        {
            if (conditionProvider())
            {
                await effect(obj, cancellationToken);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectIfAsync<T>(this T obj, Func<Task<bool>> conditionProvider, Action<T> effect)
        {
            if (await conditionProvider())
            {
                effect(obj);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectIfAsync<T>(this T obj, Func<CancellationToken, Task<bool>> conditionProvider,
            Action<T> effect, CancellationToken cancellationToken)
        {
            if (await conditionProvider(cancellationToken))
            {
                effect(obj);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides async condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The async effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectIfAsync<T>(this T obj, Func<Task<bool>> conditionProvider,
            Func<T, Task> effect)
        {
            if (await conditionProvider())
            {
                await effect(obj);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides async condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The async effect action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectIfAsync<T>(this T obj, Func<CancellationToken, Task<bool>> conditionProvider,
            Func<T, CancellationToken, Task> effect, CancellationToken cancellationToken)
        {
            if (await conditionProvider(cancellationToken))
            {
                await effect(obj, cancellationToken);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides async condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectIfAsync<T>(this T obj, Func<T, Task<bool>> conditionProvider,
            Action<T> effect)
        {
            if (await conditionProvider(obj))
            {
                effect(obj);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides async condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectIfAsync<T>(this T obj,
            Func<T, CancellationToken, Task<bool>> conditionProvider, Action<T> effect,
            CancellationToken cancellationToken)
        {
            if (await conditionProvider(obj, cancellationToken))
            {
                effect(obj);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The async effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectIfAsync<T>(this T obj, Func<T, bool> conditionProvider, Func<T, Task> effect)
        {
            if (conditionProvider(obj))
            {
                await effect(obj);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The async effect action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectIfAsync<T>(this T obj, Func<T, bool> conditionProvider,
            Func<T, CancellationToken, Task> effect, CancellationToken cancellationToken)
        {
            if (conditionProvider(obj))
            {
                await effect(obj, cancellationToken);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides async condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The async effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectIfAsync<T>(this T obj, Func<T, Task<bool>> conditionProvider,
            Func<T, Task> effect)
        {
            if (await conditionProvider(obj))
            {
                await effect(obj);
            }

            return obj;
        }

        /// <summary>
        /// Performs an async effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides async condition <see cref="bool"/> that indicates if <paramref name="effect"/> should be executed.</param>
        /// <param name="effect">The async effect action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T> EffectIfAsync<T>(this T obj,
            Func<T, CancellationToken, Task<bool>> conditionProvider, Func<T, CancellationToken, Task> effect,
            CancellationToken cancellationToken)
        {
            if (await conditionProvider(obj, cancellationToken))
            {
                await effect(obj, cancellationToken);
            }

            return obj;
        }
    }
}