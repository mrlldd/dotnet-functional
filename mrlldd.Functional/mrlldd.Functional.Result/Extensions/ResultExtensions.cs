using System;
using System.Runtime.CompilerServices;
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

        public static Task<Result<TMapped>> Bind<T, TMapped>(this Result<T> source, Func<T, Task<TMapped>> asyncMapper) 
            => source.Successful
                ? asyncMapper(source)
                    .ContinueWith(task => task.Exception ?? task.Result.AsSuccess())
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
    }
}