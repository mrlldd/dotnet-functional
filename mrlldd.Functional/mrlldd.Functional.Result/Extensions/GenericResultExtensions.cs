using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using mrlldd.Functional.Result.Internal;
using mrlldd.Functional.Result.Internal.Utilities;

namespace mrlldd.Functional.Result.Extensions
{
    public static class GenericResultExtensions
    {
        /// <summary>
        /// Wraps a value to success result type.
        /// </summary>
        /// <param name="value">The value that will be wrapped.</param>
        /// <typeparam name="T">The type of value that will be wrapped.</typeparam>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> AsSuccess<T>(this T value) => new Success<T>(value);

        /// <summary>
        /// Wraps an exception to fail result type.
        /// </summary>
        /// <param name="exception">The exception that will be wrapped.</param>
        /// <typeparam name="T">The type of value that will be wrapped.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown when exception is null.</exception>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> AsFail<T>(this Exception exception) => new Fail<T>(exception);

        /// <summary>
        /// Unwraps result as success.
        /// </summary>
        /// <param name="result">The result that will be unwrapped.</param>
        /// <typeparam name="T">The type of value that wrapped.</typeparam>
        /// <exception cref="Exceptions.ResultUnwrapException">Thrown when result is not <see cref="Result{T}.Successful"/></exception>
        /// <returns>The <see cref="T"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T UnwrapAsSuccess<T>(this Result<T> result) => result;

        /// <summary>
        /// Unwraps result as fail.
        /// </summary>
        /// <param name="result">The result that will be unwrapped.</param>
        /// <typeparam name="T">The type of value that wrapped.</typeparam>
        /// <exception cref="Exceptions.ResultUnwrapException">Thrown when result is <see cref="Result{T}.Successful"/></exception>
        /// <returns>The <see cref="Exception"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Exception UnwrapAsFail<T>(this Result<T> result) => result;

        /// <summary>
        /// Binds a result to another result if it is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="mapper">The factory function that will be called if source result is <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of source result value.</typeparam>
        /// <typeparam name="TMapped">The type of result value that will be returned.</typeparam>
        /// <returns>The <see cref="Result{TMapped}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TMapped> Bind<T, TMapped>(this Result<T> result, Func<T, TMapped> mapper)
            => result.Successful
                ? Execute.Safely(((Success<T>) result).Value, mapper)
                : FailFactory.GenericException<TMapped>(((Fail<T>) result).Exception);

        /// <summary>
        /// Binds a result to another result if it is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="mapper">The factory function that will be called if source result is <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of source result value.</typeparam>
        /// <typeparam name="TMapped">The type of result value that will be returned.</typeparam>
        /// <returns>The <see cref="Result{TMapped}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<TMapped> Bind<T, TMapped>(this Result<T> result,
            Func<T, CancellationToken, TMapped> mapper, CancellationToken cancellationToken = default)
            => result.Successful
                ? Execute.Safely(((Success<T>) result).Value, mapper, cancellationToken)
                : FailFactory.GenericException<TMapped>(((Fail<T>) result).Exception);

        /// <summary>
        /// Binds a result to another result task if it is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="asyncMapper">The async factory function that will be called if source result is <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of source result value.</typeparam>
        /// <typeparam name="TMapped">The type of result value that will be returned.</typeparam>
        /// <returns>The <see cref="Task"/> that returns <see cref="Result{TMapped}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<TMapped>> Bind<T, TMapped>(this Result<T> result, Func<T, Task<TMapped>> asyncMapper)
            => result.Successful
                ? asyncMapper(((Success<T>) result).Value)
                    .ThenWrapAsResult()
                : FailFactory.GenericExceptionTask<TMapped>(((Fail<T>) result).Exception);

