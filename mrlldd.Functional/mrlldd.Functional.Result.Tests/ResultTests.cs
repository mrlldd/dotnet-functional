using System;
using FluentAssertions;
using mrlldd.Functional.Result.Exceptions;
using mrlldd.Functional.Result.Extensions;
using mrlldd.Functional.Tests.Core;
using mrlldd.Functional.Tests.Core.Exceptions;
using mrlldd.Functional.Tests.Core.Internal.Extensions;
using NUnit.Framework;

namespace mrlldd.Functional.Result.Tests
{
    public class ResultTests : TestFixtureBase
    {
        [Test]
        public void WrapsValueTypeValue()
        {
            var target = Faker.Random
                .Number();
            Func<Result<int>> func = () => target;
            func
                .SideEffects(x => x.Should().NotThrow<ResultUnwrapException>(),
                    f => target
                        .Should()
                        .Be(f().UnwrapAsSuccess()),
                    f => f()
                        .UnwrapAsSuccess()
                        .Should()
                        .Be(target)
                );
        }

        [Test]
        public void WrapsReferenceTypeValue()
        {
            var target = new object();
            Func<Result<object>> func = () => target;
            func
                .SideEffects(x => x.Should().NotThrow<ResultUnwrapException>(),
                    f => target
                        .Should()
                        .BeSameAs(f().UnwrapAsSuccess()),
                    f => f()
                        .UnwrapAsSuccess()
                        .Should()
                        .BeSameAs(target)
                );
        }

        [Test]
        public void WrapsException()
        {
            var exception = new TestException();
            Func<Result<int>> func = () => exception;
            func
                .SideEffects(x => x
                        .Should()
                        .NotThrow<ResultUnwrapException>(),
                    f => exception
                        .Should()
                        .BeSameAs(f().UnwrapAsFail()),
                    f => f()
                        .UnwrapAsFail()
                        .Should()
                        .BeSameAs(exception)
                );
        }

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
            var target = new TestException()
                .AsFail<object>();
            Func<Exception> func = () => target;
            func
                .Should()
                .NotThrow<ResultUnwrapException>();
        }

        [Test]
        public void ThrowsIfExpectedSuccessIsFail()
        {
            var target = new TestException()
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

        [Test]
        public void NullExceptionCauseArgumentNullException()
            => new Func<Result<object>>(() => (null as Exception)!.AsFail<object>())
                .Should()
                .ThrowExactly<ArgumentNullException>();
    }
}