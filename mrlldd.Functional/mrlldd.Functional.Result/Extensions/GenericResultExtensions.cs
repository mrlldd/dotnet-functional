using System;
using System.Threading;
using System.Threading.Tasks;
using mrlldd.Functional.Result.Internal;
using mrlldd.Functional.Result.Internal.Utilities;

namespace mrlldd.Functional.Result.Extensions
{
    public static class GenericResultExtensions
    {
        public static Result<T> AsSuccess<T>(this T obj) => new Success<T>(obj);

        public static Result<T> AsFail<T>(this Exception exception) => new Fail<T>(exception);

        public static T UnwrapAsSuccess<T>(this Result<T> result) => result;

        public static Exception UnwrapAsFail<T>(this Result<T> result) => result;

        public static Result<TMapped> Bind<T, TMapped>(this Result<T> result, Func<T, TMapped> mapper)
            => result.Successful
                ? Execute.Safely(((Success<T>) result).Value, mapper)
                : ResultFactory.GenericException<TMapped>(((Fail<T>) result).Exception);

        public static Result<TMapped> Bind<T, TMapped>(this Result<T> result,
            Func<T, CancellationToken, TMapped> mapper, CancellationToken cancellationToken = default)
            => result.Successful
                ? Execute.Safely(((Success<T>) result).Value, mapper, cancellationToken)
                : ResultFactory.GenericException<TMapped>(((Fail<T>) result).Exception);

        public static Task<Result<TMapped>> Bind<T, TMapped>(this Result<T> result, Func<T, Task<TMapped>> asyncMapper)
            => result.Successful
                ? asyncMapper(((Success<T>) result).Value)
                    .ThenWrapAsResult()
                : ResultFactory.GenericExceptionTask<TMapped>(((Fail<T>) result).Exception);

        public static Task<Result<TMapped>> Bind<T, TMapped>(this Result<T> result,
            Func<T, CancellationToken, Task<TMapped>> asyncMapper, CancellationToken cancellationToken)
            => result.Successful
                ? asyncMapper(result, cancellationToken)
                    // ReSharper disable once MethodSupportsCancellation
                    .ContinueWith(task =>
                        task.Exception == null
                            ? task.IsCanceled
                                ? ResultFactory.GenericCanceled<TMapped>(task)
                                : task.Result.AsSuccess()
                            : ResultFactory.GenericException<TMapped>(task.Exception))
                : ResultFactory.GenericExceptionTask<TMapped>(((Fail<T>) result).Exception);

        public static Task<Result<TMapped>> Bind<T, TMapped>(this Task<Result<T>> sourceTask,
            Func<T, Task<TMapped>> asyncMapper)
            => sourceTask
                .ContinueWith(task
                    => task.Exception == null
                        ? task.IsCanceled
                            ? ResultFactory.GenericCanceledTask<TMapped>(task)
                            : task.Result
                                .Bind(asyncMapper)
                        : ResultFactory.GenericExceptionTask<TMapped>(task.Exception))
                .Unwrap();

        public static Task<Result<TMapped>> Bind<T, TMapped>(this Task<Result<T>> sourceTask,
            Func<T, CancellationToken, Task<TMapped>> asyncMapper, CancellationToken cancellationToken)
            => sourceTask
                .ContinueWith(task
                    => task.Exception == null
                        ? task.IsCanceled
                            ? ResultFactory.GenericCanceledTask<TMapped>(task)
                            : task.Result
                                .Bind(asyncMapper, cancellationToken)
                        : ResultFactory.GenericExceptionTask<TMapped>(task.Exception))
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
                .ContinueWith(task => task.Exception == null
                    ? task.IsCanceled
                        ? ResultFactory.GenericCanceled<T>(task)
                        : task.Result.AsSuccess()
                    : ResultFactory.GenericException<T>(task.Exception)
                );

        public static Result<T> EffectIfSuccessful<T>(this Result<T> result, Action effect)
        {
            if (result.Successful)
            {
                effect();
            }

            return result;
        }

        public static Result<T> EffectIfSuccessful<T>(this Result<T> result, Action<T> effect)
        {
            if (result.Successful)
            {
                effect(((Success<T>) result).Value);
            }

            return result;
        }

        public static Result<T> EffectIfSuccessful<T>(this Result<T> result, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
        {
            if (result.Successful)
            {
                effect(cancellationToken);
            }

            return result;
        }

        public static Result<T> EffectIfSuccessful<T>(this Result<T> result, Action<T, CancellationToken> effect,
            CancellationToken cancellationToken)
        {
            if (result.Successful)
            {
                effect(((Success<T>) result).Value, cancellationToken);
            }

            return result;
        }

        public static async Task<Result<T>> EffectIfSuccessfulAsync<T>(this Result<T> result,
            Func<Task> asyncEffect)
        {
            if (result.Successful)
            {
                await asyncEffect();
            }

            return result;
        }

        public static async Task<Result<T>> EffectIfSuccessfulAsync<T>(this Result<T> result,
            Func<CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
        {
            if (result.Successful)
            {
                await asyncEffect(cancellationToken);
            }

            return result;
        }

        public static async Task<Result<T>> EffectIfSuccessfulAsync<T>(this Result<T> result,
            Func<T, Task> asyncEffect)
        {
            if (result.Successful)
            {
                await asyncEffect(((Success<T>) result).Value);
            }

            return result;
        }

        public static async Task<Result<T>> EffectIfSuccessfulAsync<T>(this Result<T> result,
            Func<T, CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
        {
            if (result.Successful)
            {
                await asyncEffect(((Success<T>) result).Value, cancellationToken);
            }

            return result;
        }

        public static Result<T> EffectIfNotSuccessful<T>(this Result<T> result, Action effect)
        {
            if (!result.Successful)
            {
                effect();
            }

            return result;
        }

        public static Result<T> EffectIfNotSuccessful<T>(this Result<T> result, Action<CancellationToken> effect,
            CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                effect(cancellationToken);
            }

            return result;
        }

        public static Result<T> EffectIfNotSuccessful<T>(this Result<T> result, Action<Exception> effect)
        {
            if (!result.Successful)
            {
                effect(((Fail<T>) result).Exception);
            }

            return result;
        }

        public static Result<T> EffectIfNotSuccessful<T>(this Result<T> result,
            Action<Exception, CancellationToken> effect, CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                effect(((Fail<T>) result).Exception, cancellationToken);
            }

            return result;
        }

        public static async Task<Result<T>> EffectIfNotSuccessfulAsync<T>(this Result<T> result,
            Func<Task> asyncEffect)
        {
            if (!result.Successful)
            {
                await asyncEffect();
            }

            return result;
        }

        public static async Task<Result<T>> EffectIfNotSuccessfulAsync<T>(this Result<T> result,
            Func<CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                await asyncEffect(cancellationToken);
            }

            return result;
        }

        public static async Task<Result<T>> EffectIfNotSuccessfulAsync<T>(this Result<T> result,
            Func<Exception, Task> asyncEffect)
        {
            if (!result.Successful)
            {
                await asyncEffect(((Fail<T>) result).Exception);
            }

            return result;
        }

        public static async Task<Result<T>> EffectIfNotSuccessfulAsync<T>(this Result<T> result,
            Func<Exception, CancellationToken, Task> asyncEffect, CancellationToken cancellationToken)
        {
            if (!result.Successful)
            {
                await asyncEffect(((Fail<T>) result).Exception, cancellationToken);
            }

            return result;
        }
    }
}