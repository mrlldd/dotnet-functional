using System;
using System.Threading;
using System.Threading.Tasks;
using mrlldd.Functional.Result.Internal.Utilities;

namespace mrlldd.Functional.Result.Extensions
{
    public static class GenericResultExtensions
    {
        public static Result<T> AsSuccess<T>(this T obj) => new Success<T>(obj);

        public static Result<T> AsFail<T>(this Exception exception) => new Fail<T>(exception);

        public static T UnwrapAsSuccess<T>(this Result<T> result) => result;

        public static Exception UnwrapAsFail<T>(this Result<T> result) => result;

        public static Result<TMapped> Bind<T, TMapped>(this Result<T> source, Func<T, TMapped> mapper)
            => source.Successful
                ? Execute.Safely(((Success<T>)source).Value, mapper)
                : new Fail<TMapped>((Fail<T>)source);

        public static Result<TMapped> Bind<T, TMapped>(this Result<T> source,
            Func<T, CancellationToken, TMapped> mapper, CancellationToken cancellationToken = default)
            => source.Successful
                ? Execute.Safely<T, TMapped>(source, mapper, cancellationToken)
                : ((Fail<T>) source).Exception;

        public static Task<Result<TMapped>> Bind<T, TMapped>(this Result<T> source, Func<T, Task<TMapped>> asyncMapper)
            => source.Successful
                ? asyncMapper(source)
                    .ContinueWith(task => task.Exception ?? task.Result.AsSuccess())
                : Task
                    .FromResult<Result<TMapped>>(((Fail<T>) source).Exception);

        public static Task<Result<TMapped>> Bind<T, TMapped>(this Result<T> source,
            Func<T, CancellationToken, Task<TMapped>> asyncMapper, CancellationToken cancellationToken)
            => source.Successful
                ? asyncMapper(source, cancellationToken)
                    // ReSharper disable once MethodSupportsCancellation
                    .ContinueWith(task =>
                        task.Exception == null
                            ? task.IsCanceled
                                ? new AggregateException(new TaskCanceledException(task))
                                : task.Result.AsSuccess()
                            : task.Exception.AsFail<TMapped>())
                : Task
                    .FromResult<Result<TMapped>>(((Fail<T>) source).Exception);

        public static Task<Result<TMapped>> Bind<T, TMapped>(this Task<Result<T>> sourceTask,
            Func<T, Task<TMapped>> asyncMapper)
            => sourceTask
                .ContinueWith(task
                    => task.Exception == null
                        ? task.IsCanceled
                            ? Task.FromResult(
                                new AggregateException(new TaskCanceledException(sourceTask))
                                    .AsFail<TMapped>())
                            : task.Result
                                .Bind(asyncMapper)
                        : Task
                            .FromResult<Result<TMapped>>(task.Exception))
                .Unwrap();

        public static Task<Result<TMapped>> Bind<T, TMapped>(this Task<Result<T>> sourceTask,
            Func<T, CancellationToken, Task<TMapped>> asyncMapper, CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task
                    => task.Exception == null
                        ? task.Result
                            .Bind(asyncMapper, cancellationToken)
                        : Task
                            .FromResult<Result<TMapped>>(task.Exception), cancellationToken)
                .Unwrap();

        public static Task<Result<TMapped>> Bind<T, TMapped>(this Task<Result<T>> sourceTask,
            Func<T, TMapped> mapper)
            => sourceTask
                .ContinueWith(task => task.Exception ?? task.Result.Bind(mapper));

        public static Task<Result<TMapped>> Bind<T, TMapped>(this Task<Result<T>> sourceTask,
            Func<T, CancellationToken, TMapped> mapper, CancellationToken cancellationToken)
            => sourceTask
                // ReSharper disable once MethodSupportsCancellation
                .ContinueWith(task => task.Exception ?? task.Result.Bind(mapper, cancellationToken));

        public static Task<Result<T>> ThenWrapAsResult<T>(this Task<T> source)
            => source
                .ContinueWith(task => task.Exception ?? (task.IsCanceled
                    ? new AggregateException(new TaskCanceledException(task))
                    : task.Result.AsSuccess())
                );

        public static void SideEffectIfSuccessful<T>(this Result<T> result, Action effect)
        {
            if (result.Successful)
            {
                effect();
            }
        }

        public static Result<T> SideEffectIfSuccessful<T>(this Result<T> result, Action<T> effect)
        {
            if (result.Successful)
            {
                effect(((Success<T>) result).Value);
            }

            return result;
        }

        public static async Task<Result<T>> SideEffectIfSuccessfulAsync<T>(this Result<T> result, Func<Task> asyncEffect)
        {
            if (result.Successful)
            {
                await asyncEffect();
            }
            
            return result;
        }

        public static async Task<Result<T>> SideEffectIfSuccessfulAsync<T>(this Result<T> result, Func<T, Task> asyncEffect)
        {
            if (result.Successful)
            {
                await asyncEffect(((Success<T>) result).Value);
            }

            return result;
        }

        public static Result<T> SideEffectIfNotSuccessful<T>(this Result<T> result, Action effect)
        {
            if (!result.Successful)
            {
                effect();
            }

            return result;
        }

        public static Result<T> SideEffectIfNotSuccessful<T>(this Result<T> result, Action<Exception> effect)
        {
            if (!result.Successful)
            {
                effect(((Fail<T>) result).Exception);
            }

            return result;
        }
        
        public static async Task<Result<T>> SideEffectIfNotSuccessfulAsync<T>(this Result<T> result, Func<Task> asyncEffect)
        {
            if (!result.Successful)
            {
                await asyncEffect();
            }

            return result;
        }

        public static async Task<Result<T>> SideEffectIfNotSuccessfulAsync<T>(this Result<T> result, Func<Exception, Task> asyncEffect)
        {
            if (!result.Successful)
            {
                await asyncEffect(((Fail<T>) result).Exception);
            }

            return result;
        }
    }
}