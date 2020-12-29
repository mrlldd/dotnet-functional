using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using mrlldd.Functional.Result.Internal;
using mrlldd.Functional.Result.Internal.Utilities;

namespace mrlldd.Functional.Result.Extensions
{
    /// <summary>
    /// The class that contains extensions methods that bind between <see cref="Result{T}"/> and <see cref="Result"/>
    /// </summary>
    public static class SwitchingResultExtensions
    {
        /// <summary>
        /// Binds a generic result to another valueless result if it is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="effect">The effect that will be called if source result is <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of source result value.</typeparam>
        /// <returns>The <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result Bind<T>(this Result<T> result, Action effect)
            => result.Successful
                ? Execute.Safely(effect)
                : SwitchingFailFactory(result);

        /// <summary>
        /// Binds a generic result to another valueless result if it is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="effect">The effect that will be called if source result is <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of source result value.</typeparam>
        /// <returns>The <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result Bind<T>(this Result<T> result, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
            => result.Successful
                ? Execute.Safely(effect, cancellationToken)
                : SwitchingFailFactory(result);

        /// <summary>
        /// Binds a generic result to another valueless result if it is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="effect">The effect that will be called if source result is <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of source result value.</typeparam>
        /// <returns>The <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result Bind<T>(this Result<T> result, Action<T> effect)
            => result.Successful
                ? Execute.Safely(effect, ((Success<T>) result).Value)
                : SwitchingFailFactory(result);

        /// <summary>
        /// Binds a generic result to another valueless result if it is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="effect">The effect that will be called if source result is <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of source result value.</typeparam>
        /// <returns>The <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result Bind<T>(this Result<T> result, Action<T, CancellationToken> effect,
            CancellationToken cancellationToken)
            => result.Successful
                ? Execute.Safely(effect, ((Success<T>) result).Value, cancellationToken)
                : SwitchingFailFactory(result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Result SwitchingFailFactory<T>(Result<T> result)
            => new Fail(((Fail<T>) result).Exception);

        /// <summary>
        /// Binds a generic result to another valueless result task if it is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="asyncEffect">The async effect that will be called if source result is <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of source result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Result<T> result, Func<Task> asyncEffect)
            => result.Successful
                ? asyncEffect()
                    .ThenWrapAsResult()
                : FailFactory.ValuelessExceptionTask(((Fail<T>) result).Exception);

        /// <summary>
        /// Binds a generic result to another valueless result task if it is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="asyncEffect">The async effect that will be called if source result is <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of source result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Result<T> result, Func<CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => result.Successful
                ? asyncEffect(cancellationToken)
                    .ThenWrapAsResult()
                : FailFactory.ValuelessExceptionTask(((Fail<T>) result).Exception);

        /// <summary>
        /// Binds a generic result to another valueless result task if it is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="asyncEffect">The async effect that will be called if source result is <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of source result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Result<T> result, Func<T, Task> asyncEffect)
            => result.Successful
                ? asyncEffect(((Success<T>) result).Value)
                    .ThenWrapAsResult()
                : FailFactory.ValuelessExceptionTask(((Fail<T>) result).Exception);

        /// <summary>
        /// Binds a generic result to another valueless result task if it is <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="asyncEffect">The async effect that will be called if source result is <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T">The type of source result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Result<T> result, Func<T, CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => result.Successful
                ? asyncEffect(((Success<T>) result).Value, cancellationToken)
                    .ThenWrapAsResult()
                : FailFactory.ValuelessExceptionTask(((Fail<T>) result).Exception);

        /// <summary>
        /// Binds a valueless result to another generic result if it is <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="factory">The factory function that will be called if source result is <see cref="Result.Successful"/>.</param>
        /// <typeparam name="T">The type of result value that will be returned.</typeparam>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Bind<T>(this Result result, Func<T> factory)
            => result.Successful
                ? Execute.Safely(factory)
                : SwitchingFailFactory<T>(result);

        /// <summary>
        /// Binds a valueless result to another generic result if it is <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="factory">The factory function that will be called if source result is <see cref="Result.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of result value that will be returned.</typeparam>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> Bind<T>(this Result result, Func<CancellationToken, T> factory,
            CancellationToken cancellationToken)
            => result.Successful
                ? Execute.Safely(factory, cancellationToken)
                : SwitchingFailFactory<T>(result);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Result<T> SwitchingFailFactory<T>(Result result)
            => new Fail<T>(((Fail) result).Exception);


        /// <summary>
        /// Binds a valueless result to another generic result task if it is <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="asyncFactory">The async factory function that will be called if source result is <see cref="Result.Successful"/>.</param>
        /// <typeparam name="T">The type of result value that will be returned.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<T>> Bind<T>(this Result result, Func<Task<T>> asyncFactory)
            => result.Successful
                ? asyncFactory()
                    .ThenWrapAsResult()
                : FailResultTaskFactory<T>(result);

