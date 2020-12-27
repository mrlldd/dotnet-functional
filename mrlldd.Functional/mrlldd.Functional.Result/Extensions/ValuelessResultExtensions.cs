using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using mrlldd.Functional.Result.Exceptions;
using mrlldd.Functional.Result.Internal;
using mrlldd.Functional.Result.Internal.Utilities;

namespace mrlldd.Functional.Result.Extensions
{
    public static class ValuelessResultExtensions
    {
        /// <summary>
        /// Wraps an exception to fail result type.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when exception is null.</exception>
        /// <returns>The <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result AsFail(this Exception exception)
            => new Fail(exception);

        /// <summary>
        /// Unwraps result as success.
        /// </summary>
        /// <param name="result">The result that will be unwrapped.</param>
        /// <exception cref="ResultUnwrapException">Thrown when result is not <see cref="Result.Successful"/></exception>
        /// <returns>The <see cref="Success"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Success UnwrapAsSuccess(this Result result)
            => result.Successful
                ? (Success) result
                : throw new ResultUnwrapException("Result is not successful.");

        /// <summary>
        /// Unwraps result as fail.
        /// </summary>
        /// <param name="result">The result that will be unwrapped.</param>
        /// <exception cref="ResultUnwrapException">Thrown when result is <see cref="Result.Successful"/></exception>
        /// <returns>The <see cref="Exception"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Exception UnwrapAsFail(this Result result)
            => result;

        /// <summary>
        /// Binds a result to another result if it is <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="effect">The effect that will be called if source result is <see cref="Result.Successful"/>.</param>
        /// <returns>The <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result Bind(this Result result, Action effect)
            => result.Successful
                ? Execute.Safely(effect)
                : result;

        /// <summary>
        /// Binds a result to another result if it is <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="effect">The effect that will be called if source result is <see cref="Result.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result Bind(this Result result, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
            => result.Successful
                ? Execute.Safely(effect, cancellationToken)
                : result;

        /// <summary>
        /// Binds a result to another result task if it is <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="asyncEffect">The async effect that will be called if source result is <see cref="Result.Successful"/>.</param>
        /// <returns>The <see cref="Task{T}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind(this Result result, Func<Task> asyncEffect)
            => result.Successful
                ? asyncEffect()
                    .ContinueWith(task => task.Exception == null
                        ? task.IsCanceled
                            ? FailFactory.ValuelessCanceled(task)
                            : Result.Success
                        : FailFactory.ValuelessException(task.Exception))
                : Task
                    .FromResult(result);

        /// <summary>
        /// Binds a result to another result task if it is <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="asyncEffect">The async effect that will be called if source result is <see cref="Result.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task{T}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind(this Result result, Func<CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => result.Successful
                ? asyncEffect(cancellationToken)
                    // ReSharper disable once MethodSupportsCancellation
                    .ContinueWith(task => task.Exception == null
                        ? task.IsCanceled
                            ? FailFactory.ValuelessCanceled(task)
                            : Result.Success
                        : FailFactory.ValuelessException(task.Exception))
                : Task
                    .FromResult(result);

        /// <summary>
        /// Binds a result task to another result task if it will be <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="asyncEffect">The async effect that will be called if source task result is <see cref="Result.Successful"/>.</param>
        /// <returns>The <see cref="Task{T}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind(this Task<Result> sourceTask, Func<Task> asyncEffect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.ValuelessCanceledTask(task)
                        : task.Result.Bind(asyncEffect)
                    : FailFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        /// <summary>
        /// Binds a result task to another result task if it will be <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="asyncEffect">The async effect that will be called if source task result is <see cref="Result.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task{T}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind(this Task<Result> sourceTask, Func<CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.ValuelessCanceledTask(task)
                        : task.Result.Bind(asyncEffect, cancellationToken)
                    : FailFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        /// <summary>
        /// Binds a result task to another result if it will be <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="effect">The effect that will be called if source task result is <see cref="Result.Successful"/>.</param>
        /// <returns>The <see cref="Task{T}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind(this Task<Result> sourceTask, Action effect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.ValuelessCanceled(task)
                        : task.Result.Bind(effect)
                    : FailFactory.ValuelessException(task.Exception));

