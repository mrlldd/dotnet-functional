using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace mrlldd.Functional.Result.Extensions
{
    public static class ResultExtensions
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
                : new Fail<TMapped>(source);

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
                    .FromResult<Result<TMapped>>(new Fail<TMapped>(source));

        public static Task<Result<TMapped>> Bind<T, TMapped>(this Result<T> source,
            Func<T, CancellationToken, Task<TMapped>> asyncMapper, CancellationToken cancellationToken)
            => source.Successful
                ? asyncMapper(source, cancellationToken)
                    .ContinueWith(task =>
                        task.Exception == null ? 
                            task.IsCanceled 
                                ? new AggregateException(new OperationCanceledException(cancellationToken)) 
                                : task.Result.AsSuccess() 
                            : task.Exception.AsFail<TMapped>())
                : Task
                    .FromResult<Result<TMapped>>(new Fail<TMapped>(source));

        public static Task<Result<TMapped>> Bind<T, TMapped>(this Task<Result<T>> sourceTask,
            Func<T, Task<TMapped>> asyncMapper)
            => sourceTask
                .ContinueWith(task
                    => task.Exception == null
                        ? task.Result
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
    }
}