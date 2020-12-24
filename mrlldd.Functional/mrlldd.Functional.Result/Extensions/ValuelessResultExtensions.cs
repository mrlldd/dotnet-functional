using System;
using System.Threading;
using System.Threading.Tasks;
using mrlldd.Functional.Result.Exceptions;
using mrlldd.Functional.Result.Internal.Utilities;

namespace mrlldd.Functional.Result.Extensions
{
    public static class ValuelessResultExtensions
    {
        public static Result AsFail(this Exception exception)
            => new Fail(exception);

        public static Success UnwrapAsSuccess(this Result result)
            => result.Successful
                ? (Success) result
                : throw new ResultUnwrapException("Result is not successful.");

        public static Exception UnwrapAsFail(this Result result)
            => result;

        public static Result Bind(this Result result, Action effect)
            => result.Successful
                ? Execute.Safely(effect)
                : result;

        public static Result Bind(this Result result, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
            => result.Successful
                ? Execute.Safely(effect, cancellationToken)
                : result;

        public static Task<Result> Bind(this Result result, Func<Task> asyncEffect)
            => result.Successful
                ? asyncEffect()
                    .ContinueWith(task => task.Exception == null
                        ? task.IsCanceled ? new Fail(new AggregateException(new TaskCanceledException(task)))
                        : Result.Success
                        : new Fail(task.Exception))
                : Task
                    .FromResult(result);


        public static Task<Result> Bind(this Result result, Func<CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => result.Successful
                ? asyncEffect(cancellationToken)
                    // ReSharper disable once MethodSupportsCancellation
                    .ContinueWith<Result>(task => task.Exception == null
                        ? task.IsCanceled
                            ? new Fail(new AggregateException(new TaskCanceledException(task)))
                            : Result.Success
                        : new Fail(task.Exception))
                : Task
                    .FromResult(result);
        
        public static Task<Result> Bind(this Task sourceTask, Func<Task> asyncEffect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Task.FromResult<Result>(new Fail(new AggregateException(new TaskCanceledException(task))))
                        : asyncEffect().ThenWrapAsResult()
                    : Task
                        .FromResult<Result>(new Fail(task.Exception)))
                .Unwrap();

        public static Task<Result> Bind(this Task<Result> sourceTask, Func<Task> asyncEffect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Task.FromResult<Result>(new Fail(new AggregateException(new TaskCanceledException(task))))
                        : task.Result.Bind(asyncEffect)
                    : Task
                        .FromResult<Result>(new Fail(task.Exception)))
                .Unwrap();

        public static Task<Result> Bind(this Task<Result> sourceTask, Func<CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Task.FromResult<Result>(new Fail(new AggregateException(new TaskCanceledException(task))))
                        : task.Result.Bind(asyncEffect, cancellationToken)
                    : Task
                        .FromResult<Result>(new Fail(task.Exception)))
                .Unwrap();


        public static Task<Result> Bind(this Task<Result> sourceTask, Action effect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? new Fail(new AggregateException(new TaskCanceledException(task)))
                        : Execute.Safely(effect)
                    : new Fail(task.Exception));

        public static Task<Result> Bind(this Task<Result> sourceTask, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? new Fail(new AggregateException(new TaskCanceledException(task)))
                        : Execute.Safely(effect, cancellationToken)
                    : new Fail(task.Exception));

        public static Task<Result> ThenWrapAsResult(this Task sourceTask)
            => sourceTask
                .ContinueWith(task => task.Exception ?? (task.IsCanceled
                    ? new Fail(new AggregateException(new TaskCanceledException(task)))
                    : Result.Success)
                );

        public static Result SideEffectIfSuccessful(this Result result, Action effect)
        {
            if (result.Successful)
            {
                effect();
            }

            return result;
        }
        
        public static Result SideEffectIfSuccessful(this Result result, Action<CancellationToken> effect, CancellationToken cancellationToken)
        {
            if (result.Successful)
            {
                effect(cancellationToken);
            }

            return result;
        }
        
        public static async Task<Result> SideEffectIfSuccessfulAsync(this Result result, Func<Task> effect)
        {
            if (result.Successful)
            {
                await effect();
            }

            return result;
        }
        
        public static async Task<Result> SideEffectIfSuccessfulAsync(this Result result, Func<CancellationToken, Task> effect, CancellationToken cancellationToken)
        {
            if (result.Successful)
            {
                await effect(cancellationToken);
            }

            return result;
        }

        public static Result SideEffectIfNotSuccessful(this Result result, Action effect)
        {
            if (!result.Successful)
            {
                effect();
            }

            return result;
        }
        
        public static Result SideEffectIfNotSuccessful(this Result result, Action<CancellationToken> effect, CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                effect(cancellationToken);
            }

            return result;
        }
        
        public static Result SideEffectIfNotSuccessful(this Result result, Action<Exception> effect)
        {
            if (!result.Successful)
            {
                effect(((Fail) result).Exception);
            }

            return result;
        }

        public static Result SideEffectIfNotSuccessful(this Result result, Action<Exception, CancellationToken> effect, CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                effect(((Fail) result).Exception, cancellationToken);
            }

            return result;
        }
        
        public static async Task<Result> SideEffectIfNotSuccessfulAsync(this Result result, Func<Task> asyncEffect)
        {
            if (!result.Successful)
            {
                await asyncEffect();
            }

            return result;
        }
        
        public static async Task<Result> SideEffectIfNotSuccessfulAsync(this Result result, Func<CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                await asyncEffect(cancellationToken);
            }

            return result;
        }
        
        public static async Task<Result> SideEffectIfNotSuccessfulAsync(this Result result, Func<Exception, Task> asyncEffect)
        {
            if (!result.Successful)
            {
                await asyncEffect(((Fail) result).Exception);
            }

            return result;
        }
        
        public static async Task<Result> SideEffectIfNotSuccessfulAsync(this Result result, Func<Exception, CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                await asyncEffect(((Fail) result).Exception, cancellationToken);
            }

            return result;
        }
    }
}