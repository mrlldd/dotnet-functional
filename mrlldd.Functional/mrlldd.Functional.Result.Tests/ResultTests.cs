using System;
using Bogus;
using FluentAssertions;
using mrlldd.Functional.Result.Exceptions;
using mrlldd.Functional.Result.Extensions;
using mrlldd.Functional.Tests.Core;
using mrlldd.Functional.Tests.Core.Extensions;
using NUnit.Framework;

namespace mrlldd.Functional.Result.Tests
{
    public class ResultTests : TestFixtureBase
    {
        [Test]
        public void UnwrapsValue()
        {
            var target = Faker.Random
                .Number()
                .AsSuccess();
            Func<int> func = () => target;
            func
                .Should()
                .NotThrow<ResultUnwrapException>();
        }

        [Test]
        public void UnwrapsException()
        {
            var target = new Exception()
                .AsFail<object>();
            Func<Exception> func = () => target;
            func
                .Should()
                .NotThrow<ResultUnwrapException>();
        }

        [Test]
        public void ThrowsIfExpectedSuccessIsFail()
        {
            var target = new Exception()
                .AsFail<int>();
            Func<int> func = () => target;
            func
                .Should()
                .ThrowExactly<ResultUnwrapException>();
        }
        
        [Test]
        public void ThrowsIfExpectedFailIsSuccess()
        {
            var target = Faker.Random.Number()
                .AsSuccess();
            Func<Exception> func = () => target;
            func
                .Should()
                .ThrowExactly<ResultUnwrapException>();
        }
    }
}