using System;
using System.Threading;
using System.Threading.Tasks;
using mrlldd.Functional.Result.Exceptions;

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
                ? ExecuteSafely<T, TMapped>(source, mapper)
                : new Fail<TMapped>(source);

        public static Result<TMapped> Bind<T, TMapped>(this Result<T> source,
            Func<T, CancellationToken, TMapped> mapper, CancellationToken cancellationToken = default)
            => source.Successful
                ? ExecuteSafely<T, TMapped>(source, mapper, cancellationToken)
                : ((Fail<T>) source).Exception;

        private static Result<TMapped> ExecuteSafely<T, TMapped>(T source, Func<T, TMapped> mapper)
        {
            try
            {
                return mapper(source);
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private static Result<TMapped> ExecuteSafely<T, TMapped>(T source, Func<T, CancellationToken, TMapped> mapper,
            CancellationToken cancellationToken)
        {
            try
            {
                return mapper(source, cancellationToken);
            }
            catch (Exception e)
            {
                return e;
            }
        }

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
    }

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
                ? ExecuteSafely(effect)
                : result;

        private static Result ExecuteSafely(Action effect)
        {
            try
            {
                effect();
                return Result.Success;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        public static Result Bind(this Result result, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
            => result.Successful
                ? ExecuteSafely(effect, cancellationToken)
                : result;

        private static Result ExecuteSafely(Action<CancellationToken> effect, CancellationToken cancellationToken)
        {
            try
            {
                effect(cancellationToken);
                return Result.Success;
            }
            catch (Exception e)
            {
                return e;
            }
        }

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
                        : ExecuteSafely(effect)
                    : new Fail(task.Exception));

        public static Task<Result> Bind(this Task<Result> sourceTask, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? new Fail(new AggregateException(new TaskCanceledException(task)))
                        : ExecuteSafely(effect, cancellationToken)
                    : new Fail(task.Exception));

        public static Task<Result> ThenWrapAsResult(this Task sourceTask)
            => sourceTask
                .ContinueWith(task => task.Exception ?? (task.IsCanceled
                    ? new Fail(new AggregateException(new TaskCanceledException(task)))
                    : Result.Success)
                );
        
        
        /*public static Task<Result<T>> Bind<T>(this Result result, Func<Task<T>> asyncEffect)
            => result.Successful
                ? asyncEffect()
                    .ContinueWith(new Func<Task<T>, Result<T>>(task => task.Exception == null
                        ? task.IsCanceled
                            ? new Fail<T>(new AggregateException(new TaskCanceledException(task)))
                            : new Success<T>(task.Result)
                        : new Fail<T>(task.Exception)))
                : Task
                    .FromResult(result.UnwrapAsFail().AsFail<T>());*/
    }
}