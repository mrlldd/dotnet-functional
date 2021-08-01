using System;
using System.Threading;

namespace Functional.Object.Extensions
{
    /// <summary>
    /// The class that contains conditional projection extension methods for any type.
    /// </summary>
    public static class ConditionalMappingObjectExtensions
    {
        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="condition"/> is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="condition">The condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult? MapIf<T, TResult>(this T obj, bool condition, Func<T, TResult> mapper)
            => condition ? mapper(obj) : default;

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="condition"/> is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="condition">The condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult? MapIf<T, TResult>(this T obj, bool condition, Func<T, CancellationToken, TResult> mapper,
            CancellationToken cancellationToken)
            => condition ? mapper(obj, cancellationToken) : default;


        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult? MapIf<T, TResult>(this T obj, Func<bool> conditionProvider, Func<T, TResult> mapper)
            => conditionProvider() ? mapper(obj) : default;

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult? MapIf<T, TResult>(this T obj, Func<CancellationToken, bool> conditionProvider,
            Func<T, TResult> mapper, CancellationToken cancellationToken)
            => conditionProvider(cancellationToken) ? mapper(obj) : default;

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult? MapIf<T, TResult>(this T obj, Func<bool> conditionProvider,
            Func<T, CancellationToken, TResult> mapper, CancellationToken cancellationToken)
            => conditionProvider() ? mapper(obj, cancellationToken) : default;

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult? MapIf<T, TResult>(this T obj, Func<CancellationToken, bool> conditionProvider,
            Func<T, CancellationToken, TResult> mapper, CancellationToken cancellationToken)
            => conditionProvider(cancellationToken) ? mapper(obj, cancellationToken) : default;

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValue">The value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<bool> conditionProvider, Func<T, TResult> mapper,
            TResult defaultValue)
            => conditionProvider() ? mapper(obj) : defaultValue;

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValue">The value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<CancellationToken, bool> conditionProvider,
            Func<T, TResult> mapper, TResult defaultValue, CancellationToken cancellationToken)
            => conditionProvider(cancellationToken) ? mapper(obj) : defaultValue;

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValue">The value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<bool> conditionProvider,
            Func<T, CancellationToken, TResult> mapper, TResult defaultValue, CancellationToken cancellationToken)
            => conditionProvider() ? mapper(obj, cancellationToken) : defaultValue;

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValue">The value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<CancellationToken, bool> conditionProvider,
            Func<T, CancellationToken, TResult> mapper, TResult defaultValue, CancellationToken cancellationToken)
            => conditionProvider(cancellationToken) ? mapper(obj, cancellationToken) : defaultValue;

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValue">The value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, bool> conditionProvider, Func<T, TResult> mapper,
            TResult defaultValue)
            => conditionProvider(obj) ? mapper(obj) : defaultValue;

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValue">The value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, CancellationToken, bool> conditionProvider,
            Func<T, TResult> mapper, TResult defaultValue, CancellationToken cancellationToken)
            => conditionProvider(obj, cancellationToken) ? mapper(obj) : defaultValue;

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValue">The value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, bool> conditionProvider,
            Func<T, CancellationToken, TResult> mapper,
            TResult defaultValue, CancellationToken cancellationToken)
            => conditionProvider(obj) ? mapper(obj, cancellationToken) : defaultValue;

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValue">The value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, CancellationToken, bool> conditionProvider,
            Func<T, CancellationToken, TResult> mapper,
            TResult defaultValue, CancellationToken cancellationToken)
            => conditionProvider(obj, cancellationToken) ? mapper(obj, cancellationToken) : defaultValue;

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, bool> conditionProvider, Func<T, TResult> mapper,
            Func<TResult> defaultValueProvider)
            => conditionProvider(obj) ? mapper(obj) : defaultValueProvider();

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, CancellationToken, bool> conditionProvider,
            Func<T, TResult> mapper,
            Func<TResult> defaultValueProvider, CancellationToken cancellationToken)
            => conditionProvider(obj, cancellationToken) ? mapper(obj) : defaultValueProvider();

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, bool> conditionProvider,
            Func<T, CancellationToken, TResult> mapper,
            Func<TResult> defaultValueProvider, CancellationToken cancellationToken)
            => conditionProvider(obj) ? mapper(obj, cancellationToken) : defaultValueProvider();

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, bool> conditionProvider, Func<T, TResult> mapper,
            Func<CancellationToken, TResult> defaultValueProvider, CancellationToken cancellationToken)
            => conditionProvider(obj) ? mapper(obj) : defaultValueProvider(cancellationToken);

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, CancellationToken, bool> conditionProvider,
            Func<T, CancellationToken, TResult> mapper,
            Func<TResult> defaultValueProvider, CancellationToken cancellationToken)
            => conditionProvider(obj, cancellationToken) ? mapper(obj, cancellationToken) : defaultValueProvider();

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, bool> conditionProvider,
            Func<T, CancellationToken, TResult> mapper,
            Func<CancellationToken, TResult> defaultValueProvider, CancellationToken cancellationToken)
            => conditionProvider(obj) ? mapper(obj, cancellationToken) : defaultValueProvider(cancellationToken);

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, CancellationToken, bool> conditionProvider,
            Func<T, TResult> mapper,
            Func<CancellationToken, TResult> defaultValueProvider, CancellationToken cancellationToken)
            => conditionProvider(obj, cancellationToken) ? mapper(obj) : defaultValueProvider(cancellationToken);

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition <see cref="bool"/> that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, CancellationToken, bool> conditionProvider,
            Func<T, CancellationToken, TResult> mapper,
            Func<CancellationToken, TResult> defaultValueProvider, CancellationToken cancellationToken)
            => conditionProvider(obj, cancellationToken)
                ? mapper(obj, cancellationToken)
                : defaultValueProvider(cancellationToken);

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition bool that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, bool> conditionProvider, Func<T, TResult> mapper,
            Func<T, TResult> defaultValueProvider)
            => conditionProvider(obj) ? mapper(obj) : defaultValueProvider(obj);

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition bool that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T,CancellationToken, bool> conditionProvider, Func<T, TResult> mapper,
            Func<T, TResult> defaultValueProvider, CancellationToken cancellationToken)
            => conditionProvider(obj, cancellationToken) ? mapper(obj) : defaultValueProvider(obj);

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition bool that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, bool> conditionProvider, Func<T, CancellationToken, TResult> mapper,
            Func<T, TResult> defaultValueProvider, CancellationToken cancellationToken)
            => conditionProvider(obj) ? mapper(obj, cancellationToken) : defaultValueProvider(obj);

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition bool that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, bool> conditionProvider, Func<T, TResult> mapper,
            Func<T, CancellationToken, TResult> defaultValueProvider, CancellationToken cancellationToken)
            => conditionProvider(obj) ? mapper(obj) : defaultValueProvider(obj, cancellationToken);

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition bool that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, CancellationToken, bool> conditionProvider, Func<T, CancellationToken, TResult> mapper,
            Func<T, TResult> defaultValueProvider, CancellationToken cancellationToken)
            => conditionProvider(obj, cancellationToken) ? mapper(obj, cancellationToken) : defaultValueProvider(obj);

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition bool that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, bool> conditionProvider, Func<T, CancellationToken, TResult> mapper,
            Func<T, CancellationToken, TResult> defaultValueProvider, CancellationToken cancellationToken)
            => conditionProvider(obj) ? mapper(obj, cancellationToken) : defaultValueProvider(obj, cancellationToken);

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition bool that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T,CancellationToken, bool> conditionProvider, Func<T, TResult> mapper,
            Func<T, CancellationToken, TResult> defaultValueProvider, CancellationToken cancellationToken)
            => conditionProvider(obj, cancellationToken) ? mapper(obj) : defaultValueProvider(obj, cancellationToken);

        /// <summary>
        /// Projects the source object to the new object with given projector delegate if <paramref name="conditionProvider"/> result is truthy, otherwise - default value of <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="obj">The source object.</param>
        /// <param name="conditionProvider">The delegate that provides condition bool that indicates if object should be projected.</param>
        /// <param name="mapper">The projector delegate.</param>
        /// <param name="defaultValueProvider">The delegate that provides value of <typeparamref name="TResult"/> type that will be returned if <paramref name="conditionProvider"/> result would be falsy.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The source object type.</typeparam>
        /// <typeparam name="TResult">The result object type.</typeparam>
        /// <returns>The projected value of <typeparamref name="TResult"/>.</returns>
        public static TResult MapIf<T, TResult>(this T obj, Func<T, CancellationToken, bool> conditionProvider, Func<T, CancellationToken, TResult> mapper,
            Func<T, CancellationToken, TResult> defaultValueProvider, CancellationToken cancellationToken)
            => conditionProvider(obj, cancellationToken) ? mapper(obj, cancellationToken) : defaultValueProvider(obj, cancellationToken);
    }
}