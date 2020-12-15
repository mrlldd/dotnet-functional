using System;
using FluentAssertions;
using FluentAssertions.Primitives;
using NUnit.Framework.Constraints;

namespace mrlldd.Functional.Tests.Core.Extensions
{
    public static class AssertionsExtensions
    {
        public static ReferenceTypeAssertions<TFirst, TSecond> Multiple<TFirst, TSecond>(this ReferenceTypeAssertions<TFirst, TSecond> assertions,
            params Action<ReferenceTypeAssertions<TFirst, TSecond>>[] assertionEffects)
            where TSecond : ReferenceTypeAssertions<TFirst, TSecond>
        {
            foreach (var effect in assertionEffects)
            {
                effect(assertions);
            }

            return assertions;
        }

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