        /// <summary>
        /// Binds a result to another result task if it is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="asyncMapper">The async factory function that will be called if source result is <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of source result value.</typeparam>
        /// <typeparam name="TMapped">The type of result value that will be returned.</typeparam>
        /// <returns>The <see cref="Task"/> that returns <see cref="Result{TMapped}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<TMapped>> Bind<T, TMapped>(this Result<T> result,
            Func<T, CancellationToken, Task<TMapped>> asyncMapper, CancellationToken cancellationToken)
            => result.Successful
                ? asyncMapper(result, cancellationToken)
                    // ReSharper disable once MethodSupportsCancellation
                    .ContinueWith(task =>
                        task.Exception == null
                            ? task.IsCanceled
                                ? FailFactory.GenericCanceled<TMapped>(task)
                                : task.Result.AsSuccess()
                            : FailFactory.GenericException<TMapped>(task.Exception))
                : FailFactory.GenericExceptionTask<TMapped>(((Fail<T>) result).Exception);

        /// <summary>
        /// Binds a result task to another result task if it will be <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="asyncMapper">The async factory function that will be called if source task result will be <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of source task result value.</typeparam>
        /// <typeparam name="TMapped">The type of result value that will be returned.</typeparam>
        /// <returns>The <see cref="Task"/> that returns <see cref="Result{TMapped}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<TMapped>> Bind<T, TMapped>(this Task<Result<T>> sourceTask,
            Func<T, Task<TMapped>> asyncMapper)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                        ? task.IsCanceled
                            ? FailFactory.GenericCanceledTask<TMapped>(task)
                            : task.Result
                                .Bind(asyncMapper)
                        : FailFactory.GenericExceptionTask<TMapped>(task.Exception))
                .Unwrap();

        /// <summary>
        /// Binds a result task to another result task if it will be <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="asyncMapper">The async factory function that will be called if source task result will be <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of source task result value.</typeparam>
        /// <typeparam name="TMapped">The type of result value that will be returned.</typeparam>
        /// <returns>The <see cref="Task"/> that returns <see cref="Result{TMapped}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<TMapped>> Bind<T, TMapped>(this Task<Result<T>> sourceTask,
            Func<T, CancellationToken, Task<TMapped>> asyncMapper, CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.GenericCanceledTask<TMapped>(task)
                        : task.Result
                            .Bind(asyncMapper, cancellationToken)
                    : FailFactory.GenericExceptionTask<TMapped>(task.Exception))
                .Unwrap();

        /// <summary>
        /// Binds a result task to another result if it will be <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="mapper">The factory function that will be called if source result is <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of source task result value.</typeparam>
        /// <typeparam name="TMapped">The type of result value that will be returned.</typeparam>
        /// <returns>The <see cref="Task"/> that returns <see cref="Result{TMapped}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<TMapped>> Bind<T, TMapped>(this Task<Result<T>> sourceTask,
            Func<T, TMapped> mapper)
            => sourceTask
                .ContinueWith(task => task.Exception ?? task.Result.Bind(mapper));

        /// <summary>
        /// Binds a result task to another result if it will be <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="mapper">The factory function that will be called if source result is <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of source task result value.</typeparam>
        /// <typeparam name="TMapped">The type of result value that will be returned.</typeparam>
        /// <returns>The <see cref="Task"/> that returns <see cref="Result{TMapped}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<TMapped>> Bind<T, TMapped>(this Task<Result<T>> sourceTask,
            Func<T, CancellationToken, TMapped> mapper, CancellationToken cancellationToken)
            => sourceTask
                // ReSharper disable once MethodSupportsCancellation
                .ContinueWith(task => task.Exception ?? task.Result.Bind(mapper, cancellationToken));
        
        /// <summary>
        /// Wraps a task result to <see cref="Result{T}"/> type. 
        /// </summary>
        /// <param name="sourceTask">The result task that will be wrapped.</param>
        /// <typeparam name="T">The type of source task returned value.</typeparam>
        /// <returns>The <see cref="Task"/> that returns <see cref="Result{TMapped}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<T>> ThenWrapAsResult<T>(this Task<T> sourceTask)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.GenericCanceled<T>(task)
                        : task.Result.AsSuccess()
                    : FailFactory.GenericException<T>(task.Exception)
                );

        /// <summary>
        /// Performs an effect if result is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="effect">The effect that should be performed if result is <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> EffectIfSuccessful<T>(this Result<T> result, Action effect)
        {
            if (result.Successful)
            {
                effect();
            }

            return result;
        }
        /// <summary>
        /// Performs an effect if result is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="effect">The effect that should be performed if result is <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> EffectIfSuccessful<T>(this Result<T> result, Action<T> effect)
        {
            if (result.Successful)
            {
                effect(((Success<T>) result).Value);
            }

            return result;
        }
        /// <summary>
        /// Performs an effect if result is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="effect">The effect that should be performed if result is <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> EffectIfSuccessful<T>(this Result<T> result, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
        {
            if (result.Successful)
            {
                effect(cancellationToken);
            }

            return result;
        }
        /// <summary>
        /// Performs an effect if result is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="effect">The effect that should be performed if result is <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> EffectIfSuccessful<T>(this Result<T> result, Action<T, CancellationToken> effect,
            CancellationToken cancellationToken)
        {
            if (result.Successful)
            {
                effect(((Success<T>) result).Value, cancellationToken);
            }

            return result;
        }
        
        /// <summary>
        /// Performs an async effect if result is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="asyncEffect">The async effect that should be performed if result is <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Task"/> that returns <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T>> EffectIfSuccessfulAsync<T>(this Result<T> result,
            Func<Task> asyncEffect)
        {
            if (result.Successful)
            {
                await asyncEffect();
            }

            return result;
        }
        
        /// <summary>
        /// Performs an async effect if result is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="asyncEffect">The async effect that should be performed if result is <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Task"/> that returns <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T>> EffectIfSuccessfulAsync<T>(this Result<T> result,
            Func<CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
        {
            if (result.Successful)
            {
                await asyncEffect(cancellationToken);
            }

            return result;
        }
        
        /// <summary>
        /// Performs an async effect if result is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="asyncEffect">The async effect that should be performed if result is <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Task"/> that returns <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T>> EffectIfSuccessfulAsync<T>(this Result<T> result,
            Func<T, Task> asyncEffect)
        {
            if (result.Successful)
            {
                await asyncEffect(((Success<T>) result).Value);
            }

            return result;
        }
        
        /// <summary>
        /// Performs an async effect if result is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="asyncEffect">The async effect that should be performed if result is <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Task"/> that returns <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T>> EffectIfSuccessfulAsync<T>(this Result<T> result,
            Func<T, CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
        {
            if (result.Successful)
            {
                await asyncEffect(((Success<T>) result).Value, cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Performs an effect if result is not <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="effect">The effect that should be performed if result is not <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> EffectIfNotSuccessful<T>(this Result<T> result, Action effect)
        {
            if (!result.Successful)
            {
                effect();
            }

            return result;
        }
        
        /// <summary>
        /// Performs an effect if result is not <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="effect">The effect that should be performed if result is not <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> EffectIfNotSuccessful<T>(this Result<T> result, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                effect(cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Performs an effect if result is not <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="effect">The effect that should be performed if result is not <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> EffectIfNotSuccessful<T>(this Result<T> result, Action<Exception> effect)
        {
            if (!result.Successful)
            {
                effect(((Fail<T>) result).Exception);
            }

            return result;
        }

        /// <summary>
        /// Performs an effect if result is not <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="effect">The effect that should be performed if result is not <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> EffectIfNotSuccessful<T>(this Result<T> result,
            Action<Exception, CancellationToken> effect, CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                effect(((Fail<T>) result).Exception, cancellationToken);
            }

            return result;
        }
        
        /// <summary>
        /// Performs an async effect if result is not <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="asyncEffect">The async effect that should be performed if result is not <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Task"/> that returns <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T>> EffectIfNotSuccessfulAsync<T>(this Result<T> result,
            Func<Task> asyncEffect)
        {
            if (!result.Successful)
            {
                await asyncEffect();
            }

            return result;
        }
        
        /// <summary>
        /// Performs an async effect if result is not <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="asyncEffect">The async effect that should be performed if result is not <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Task"/> that returns <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T>> EffectIfNotSuccessfulAsync<T>(this Result<T> result,
            Func<CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                await asyncEffect(cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Performs an async effect if result is not <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="asyncEffect">The async effect that should be performed if result is not <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Task"/> that returns <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T>> EffectIfNotSuccessfulAsync<T>(this Result<T> result,
            Func<Exception, Task> asyncEffect)
        {
            if (!result.Successful)
            {
                await asyncEffect(((Fail<T>) result).Exception);
            }

            return result;
        }

        /// <summary>
        /// Performs an async effect if result is not <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="asyncEffect">The async effect that should be performed if result is not <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of result value.</typeparam>
        /// <returns>The <see cref="Task"/> that returns <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result<T>> EffectIfNotSuccessfulAsync<T>(this Result<T> result,
            Func<Exception, CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                await asyncEffect(((Fail<T>) result).Exception, cancellationToken);
            }

            return result;
        }
    }
}