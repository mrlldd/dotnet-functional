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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result AsFail(this Exception exception)
            => new Fail(exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Success UnwrapAsSuccess(this Result result)
            => result.Successful
                ? (Success) result
                : throw new ResultUnwrapException("Result is not successful.");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Exception UnwrapAsFail(this Result result)
            => result;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result Bind(this Result result, Action effect)
            => result.Successful
                ? Execute.Safely(effect)
                : result;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result Bind(this Result result, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
            => result.Successful
                ? Execute.Safely(effect, cancellationToken)
                : result;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind(this Result result, Func<Task> asyncEffect)
            => result.Successful
                ? asyncEffect()
                    .ContinueWith(task => task.Exception == null
                        ? task.IsCanceled
                            ? ResultFactory.ValuelessCanceled(task)
                            : Result.Success
                        : ResultFactory.ValuelessException(task.Exception))
                : Task
                    .FromResult(result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind(this Result result, Func<CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => result.Successful
                ? asyncEffect(cancellationToken)
                    // ReSharper disable once MethodSupportsCancellation
                    .ContinueWith(task => task.Exception == null
                        ? task.IsCanceled
                            ? ResultFactory.ValuelessCanceled(task)
                            : Result.Success
                        : ResultFactory.ValuelessException(task.Exception))
                : Task
                    .FromResult(result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind(this Task<Result> sourceTask, Func<Task> asyncEffect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? ResultFactory.ValuelessCanceledTask(task)
                        : task.Result.Bind(asyncEffect)
                    : ResultFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind(this Task<Result> sourceTask, Func<CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? ResultFactory.ValuelessCanceledTask(task)
                        : task.Result.Bind(asyncEffect, cancellationToken)
                    : ResultFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind(this Task<Result> sourceTask, Action effect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? ResultFactory.ValuelessCanceled(task)
                        : task.Result.Bind(effect)
                    : ResultFactory.ValuelessException(task.Exception));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind(this Task<Result> sourceTask, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? ResultFactory.ValuelessCanceled(task)
                        : task.Result.Bind(effect, cancellationToken)
                    : ResultFactory.ValuelessException(task.Exception));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> ThenWrapAsResult(this Task sourceTask)
            => sourceTask
                .ContinueWith(task => task.Exception ?? (task.IsCanceled
                    ? ResultFactory.ValuelessCanceled(task)
                    : Result.Success)
                );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result EffectIfSuccessful(this Result result, Action effect)
        {
            if (result.Successful)
            {
                effect();
            }

            return result;
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result> EffectIfSuccessfulAsync(this Result result, Func<Task> effect)
        {
            if (result.Successful)
            {
                await effect();
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result> EffectIfSuccessfulAsync(this Result result,
            Func<CancellationToken, Task> effect, CancellationToken cancellationToken)
        {
            if (result.Successful)
            {
                await effect(cancellationToken);
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result EffectIfNotSuccessful(this Result result, Action effect)
        {
            if (!result.Successful)
            {
                effect();
            }

            return result;
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result EffectIfNotSuccessful(this Result result, Action<Exception> effect)
        {
            if (!result.Successful)
            {
                effect(((Fail) result).Exception);
            }

            return result;
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<Result> EffectIfNotSuccessfulAsync(this Result result, Func<Task> asyncEffect)
        {
            if (!result.Successful)
            {
                await asyncEffect();
            }

            return result;
        }

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