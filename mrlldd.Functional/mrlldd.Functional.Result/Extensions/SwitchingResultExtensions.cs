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
                ? ContinuationFactory(asyncEffect())
                : FailResultTaskFactory(result);

        public static Task<Result> Bind<T>(this Result<T> result, Func<CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => result.Successful
                ? ContinuationFactory(asyncEffect(cancellationToken))
                : FailResultTaskFactory(result);

        public static Task<Result> Bind<T>(this Result<T> result, Func<T, CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => result.Successful
                ? ContinuationFactory(asyncEffect(((Success<T>) result).Value, cancellationToken))
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
                ? ContinuationFactory(asyncFactory())
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
            => Task.FromException<Result<T>>(((Fail) result).Exception);
        
        // todo binders from task to task
    }
}