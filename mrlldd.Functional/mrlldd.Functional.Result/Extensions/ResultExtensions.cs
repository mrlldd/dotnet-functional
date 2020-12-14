using System;
using System.Threading.Tasks;

namespace mrlldd.Functional.Result.Extensions
{
    public static class ResultExtensions
    {
        public static Result<T> AsSuccess<T>(this T obj) => new Success<T>(obj);

        public static Result<T> AsFail<T>(this Exception exception) => new Fail<T>(exception);

        public static Result<TMapped> Bind<T, TMapped>(this Result<T> source, Func<T, TMapped> mapper)
        {
            if (!source.Successful)
            {
                return new Fail<TMapped>(source);
            }

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
                    .ContinueWith(new Func<Task<TMapped>, Result<TMapped>>(task => task.IsCompleted
                        ? task.Result
                        : task.Exception))
                : Task
                    .FromResult<Result<TMapped>>(new Fail<TMapped>(source));

        public static Task<Result<TMapped>> Bind<T, TMapped>(this Task<Result<T>> sourceTask,
            Func<T, Task<TMapped>> asyncMapper) 
            => sourceTask
                .ContinueWith(task
                    => task.IsCompleted
                        ? task.Result
                            .Bind(asyncMapper)
                        : Task
                            .FromResult<Result<TMapped>>(task.Exception))
                .Unwrap();
    }
}