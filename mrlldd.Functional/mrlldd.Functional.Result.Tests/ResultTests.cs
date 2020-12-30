using System;
using FluentAssertions;
using Functional.Result.Exceptions;
using Functional.Result.Extensions;
using Functional.Tests.Core;
using Functional.Tests.Core.Exceptions;
using Functional.Tests.Core.Internal.Extensions;
using NUnit.Framework;

namespace Functional.Result.Tests
{
    public class GenericResultTests : TestFixtureBase
    {
        [Test]
        public void WrapsValueTypeValue()
            => Faker.Random
                .Number()
                .Map(target => new Func<Result<int>>(() => target)
                    .SideEffects(x => x
                            .Should()
                            .NotThrow<ResultUnwrapException>(),
                        f => target.Should()
                            .Be(GenericResultExtensions.UnwrapAsSuccess<int>(f())),
                        f => GenericResultExtensions.UnwrapAsSuccess<int>(f())
                            .Should()
                            .Be(target)
                    ));

        [Test]
        public void WrapsReferenceTypeValue()
            => new object()
                .Map(target => new Func<Result<object>>(() => target)
                    .SideEffects(x => x
                            .Should()
                            .NotThrow<ResultUnwrapException>(),
                        f => target
                            .Should()
                            .BeSameAs(f().UnwrapAsSuccess()),
                        f => f()
                            .UnwrapAsSuccess()
                            .Should()
                            .BeSameAs(target)
                    ));

        [Test]
        public void WrapsException()
            => new TestException()
                .Map(exception => new Func<Result<int>>(() => exception)
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
                    ));

        [Test]
        public void UnwrapsValue()
            => new Func<int>(() => Faker.Random
                    .Number()
                    .AsSuccess())
                .Should()
                .NotThrow<ResultUnwrapException>();

        [Test]
        public void UnwrapsException()
            => new Func<Exception>(() => new TestException()
                    .AsFail<object>())
                .Should()
                .NotThrow<ResultUnwrapException>();

        [Test]
        public void ThrowsIfExpectedSuccessIsFail()
            => new Func<int>(() => new TestException()
                    .AsFail<int>())
                .Should()
                .ThrowExactly<ResultUnwrapException>();

        [Test]
        public void ThrowsIfExpectedFailIsSuccess()
            => new Func<Exception>(() => Faker.Random.Number()
                    .AsSuccess())
                .Should()
                .ThrowExactly<ResultUnwrapException>();

        [Test]
        public void NullExceptionCauseArgumentNullException()
            => new Func<Result<object>>(() => (null as Exception)!.AsFail<object>())
                .Should()
                .ThrowExactly<ArgumentNullException>();
    }

    public class ValuelessResultTests : TestFixtureBase
    {
        [Test]
        public void WrapsException()
            => new TestException()
                .Map(exception => new Func<Result>(() => exception)
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
                    ));

        [Test]
        public void UnwrapsException() 
            => new Func<Exception>(() => new TestException()
                    .AsFail())
                .Should()
                .NotThrow<ResultUnwrapException>();

        [Test]
        public void ThrowsIfExpectedSuccessIsFail() 
            => new Func<Success>(() => new TestException()
                    .AsFail()
                    .UnwrapAsSuccess())
                .Should()
                .ThrowExactly<ResultUnwrapException>();

        [Test]
        public void ThrowsIfExpectedFailIsSuccess()
            => new Func<Exception>(() => Result.Success)
                .Should()
                .ThrowExactly<ResultUnwrapException>();

        [Test]
        public void NullExceptionCauseArgumentNullException()
            => new Func<Result<object>>(() => (null as Exception)!.AsFail<object>())
                .Should()
                .ThrowExactly<ArgumentNullException>();
    }
}