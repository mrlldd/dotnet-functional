using System;
using System.Threading.Tasks;

namespace mrlldd.Functional.Result.Internal
{
    internal static class ResultFactory
    {
        public static Result ValuelessException(Exception exception)
            => new Fail(exception);
        
        public static Result<T> GenericException<T>(Exception exception)
            => new Fail<T>(exception);

        public static Result ValuelessCanceled(Task task)
            => new Fail(new AggregateException(new TaskCanceledException(task)));
        
        public static Result<T> GenericCanceled<T>(Task task)
            => new Fail<T>(new AggregateException(new TaskCanceledException(task)));
        
        public static Task<Result> ValuelessCanceledTask(Task task)
            => Task.FromResult<Result>(new Fail(new AggregateException(new TaskCanceledException(task))));
        
        public static Task<Result<T>> GenericCanceledTask<T>(Task task)
            => Task
                .FromResult<Result<T>>(new Fail<T>(new AggregateException(new TaskCanceledException(task))));
        
        public static Task<Result> ValuelessExceptionTask(Exception exception)
            => Task.FromResult<Result>(new Fail(exception));

        public static Task<Result<T>> GenericExceptionTask<T>(Exception exception)
            => Task.FromResult<Result<T>>(new Fail<T>(exception));
    }
}