        /// <summary>
        /// Binds a result task to another result if it will be <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="effect">The effect that will be called if source task result is <see cref="Result.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task{T}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind(this Task<Result> sourceTask, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.ValuelessCanceled(task)
                        : task.Result.Bind(effect, cancellationToken)
                    : FailFactory.ValuelessException(task.Exception));

        /// <summary>
        /// Wraps a task result to <see cref="Result"/> type. 
        /// </summary>
        /// <param name="sourceTask">The result task that will be wrapped.</param>
        /// <returns>The <see cref="Task{T}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> ThenWrapAsResult(this Task sourceTask)
            => sourceTask
                .ContinueWith(task => task.Exception ?? (task.IsCanceled
                    ? FailFactory.ValuelessCanceled(task)
                    : Result.Success)
                );

        /// <summary>
        /// Performs an effect if result is <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="effect">The effect that should be performed if result is <see cref="Result.Successful"/>.</param>
        /// <returns>The <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result EffectIfSuccessful(this Result result, Action effect)
        {
            if (result.Successful)
            {
                effect();
            }

            return result;
        }

        /// <summary>
        /// Performs an effect if result is <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="effect">The effect that should be performed if result is <see cref="Result.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result EffectIfSuccessful(this Result result, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
        {
            if (result.Successful)
            {
                effect(cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Performs an async effect if result is <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="asyncEffect">The async effect that should be performed if result is <see cref="Result.Successful"/>.</param>
        /// <returns>The <see cref="Task{T}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result> EffectIfSuccessfulAsync(this Result result, Func<Task> asyncEffect)
        {
            if (result.Successful)
            {
                await asyncEffect();
            }

            return result;
        }

        /// <summary>
        /// Performs an async effect if result is <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="asyncEffect">The async effect that should be performed if result is <see cref="Result.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task{T}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result> EffectIfSuccessfulAsync(this Result result,
            Func<CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
        {
            if (result.Successful)
            {
                await asyncEffect(cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Performs an effect if result is not <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="effect">The effect that should be performed if result is not <see cref="Result.Successful"/>.</param>
        /// <returns>The <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result EffectIfNotSuccessful(this Result result, Action effect)
        {
            if (!result.Successful)
            {
                effect();
            }

            return result;
        }

        /// <summary>
        /// Performs an effect if result is not <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="effect">The effect that should be performed if result is not <see cref="Result.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result EffectIfNotSuccessful(this Result result, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                effect(cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Performs an effect if result is not <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="effect">The effect that should be performed if result is not <see cref="Result.Successful"/>.</param>
        /// <returns>The <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result EffectIfNotSuccessful(this Result result, Action<Exception> effect)
        {
            if (!result.Successful)
            {
                effect(((Fail) result).Exception);
            }

            return result;
        }

        /// <summary>
        /// Performs an effect if result is not <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="effect">The effect that should be performed if result is not <see cref="Result.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result EffectIfNotSuccessful(this Result result, Action<Exception, CancellationToken> effect,
            CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                effect(((Fail) result).Exception, cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Performs an async effect if result is not <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="asyncEffect">The async effect that should be performed if result is not <see cref="Result.Successful"/>.</param>
        /// <returns>The <see cref="Task{T}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result> EffectIfNotSuccessfulAsync(this Result result, Func<Task> asyncEffect)
        {
            if (!result.Successful)
            {
                await asyncEffect();
            }

            return result;
        }

        /// <summary>
        /// Performs an async effect if result is not <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="asyncEffect">The async effect that should be performed if result is not <see cref="Result.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task{T}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result> EffectIfNotSuccessfulAsync(this Result result,
            Func<CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                await asyncEffect(cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Performs an async effect if result is not <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="asyncEffect">The async effect that should be performed if result is not <see cref="Result.Successful"/>.</param>
        /// <returns>The <see cref="Task{T}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result> EffectIfNotSuccessfulAsync(this Result result,
            Func<Exception, Task> asyncEffect)
        {
            if (!result.Successful)
            {
                await asyncEffect(((Fail) result).Exception);
            }

            return result;
        }

        /// <summary>
        /// Performs an async effect if result is not <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="asyncEffect">The async effect that should be performed if result is not <see cref="Result.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task{T}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result> EffectIfNotSuccessfulAsync(this Result result,
            Func<Exception, CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                await asyncEffect(((Fail) result).Exception, cancellationToken);
            }

            return result;
        }
    }
}