        /// <summary>
        /// Binds a valueless result to another generic result task if it is <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="result">The result that will be bound.</param>
        /// <param name="asyncFactory">The async factory function that will be called if source result is <see cref="Result.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of result value that will be returned.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result{T}"/>.</returns>
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
                        ? FailFactory.GenericCanceled<T>(task)
                        : new Success<T>(task.Result)
                    : FailFactory.GenericException<T>(task.Exception));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Task<Result<T>> FailResultTaskFactory<T>(Result result)
            => Task.FromResult<Result<T>>(new Fail<T>(((Fail) result).Exception));

        /// <summary>
        /// Binds a generic result task to another valueless result task if it will be <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="asyncEffect">The async effect that will be called if source task result is <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of source task result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask, Func<Task> asyncEffect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.ValuelessCanceledTask(task)
                        : task.Result.Bind(asyncEffect)
                    : FailFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        /// <summary>
        /// Binds a generic result task to another valueless result task if it will be <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="asyncEffect">The async effect that will be called if source task result is <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of source task result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask, Func<CancellationToken, Task> asyncEffect,
            CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.ValuelessCanceledTask(task)
                        : task.Result.Bind(asyncEffect, cancellationToken)
                    : FailFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        /// <summary>
        /// Binds a generic result task to another valueless result task if it will be <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="asyncEffect">The async effect that will be called if source task result is <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of source task result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask, Func<T, Task> asyncEffect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.ValuelessCanceledTask(task)
                        : task.Result.Bind(asyncEffect)
                    : FailFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        /// <summary>
        /// Binds a generic result task to another valueless result task if it will be <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="asyncEffect">The async effect that will be called if source task result is <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of source task result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask,
            Func<T, CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.ValuelessCanceledTask(task)
                        : task.Result.Bind(asyncEffect, cancellationToken)
                    : FailFactory.ValuelessExceptionTask(task.Exception))
                .Unwrap();

        /// <summary>
        /// Binds a valueless result task to another generic result task if it will be <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="asyncFactory">The async factory that will be called if source task result is <see cref="Result.Successful"/>.</param>
        /// <typeparam name="T">The type of returned result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<T>> Bind<T>(this Task<Result> sourceTask, Func<Task<T>> asyncFactory)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.GenericCanceledTask<T>(task)
                        : task.Result
                            .Bind(asyncFactory)
                    : FailFactory.GenericExceptionTask<T>(task.Exception))
                .Unwrap();

        /// <summary>
        /// Binds a valueless result task to another generic result task if it will be <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="asyncFactory">The async factory that will be called if source task result is <see cref="Result.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of returned result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<T>> Bind<T>(this Task<Result> sourceTask,
            Func<CancellationToken, Task<T>> asyncFactory, CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.GenericCanceledTask<T>(task)
                        : task.Result.Bind(asyncFactory, cancellationToken)
                    : FailFactory.GenericExceptionTask<T>(task.Exception))
                .Unwrap();

        /// <summary>
        /// Binds a generic result task to another valueless result if it will be <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="effect">The effect that will be called if source task result is <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of source task result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask,
            Action effect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.ValuelessCanceled(task)
                        : task.Result.Bind(effect)
                    : FailFactory.ValuelessException(task.Exception)
                );

        /// <summary>
        /// Binds a generic result task to another valueless result if it will be <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="effect">The effect that will be called if source task result is <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of source task result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask,
            Action<CancellationToken> effect, CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.ValuelessCanceled(task)
                        : task.Result.Bind(effect, cancellationToken)
                    : FailFactory.ValuelessException(task.Exception)
                );

        /// <summary>
        /// Binds a generic result task to another valueless result if it will be <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="effect">The effect that will be called if source task result is <see cref="Result{T}.Successful"/>.</param>
        /// <typeparam name="T">The type of source task result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask,
            Action<T> effect)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.ValuelessCanceled(task)
                        : task.Result.Bind(effect)
                    : FailFactory.ValuelessException(task.Exception)
                );

        /// <summary>
        /// Binds a generic result task to another valueless result if it will be <see cref="Result{T}.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="effect">The effect that will be called if source task result is <see cref="Result{T}.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of source task result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> Bind<T>(this Task<Result<T>> sourceTask,
            Action<T, CancellationToken> effect, CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.ValuelessCanceled(task)
                        : task.Result.Bind(effect, cancellationToken)
                    : FailFactory.ValuelessException(task.Exception)
                );

        /// <summary>
        /// Binds a valueless result task to another generic result if it will be <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="factory">The factory that will be called if source task result is <see cref="Result.Successful"/>.</param>
        /// <typeparam name="T">The type of source task result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<T>> Bind<T>(this Task<Result> sourceTask, Func<T> factory)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.GenericCanceled<T>(task)
                        : task.Result.Bind(factory)
                    : FailFactory.GenericException<T>(task.Exception)
                );

        /// <summary>
        /// Binds a valueless result task to another generic result if it will be <see cref="Result.Successful"/>.
        /// </summary>
        /// <param name="sourceTask">The result task that will be bound.</param>
        /// <param name="factory">The factory that will be called if source task result is <see cref="Result.Successful"/>.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of source task result value.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result{T}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<T>> Bind<T>(this Task<Result> sourceTask, Func<CancellationToken, T> factory,
            CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? FailFactory.GenericCanceled<T>(task)
                        : task.Result.Bind(factory, cancellationToken)
                    : FailFactory.GenericException<T>(task.Exception)
                );
    }
}