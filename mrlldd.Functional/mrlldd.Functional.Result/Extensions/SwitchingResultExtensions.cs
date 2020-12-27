using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using mrlldd.Functional.Result.Internal;
using mrlldd.Functional.Result.Internal.Utilities;

namespace mrlldd.Functional.Result.Extensions
{
    public static class SwitchingResultExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result Bind<T>(this Result<T> result, Action effect)
            => result.Successful
                ? Execute.Safely(effect)
                : FailFactory(result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result Bind<T>(this Result<T> result, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
            => result.Successful
                ? Execute.Safely(effect, cancellationToken)
                : FailFactory(result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result Bind<T>(this Result<T> result, Action<T> effect)
            => result.Successful
                ? Execute.Safely(effect, ((Success<T>) result).Value)
                : FailFactory(result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result Bind<T>(this Result<T> result, Action<T, CancellationToken> effect,
            CancellationToken cancellationToken)
            => result.Successful
                ? Execute.Safely(effect, ((Success<T>) result).Value, cancellationToken)
                : FailFactory(result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Result FailFactory<T>(Result<T> result)
            => new Fail(((Fail<T>) result).Exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Result<T> result, Func<Task> asyncEffect)
            => result.Successful
                ? asyncEffect()
                    .ThenWrapAsResult()
                : Internal.FailFactory.ValuelessExceptionTask(((Fail<T>) result).Exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Result<T> result, Func<CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => result.Successful
                ? asyncEffect(cancellationToken)
                    .ThenWrapAsResult()
                : Internal.FailFactory.ValuelessExceptionTask(((Fail<T>) result).Exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Result<T> result, Func<T, Task> asyncEffect)
            => result.Successful
                ? asyncEffect(((Success<T>) result).Value)
                    .ThenWrapAsResult()
                : Internal.FailFactory.ValuelessExceptionTask(((Fail<T>) result).Exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Result<T> result, Func<T, CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => result.Successful
                ? asyncEffect(((Success<T>) result).Value, cancellationToken)
                    .ThenWrapAsResult()
                : Internal.FailFactory.ValuelessExceptionTask(((Fail<T>) result).Exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Bind<T>(this Result result, Func<T> factory)
            => result.Successful
                ? Execute.Safely(factory)
                : FailResultFactory<T>(result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Bind<T>(this Result result, Func<CancellationToken, T> factory,
            CancellationToken cancellationToken)
            => result.Successful
                ? Execute.Safely(factory, cancellationToken)
                : FailResultFactory<T>(result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Result<T> FailResultFactory<T>(Result result)
            => new Fail<T>(((Fail) result).Exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<T>> Bind<T>(this Result result, Func<Task<T>> asyncFactory)
            => result.Successful
                ? asyncFactory()
                    .ThenWrapAsResult()
                : FailResultTaskFactory<T>(result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<T>> Bind<T>(this Result result, Func<CancellationToken, Task<T>> asyncFactory,
            CancellationToken cancellationToken)
            => result.Successful
                ? ContinuationFactory(asyncFactory(cancellationToken))
                : FailResultTaskFactory<T>(result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Task<Result<T>> ContinuationFactory<T>(Task<T> sourceTask)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Internal.FailFactory.GenericCanceled<T>(task)
                        : new Success<T>(task.Result)
                    : Internal.FailFactory.GenericException<T>(task.Exception));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Task<Result<T>> FailResultTaskFactory<T>(Result result)
            => Task.FromResult<Result<T>>(new Fail<T>(((Fail) result).Exception));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask, Func<Task> asyncEffect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Internal.FailFactory.ValuelessCanceledTask(task)
                        : task.Result.Bind(asyncEffect)
                    : Internal.FailFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask, Func<CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Internal.FailFactory.ValuelessCanceledTask(task)
                        : task.Result.Bind(asyncEffect, cancellationToken)
                    : Internal.FailFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask, Func<T, Task> asyncEffect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Internal.FailFactory.ValuelessCanceledTask(task)
                        : task.Result.Bind(asyncEffect)
                    : Internal.FailFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask,
            Func<T, CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Internal.FailFactory.ValuelessCanceledTask(task)
                        : task.Result.Bind(asyncEffect, cancellationToken)
                    : Internal.FailFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<T>> Bind<T>(this Task<Result> sourceTask, Func<Task<T>> asyncFactory)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Internal.FailFactory.GenericCanceledTask<T>(task)
                        : task.Result
                            .Bind(asyncFactory)
                    : Internal.FailFactory.GenericExceptionTask<T>(task.Exception))
                .Unwrap();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<T>> Bind<T>(this Task<Result> sourceTask,
            Func<CancellationToken, Task<T>> asyncFactory, CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Internal.FailFactory.GenericCanceledTask<T>(task)
                        : task.Result.Bind(asyncFactory, cancellationToken)
                    : Internal.FailFactory.GenericExceptionTask<T>(task.Exception))
                .Unwrap();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask,
            Action effect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Internal.FailFactory.ValuelessCanceled(task)
                        : task.Result.Bind(effect)
                    : Internal.FailFactory.ValuelessException(task.Exception)
                );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask,
            Action<CancellationToken> effect, CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Internal.FailFactory.ValuelessCanceled(task)
                        : task.Result.Bind(effect, cancellationToken)
                    : Internal.FailFactory.ValuelessException(task.Exception)
                );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask,
            Action<T> effect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Internal.FailFactory.ValuelessCanceled(task)
                        : task.Result.Bind(effect)
                    : Internal.FailFactory.ValuelessException(task.Exception)
                );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask,
            Action<T, CancellationToken> effect, CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Internal.FailFactory.ValuelessCanceled(task)
                        : task.Result.Bind(effect, cancellationToken)
                    : Internal.FailFactory.ValuelessException(task.Exception)
                );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<T>> Bind<T>(this Task<Result> sourceTask, Func<T> factory)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Internal.FailFactory.GenericCanceled<T>(task)
                        : task.Result.Bind(factory)
                    : Internal.FailFactory.GenericException<T>(task.Exception)
                );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<T>> Bind<T>(this Task<Result> sourceTask, Func<CancellationToken, T> factory,
            CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? Internal.FailFactory.GenericCanceled<T>(task)
                        : task.Result.Bind(factory, cancellationToken)
                    : Internal.FailFactory.GenericException<T>(task.Exception)
                );
    }
}