using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        /// <param name="mapper">The projector delegate.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <see cref="TResult"/>.</returns>
        public static TResult? Map<T, TResult>(this T? obj, Func<T?, TResult?> mapper) 
            => mapper(obj);

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <see cref="condition"/> is truthy, otherwise - default value of <see cref="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="condition">The condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <see cref="TResult"/>.</returns>
        public static TResult? MapIf<T, TResult>(this T? obj, bool condition, Func<T?, TResult?> mapper) 
            => condition ? mapper(obj) : default;
        
        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <see cref="conditionProvider"/> result is truthy, otherwise - default value of <see cref="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <see cref="TResult"/>.</returns>
        public static TResult? MapIf<T, TResult>(this T? obj, Func<bool> conditionProvider, Func<T?, TResult?> mapper) 
            => conditionProvider() ? mapper(obj) : default;

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <see cref="conditionProvider"/> result is truthy, otherwise - default value of <see cref="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValue">The value of <see cref="TResult"/> type that will be returned if <see cref="conditionProvider"/> result would be falsy.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <see cref="TResult"/>.</returns>
        public static TResult? MapIf<T, TResult>(this T? obj, Func<bool> conditionProvider, Func<T?, TResult?> mapper, TResult? defaultValue) 
            => conditionProvider() ? mapper(obj) : defaultValue;
        
        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <see cref="conditionProvider"/> result is truthy, otherwise - default value of <see cref="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValue">The value of <see cref="TResult"/> type that will be returned if <see cref="conditionProvider"/> result would be falsy.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <see cref="TResult"/>.</returns>
        public static TResult? MapIf<T, TResult>(this T? obj, Func<T?, bool> conditionProvider, Func<T?, TResult?> mapper, TResult? defaultValue) 
            => conditionProvider(obj) ? mapper(obj) : defaultValue;
        
        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <see cref="conditionProvider"/> result is truthy, otherwise - default value of <see cref="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <see cref="TResult"/> type that will be returned if <see cref="conditionProvider"/> result would be falsy.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <see cref="TResult"/>.</returns>
        public static TResult? MapIf<T, TResult>(this T? obj, Func<T?, bool> conditionProvider, Func<T?, TResult?> mapper, Func<TResult?> defaultValueProvider) 
            => conditionProvider(obj) ? mapper(obj) : defaultValueProvider();
        
        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <see cref="conditionProvider"/> result is truthy, otherwise - default value of <see cref="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition bool that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <see cref="TResult"/> type that will be returned if <see cref="conditionProvider"/> result would be falsy.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <see cref="TResult"/>.</returns>
        public static TResult? MapIf<T, TResult>(this T? obj, Func<T?, bool> conditionProvider, Func<T?, TResult?> mapper, Func<T?, TResult?> defaultValueProvider) 
            => conditionProvider(obj) ? mapper(obj) : defaultValueProvider(obj);


        /// <summary>
        /// Projects the source object to the new object with given projector delegate if the source object is not null (present).
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <see cref="TResult"/> or default value if the source object is not present.</returns>
        public static TResult? MapIfPresent<T, TResult>(this T? obj, Func<T, TResult> mapper)
            => obj == null ? default : mapper(obj);
        
        
        /// <summary>
        /// Performs an effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="effect">The effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static T? Effect<T>(this T? obj, Action<T?> effect)
        {
            effect(obj);
            return obj;
        }
        
        /// <summary>
        /// Performs an effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="effect">The async effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T?> EffectAsync<T>(this T? obj, Func<T?, Task> effect)
        {
            await effect(obj);
            return obj;
        }

        /// <summary>
        /// Performs an effect on given object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="condition">The condition <see cref="bool"/> that indicates if <see cref="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static T? EffectIf<T>(this T? obj, bool condition, Action<T?> effect)
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
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <see cref="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static T? EffectIf<T>(this T? obj, Func<bool> conditionProvider, Action<T?> effect)
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
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <see cref="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static T? EffectIf<T>(this T? obj, Func<T?, bool> conditionProvider, Action<T?> effect)
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
        /// <param name="condition">The condition <see cref="bool"/> that indicates if <see cref="effect"/> should be executed.</param>
        /// <param name="effect">The async effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T?> EffectIfAsync<T>(this T? obj, bool condition, Func<T?, Task> effect)
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
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <see cref="effect"/> should be executed.</param>
        /// <param name="effect">The async effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T?> EffectIfAsync<T>(this T? obj, Func<bool> conditionProvider, Func<T?, Task> effect)
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
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <see cref="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T?> EffectIfAsync<T>(this T? obj, Func<Task<bool>> conditionProvider, Action<T?> effect)
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
        /// <param name="conditionProvider">The delegate that provides async condition <see cref="bool"/> that indicates if <see cref="effect"/> should be executed.</param>
        /// <param name="effect">The async effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T?> EffectIfAsync<T>(this T? obj, Func<Task<bool>> conditionProvider, Func<T?, Task> effect)
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
        /// <param name="conditionProvider">The delegate that provides async condition <see cref="bool"/> that indicates if <see cref="effect"/> should be executed.</param>
        /// <param name="effect">The effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T?> EffectIfAsync<T>(this T? obj, Func<T?, Task<bool>> conditionProvider, Action<T?> effect)
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
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if <see cref="effect"/> should be executed.</param>
        /// <param name="effect">The async effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T?> EffectIfAsync<T>(this T? obj, Func<T?, bool> conditionProvider, Func<T?, Task> effect)
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
        /// <param name="conditionProvider">The delegate that provides async condition <see cref="bool"/> that indicates if <see cref="effect"/> should be executed.</param>
        /// <param name="effect">The async effect action.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The source object.</returns>
        public static async Task<T?> EffectIfAsync<T>(this T? obj, Func<T?, Task<bool>> conditionProvider, Func<T?, Task> effect)
        {
            if (await conditionProvider(obj))
            {
                await effect(obj);
            }

            return obj;
        }
        
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
        /// Checks if the given values collection contains source object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="values">The values.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <returns>The <see cref="bool"/> that indicates existence of given object in values collection</returns>
        public static bool In<T>(this T? obj, params T?[] values)
            => values.Contains(obj);
        
        /// <summary>
        /// Checks if the given values collection contains source object.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="values">The values.</param>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <returns>The <see cref="bool"/> that indicates existence of given object in values collection</returns>
        public static bool In<T>(this T? obj, IEnumerable<T?> values)
            => values.Contains(obj);
        
    }
}