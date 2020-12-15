using System;
using FluentAssertions;
using mrlldd.Functional.Result.Extensions;
using mrlldd.Functional.Tests.Core;
using mrlldd.Functional.Tests.Core.Extensions;
using NUnit.Framework;

namespace mrlldd.Functional.Result.Tests.Extensions
{
    public class ResultExtensionsTests : TestFixtureBase
    {
        [Test]
        public void ConvertsValueToSuccess()
            => Faker.Random
                .Number()
                .AsSuccess()
                .ShouldMuch(x => x.BeOfType<Success<int>>(),
                    x => x.BeAssignableTo<Result<int>>())
                .Successful
                .Should()
                .BeTrue();

        [Test]
        public void ConvertsExceptionToFailResult()
            => new Exception()
                .AsFail<object>()
                .ShouldMuch(x => x.BeOfType<Fail<object>>(),
                    x => x.BeAssignableTo<Result<object>>())
                .Successful
                .Should()
                .BeFalse();


        [Test]
        public void EvenExceptionIsSuccessfulResult()
            => new Exception()
                .AsSuccess()
                .ShouldMuch(x => x.BeOfType<Success<Exception>>(),
                    x => x.BeAssignableTo<Result<Exception>>())
                .Successful
                .Should()
                .BeTrue();
    }
}