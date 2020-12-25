using System;
using System.Threading;
using System.Threading.Tasks;
using mrlldd.Functional.Result.Internal;
using mrlldd.Functional.Result.Internal.Utilities;

namespace mrlldd.Functional.Result.Extensions
{
    public static class SwitchingResultExtensions
    {
        public static Result Bind<T>(this Result<T> result, Action effect)
            => result.Successful
                ? Execute.Safely(effect)
                : FailFactory(result);

        public static Result Bind<T>(this Result<T> result, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
            => result.Successful
                ? Execute.Safely(effect, cancellationToken)
                : FailFactory(result);

        public static Result Bind<T>(this Result<T> result, Action<T> effect)
            => result.Successful
                ? Execute.Safely(effect, ((Success<T>) result).Value)
                : FailFactory(result);

        public static Result Bind<T>(this Result<T> result, Action<T, CancellationToken> effect,
            CancellationToken cancellationToken)
            => result.Successful
                ? Execute.Safely(effect, ((Success<T>) result).Value, cancellationToken)
                : FailFactory(result);

        private static Result FailFactory<T>(Result<T> result)
            => new Fail(((Fail<T>) result).Exception);

        public static Task<Result> Bind<T>(this Result<T> result, Func<Task> asyncEffect)
            => result.Successful
                ? asyncEffect()
                    .ThenWrapAsResult()
                : ResultFactory.ValuelessExceptionTask(((Fail<T>) result).Exception);

        public static Task<Result> Bind<T>(this Result<T> result, Func<CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => result.Successful
                ? asyncEffect(cancellationToken)
                    .ThenWrapAsResult()
                : ResultFactory.ValuelessExceptionTask(((Fail<T>) result).Exception);

        public static Task<Result> Bind<T>(this Result<T> result, Func<T, CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => result.Successful
                ? asyncEffect(((Success<T>) result).Value, cancellationToken)
                    .ThenWrapAsResult()
                : ResultFactory.ValuelessExceptionTask(((Fail<T>) result).Exception);

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
                        ? ResultFactory.GenericCanceled<T>(task)
                        : new Success<T>(task.Result)
                    : ResultFactory.GenericException<T>(task.Exception));

        private static Task<Result<T>> FailResultTaskFactory<T>(Result result)
            => Task.FromResult<Result<T>>(new Fail<T>(((Fail) result).Exception));

        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask, Func<Task> asyncEffect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? ResultFactory.ValuelessCanceledTask(task)
                        : task.Result.Bind(asyncEffect)
                    : ResultFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask, Func<CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? ResultFactory.ValuelessCanceledTask(task)
                        : task.Result.Bind(asyncEffect, cancellationToken)
                    : ResultFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask, Func<T, Task> asyncEffect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? ResultFactory.ValuelessCanceledTask(task)
                        : task.Result.Successful
                            ? asyncEffect(((Success<T>) task.Result).Value)
                                .ThenWrapAsResult()
                            : ResultFactory.ValuelessExceptionTask(((Fail<T>) task.Result).Exception)
                    : ResultFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask,
            Func<T, CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? ResultFactory.ValuelessCanceledTask(task)
                        : task.Result.Bind(asyncEffect, cancellationToken)
                    : ResultFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        public static Task<Result<T>> Bind<T>(this Task<Result> sourceTask, Func<Task<T>> asyncFactory)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? ResultFactory.GenericCanceledTask<T>(task)
                        : task.Result
                            .Bind(asyncFactory)
                    : ResultFactory.GenericExceptionTask<T>(task.Exception))
                .Unwrap();
        
        public static Task<Result<T>> Bind<T>(this Task<Result> sourceTask, Func<CancellationToken, Task<T>> asyncFactory, CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? ResultFactory.GenericCanceledTask<T>(task)
                        : task.Result.Bind(asyncFactory, cancellationToken)
                    : ResultFactory.GenericExceptionTask<T>(task.Exception))
                .Unwrap();
    }
}