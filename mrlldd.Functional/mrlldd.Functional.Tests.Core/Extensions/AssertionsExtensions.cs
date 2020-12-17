using System;
using FluentAssertions;
using FluentAssertions.Primitives;

namespace mrlldd.Functional.Tests.Core.Extensions
{
    public static class AssertionsExtensions
    {
        public static T ShouldMuch<T>(this T source, params Action<ObjectAssertions>[] assertions)
        {
            foreach (var assert in assertions)
            {
                assert(source.Should());
            }

            return source;
        }
    }
}