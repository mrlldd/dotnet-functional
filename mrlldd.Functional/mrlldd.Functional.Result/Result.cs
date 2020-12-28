using System;
using System.Threading;
using System.Threading.Tasks;
using mrlldd.Functional.Result.Exceptions;
using mrlldd.Functional.Result.Extensions;
using mrlldd.Functional.Result.Internal.Utilities;

namespace mrlldd.Functional.Result
{
    /// <summary>
    /// The class that represents a valueless result.
    /// </summary>
    public abstract class Result : IHasSuccessState
    {
        /// <inheritdoc />
        public abstract bool Successful { get; }

        /// <inheritdoc />
        public override string ToString()
            => $"Success: {Successful}";

        /// <value>
        /// The singleton instance of success result type.
        /// </value>
        public static Result Success { get; } = new Success();

        /// <summary>
        /// Executes the effect and returns
        /// <see cref="mrlldd.Functional.Result.Success"/> if no exception was thrown or
        /// <see cref="mrlldd.Functional.Result.Fail"/> if exception was thrown.
        /// </summary>
        /// <param name="effect">The effect that will be executed.</param>
        /// <returns>The <see cref="Result"/>.</returns>
        public static Result Of(Action effect)
            => Execute.Safely(effect);

        /// <summary>
        /// Executes the effect and returns
        /// <see cref="mrlldd.Functional.Result.Success"/> if no exception was thrown or
        /// <see cref="mrlldd.Functional.Result.Fail"/> if exception was thrown.
        /// </summary>
        /// <param name="effect">The effect that will be executed.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Result"/>.</returns>
        public static Result Of(Action<CancellationToken> effect, CancellationToken cancellationToken)
            => Execute.Safely(effect, cancellationToken);

        /// <summary>
        /// Executes the async effect and returns
        /// <see cref="mrlldd.Functional.Result.Success"/> if no exception was thrown or
        /// <see cref="mrlldd.Functional.Result.Fail"/> if exception was thrown.
        /// </summary>
        /// <param name="asyncEffect">The async effect that will be executed.</param>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result"/>.</returns>
        public static Task<Result> Of(Func<Task> asyncEffect)
            => asyncEffect().ThenWrapAsResult();

        /// <summary>
        ///  Executes the async effect and returns
        /// <see cref="mrlldd.Functional.Result.Success"/> if no exception was thrown or
        /// <see cref="mrlldd.Functional.Result.Fail"/> if exception was thrown.
        /// </summary>
        /// <param name="asyncEffect">The async effect that will be executed.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result"/>.</returns>
        public static Task<Result> Of(Func<CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
            => asyncEffect(cancellationToken).ThenWrapAsResult();

        /// <summary>
        /// Executes the factory and returns
        /// <see cref="mrlldd.Functional.Result.Success{T}"/> if no exception was thrown or
        /// <see cref="mrlldd.Functional.Result.Fail{T}"/> if exception was thrown.
        /// </summary>
        /// <param name="factory">The factory that will be executed.</param>
        /// <typeparam name="T">The type of value that factory returns.</typeparam>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        public static Result<T> Of<T>(Func<T> factory)
            => Execute.Safely(factory);

        /// <summary>
        /// Executes the factory and returns
        /// <see cref="mrlldd.Functional.Result.Success{T}"/> if no exception was thrown or
        /// <see cref="mrlldd.Functional.Result.Fail{T}"/> if exception was thrown.
        /// </summary>
        /// <param name="factory">The factory that will be executed.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of value that factory returns.</typeparam>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        public static Result<T> Of<T>(Func<CancellationToken, T> factory, CancellationToken cancellationToken)
            => Execute.Safely(factory, cancellationToken);

        /// <summary>
        /// Executes the async factory and returns
        /// <see cref="mrlldd.Functional.Result.Success{T}"/> if no exception was thrown or
        /// <see cref="mrlldd.Functional.Result.Fail{T}"/> if exception was thrown.
        /// </summary>
        /// <param name="asyncFactory">The async factory that will be executed.</param>
        /// <typeparam name="T">The type of value that async factory returns.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result{T}"/>.</returns>
        public static Task<Result<T>> Of<T>(Func<Task<T>> asyncFactory)
            => asyncFactory().ThenWrapAsResult();

        /// <summary>
        /// Executes the async factory and returns
        /// <see cref="mrlldd.Functional.Result.Success{T}"/> if no exception was thrown or
        /// <see cref="mrlldd.Functional.Result.Fail{T}"/> if exception was thrown.
        /// </summary>
        /// <param name="asyncFactory">The async factory that will be executed.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <typeparam name="T">The type of value that async factory returns.</typeparam>
        /// <returns>The <see cref="Task{TResult}"/> that returns <see cref="Result{T}"/>.</returns>
        public static Task<Result<T>> Of<T>(Func<CancellationToken, Task<T>> asyncFactory,
            CancellationToken cancellationToken)
            => asyncFactory(cancellationToken).ThenWrapAsResult();
        /// <summary>
        /// The operator that implicitly wraps <see cref="Exception"/> to <see cref="Result"/>.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">Throw when exception is null.</exception>
        /// <returns>The <see cref="Result"/>.</returns>
        public static implicit operator Result(Exception exception)
            => new Fail(exception);

        /// <summary>
        /// The operator that implicitly unwraps <see cref="Result"/> to <see cref="Exception"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>The <see cref="Exception"/>.</returns>
        public static implicit operator Exception(Result result)
            => result.Successful
                ? throw new ResultUnwrapException("Can't extract exception from result as it's successful.")
                : ((Fail) result).Exception;
    }

    /// <summary>
    /// The class that represents generic result.
    /// </summary>
    /// <typeparam name="T">The type of wrapped value.</typeparam>
    public abstract class Result<T> : IHasSuccessState
    {
        /// <inheritdoc />
        public abstract bool Successful { get; }

        /// <inheritdoc />
        public override string ToString()
            => $"Success: {Successful}, ";

        /// <summary>
        /// The operator that implicitly unwraps the <see cref="Result{T}"/> to <see cref="T"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <exception cref="ResultUnwrapException">Thrown when passed result is not <see cref="Successful"/>.</exception>
        /// <returns>The <see cref="T"/>.</returns>
        public static implicit operator T(Result<T> result)
            => result.Successful
                ? ((Success<T>) result).Value
                : throw new ResultUnwrapException("Can't extract value from result as it's not successful.");

        /// <summary>
        /// The operator that implicitly unwraps the <see cref="Result{T}"/> to <see cref="Exception"/>.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <exception cref="ResultUnwrapException">Thrown when passed result is <see cref="Successful"/>.</exception>
        /// <returns>The <see cref="Exception"/>.</returns>
        public static implicit operator Exception(Result<T> result)
            => result.Successful
                ? throw new ResultUnwrapException("Can't extract exception from result as it's successful.")
                : ((Fail<T>) result).Exception;

        /// <summary>
        /// The operator that implicitly wraps the <see cref="Exception"/> to <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when passed exception is null.</exception>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        public static implicit operator Result<T>(Exception exception)
            => new Fail<T>(exception);

        /// <summary>
        /// The operator that implicitly wraps the <see cref="T"/> to <see cref="Result{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The <see cref="Result{T}"/>.</returns>
        public static implicit operator Result<T>(T value)
            => new Success<T>(value);
    }
}