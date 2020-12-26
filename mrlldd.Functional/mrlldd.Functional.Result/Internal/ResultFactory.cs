using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace mrlldd.Functional.Result.Internal
{
    internal static class ResultFactory
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result ValuelessException(Exception exception)
            => new Fail(exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> GenericException<T>(Exception exception)
            => new Fail<T>(exception);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result ValuelessCanceled(Task task)
            => new Fail(new AggregateException(new TaskCanceledException(task)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Result<T> GenericCanceled<T>(Task task)
            => new Fail<T>(new AggregateException(new TaskCanceledException(task)));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> ValuelessCanceledTask(Task task)
            => Task.FromResult<Result>(new Fail(new AggregateException(new TaskCanceledException(task))));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<T>> GenericCanceledTask<T>(Task task)
            => Task
                .FromResult<Result<T>>(new Fail<T>(new AggregateException(new TaskCanceledException(task))));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result> ValuelessExceptionTask(Exception exception)
            => Task.FromResult<Result>(new Fail(exception));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<Result<T>> GenericExceptionTask<T>(Exception exception)
            => Task.FromResult<Result<T>>(new Fail<T>(exception));
    }
}