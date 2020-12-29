using FluentAssertions.Primitives;
using mrlldd.Functional.Tests.Core.Internal.Extensions;

namespace mrlldd.Functional.Result.Tests.TestUtilities
{
    internal static class AssertionsExtensions
    {
        private static ObjectAssertions BeAResult<T, TResult>(ObjectAssertions objectAssertions) where TResult : Result<T> 
            => objectAssertions
                .SideEffects(x => x.BeOfType<TResult>(),
                    x => x.BeAssignableTo<Result<T>>());

        public static ObjectAssertions BeAFail<T>(this ObjectAssertions objectAssertions)
            => BeAResult<T, Fail<T>>(objectAssertions);

        public static ObjectAssertions BeASuccess<T>(this ObjectAssertions objectAssertions)
            => BeAResult<T, Success<T>>(objectAssertions);
        
        public static ObjectAssertions BeAResult<TResult>(this ObjectAssertions objectAssertions) where TResult : Result 
            => objectAssertions
                .SideEffects(x => x.BeOfType<TResult>(),
                    x => x.BeAssignableTo<Result>());
    }
}