using System;
using System.IO;
using System.Threading.Tasks;
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
                    x => x.BeAssignableTo<Result<int>>()).Successful
                .Should()
                .BeTrue();

        [Test]
        public void ConvertsExceptionToFailResult()
            => new Exception()
                .AsFail<object>()
                .ShouldMuch(x => x.BeOfType<Fail<object>>(),
                    x => x.BeAssignableTo<Result<object>>()).Successful
                .Should()
                .BeFalse();


        [Test]
        public void EvenExceptionIsSuccessfulResult()
            => new Exception()
                .AsSuccess()
                .ShouldMuch(x => x.BeOfType<Success<Exception>>(),
                    x => x.BeAssignableTo<Result<Exception>>()).Successful
                .Should()
                .BeTrue();

        [Test]
        public void BindsSuccess()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(x => x.ToString())
                .ShouldMuch(x => x.BeOfType<Success<string>>(),
                    x => x.BeAssignableTo<Result<string>>());

        [Test]
        public void BindsThrownException()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(new Func<int, string>(_ => throw new Exception()))
                .ShouldMuch(x => x.BeOfType<Fail<string>>(),
                    x => x.BeAssignableTo<Result<string>>());

        [Test]
        public void BindsException()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(_ => new Exception())
                .ShouldMuch(x => x.BeOfType<Success<Exception>>(),
                    x => x.BeAssignableTo<Result<Exception>>());

        [Test]
        public Task BindsAsyncSuccess()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(x => Task
                    .Delay(Faker.Random.Number(0, 100))
                    .ContinueWith(_ => x.ToString())
                )
                .ContinueWith(task => task.Result
                    .ShouldMuch(x => x.BeOfType<Success<string>>(),
                        x => x.BeAssignableTo<Result<string>>())
                );

        [Test]
        public Task BindsAsyncThrownException()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(_ => Task
                    .Delay(Faker.Random.Number(0, 100))
                    .ContinueWith<string>(_ => throw new Exception())
                )
                .ContinueWith(task => task.Result
                    .ShouldMuch(x => x.BeOfType<Fail<string>>(),
                        x => x.BeAssignableTo<Result<string>>())
                );

        [Test]
        public Task BindsAsyncException()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(x => Task
                    .Delay(Faker.Random.Number(0, 100))
                    .ContinueWith(_ => new Exception())
                )
                .ContinueWith(task => task.Result
                    .ShouldMuch(x => x.BeOfType<Success<Exception>>(),
                        x => x.BeAssignableTo<Result<Exception>>())
                );

        [Test]
        public Task BindsAsyncSuccessFromTask()
            => Task
                .Delay(Faker.Random.Number(0, 100))
                .ContinueWith(_ => Faker.Random.Number().AsSuccess())
                .Bind(x => Task
                    .Delay(Faker.Random.Number(0, 100))
                    .ContinueWith(_ => x.ToString())
                )
                .ContinueWith(task => task.Result
                    .ShouldMuch(x => x.BeOfType<Success<string>>(),
                        x => x.BeAssignableTo<Result<string>>()));
        
        [Test]
        public Task BindsAsyncFailFromTask()
            => Task
                .FromResult(Faker.Random.Number().AsSuccess())
                .Bind(x => Task
                    .Delay(Faker.Random.Number(0, 100))
                    .ContinueWith<string>(_ => throw new Exception())
                )
                .ContinueWith(task => task.Result
                    .ShouldMuch(x => x.BeOfType<Fail<string>>(),
                        x => x.BeAssignableTo<Result<string>>()));
    }
}