using System;
using System.Threading;
using System.Threading.Tasks;
using mrlldd.Functional.Result.Internal.Utilities;

namespace mrlldd.Functional.Result.Extensions
{
    public static class SwitchingResultExtensions
    {
        public static Result Bind<T>(this Result<T> result, Action effect)
            => result.Successful
                ? Execute.Safely(effect)
                : FailResultFactory(result);

        public static Result Bind<T>(this Result<T> result, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
            => result.Successful
                ? Execute.Safely(effect, cancellationToken)
                : FailResultFactory(result);

        public static Result Bind<T>(this Result<T> result, Action<T> effect)
            => result.Successful
                ? Execute.Safely(effect, ((Success<T>) result).Value)
                : FailResultFactory(result);

        public static Result Bind<T>(this Result<T> result, Action<T, CancellationToken> effect,
            CancellationToken cancellationToken)
            => result.Successful
                ? Execute.Safely(effect, ((Success<T>) result).Value, cancellationToken)
                : FailResultFactory(result);

        private static Result FailResultFactory<T>(Result<T> result)
            => new Fail(((Fail<T>) result).Exception);

        public static Task<Result> Bind<T>(this Result<T> result, Func<Task> asyncEffect)
            => result.Successful
                ? asyncEffect()
                    .ThenWrapAsResult()
                : FailResultTaskFactory(result);

        public static Task<Result> Bind<T>(this Result<T> result, Func<CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => result.Successful
                ? asyncEffect(cancellationToken)
                    .ThenWrapAsResult()
                : FailResultTaskFactory(result);

        public static Task<Result> Bind<T>(this Result<T> result, Func<T, CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => result.Successful
                ? asyncEffect(((Success<T>) result).Value, cancellationToken)
                    .ThenWrapAsResult()
                : FailResultTaskFactory(result);

        private static Task<Result> ContinuationFactory(Task sourceTask)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? new Fail(new AggregateException(new TaskCanceledException(task)))
                        : Result.Success
                    : new Fail(task.Exception));

        private static Task<Result> FailResultTaskFactory<T>(Result<T> result)
            => Task.FromResult<Result>(new Fail(((Fail<T>) result).Exception));

        public static Result<T> Bind<T>(this Result result, Func<T> factory)
            => result.Successful
                ? Execute.Safely(factory)
                : FailResultFactory<T>(result);

        public static Result<T> Bind<T>(this Result result, Func<CancellationToken, T> factory,
            CancellationToken cancellationToken)
            => result.Successful
                ? Execute.Safely(factory, cancellationToken)
                : FailResultFactory<T>(result);

        private static Result<T> FailResultFactory<T>(Result result)
            => new Fail<T>(((Fail) result).Exception);

        public static Task<Result<T>> Bind<T>(this Result result, Func<Task<T>> asyncFactory)
            => result.Successful
                ? asyncFactory()
                    .ThenWrapAsResult()
                : FailResultTaskFactory<T>(result);

        public static Task<Result<T>> Bind<T>(this Result result, Func<CancellationToken, Task<T>> asyncFactory,
            CancellationToken cancellationToken)
            => result.Successful
                ? ContinuationFactory(asyncFactory(cancellationToken))
                : FailResultTaskFactory<T>(result);

        private static Task<Result<T>> ContinuationFactory<T>(Task<T> sourceTask)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? (Result<T>) new Fail<T>(new AggregateException(new TaskCanceledException(task)))
                        : new Success<T>(task.Result)
                    : new Fail<T>(task.Exception));

        private static Task<Result<T>> FailResultTaskFactory<T>(Result result)
            => Task.FromResult<Result<T>>(new Fail<T>(((Fail) result).Exception));

        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask, Func<Task> asyncEffect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? CanceledTaskResultFactory(task)
                        : task.Result.Successful
                            ? asyncEffect()
                                .ThenWrapAsResult()
                            : FailResultTaskFactory(task.Result)
                    : ExceptionFailResultTaskFactory(task.Exception))
                .Unwrap();

        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask, Func<CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? CanceledTaskResultFactory(task)
                        : task.Result.Successful
                            ? asyncEffect(cancellationToken)
                                .ThenWrapAsResult()
                            : FailResultTaskFactory(task.Result)
                    : ExceptionFailResultTaskFactory(task.Exception))
                .Unwrap();

        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask, Func<T, Task> asyncEffect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? CanceledTaskResultFactory(task)
                        : task.Result.Successful
                            ? asyncEffect(((Success<T>) task.Result).Value)
                                .ThenWrapAsResult()
                            : FailResultTaskFactory(task.Result)
                    : ExceptionFailResultTaskFactory(task.Exception))
                .Unwrap();

        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask,
            Func<T, CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? CanceledTaskResultFactory(task)
                        : task.Result.Successful
                            ? asyncEffect(((Success<T>) task.Result).Value, cancellationToken)
                                .ThenWrapAsResult()
                            : FailResultTaskFactory(task.Result)
                    : ExceptionFailResultTaskFactory(task.Exception))
                .Unwrap();

        private static Task<Result> CanceledTaskResultFactory(Task task)
            => Task.FromResult<Result>(new Fail(new AggregateException(new TaskCanceledException(task))));

        private static Task<Result> ExceptionFailResultTaskFactory(Exception exception)
            => Task.FromResult<Result>(new Fail(exception));

        public static Task<Result<T>> Bind<T>(this Task<Result> sourceTask, Func<Task<T>> asyncFactory)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? CanceledTaskResultFactory<T>(task)
                        : task.Result
                            .Bind(asyncFactory)
                    : ExceptionFailResultTaskFactory<T>(task.Exception))
                .Unwrap();
        
        public static Task<Result<T>> Bind<T>(this Task<Result> sourceTask, Func<CancellationToken, Task<T>> asyncFactory, CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? CanceledTaskResultFactory<T>(task)
                        : task.Result.Successful
                            ? ContinuationFactory(asyncFactory(cancellationToken))
                            : FailResultTaskFactory<T>(task.Result)
                    : ExceptionFailResultTaskFactory<T>(task.Exception))
                .Unwrap();
        
        private static Task<Result<T>> CanceledTaskResultFactory<T>(Task task)
            => Task
                .FromResult<Result<T>>(new Fail<T>(new AggregateException(new TaskCanceledException(task))));
        
        private static Task<Result<T>> ExceptionFailResultTaskFactory<T>(Exception exception)
            => Task.FromResult<Result<T>>(new Fail<T>(exception));
    }
}