using FluentAssertions.Primitives;
using mrlldd.Functional.Tests.Core.Internal.Extensions;

namespace mrlldd.Functional.Result.Tests.TestUtilities
{
    internal static class AssertionsExtensions
    {
        public static ObjectAssertions BeAResult<T, TResult>(this ObjectAssertions objectAssertions) where TResult : Result<T> 
            => objectAssertions
                .SideEffects(x => x.BeOfType<TResult>(),
                    x => x.BeAssignableTo<Result<T>>());
    }
}