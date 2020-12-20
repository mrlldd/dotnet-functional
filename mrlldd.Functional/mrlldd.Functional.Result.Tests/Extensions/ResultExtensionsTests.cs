using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using mrlldd.Functional.Result.Extensions;
using mrlldd.Functional.Result.Tests.TestUtilities;
using mrlldd.Functional.Tests.Core;
using mrlldd.Functional.Tests.Core.Exceptions;
using mrlldd.Functional.Tests.Core.Extensions;
using mrlldd.Functional.Tests.Core.Internal.Extensions;
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
                .SideEffect(x => x
                    .Should()
                    .BeAResult<int, Success<int>>()).Successful
                .Should()
                .BeTrue();

        [Test]
        public void ConvertsExceptionToFailResult()
            => new TestException()
                .AsFail<object>()
                .SideEffect(x => x
                    .Should()
                    .BeAResult<object, Fail<object>>()).Successful
                .Should()
                .BeFalse();


        [Test]
        public void EvenExceptionIsSuccessfulResult()
            => new TestException()
                .AsSuccess()
                .SideEffect(x => x
                    .Should()
                    .BeAResult<TestException, Success<TestException>>()).Successful
                .Should()
                .BeTrue();

        [Test]
        public void BindsSuccess()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(x => x.ToString())
                .Should()
                .BeAResult<string, Success<string>>();

        [Test]
        public void BindsThrownException()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(new Func<int, string>(_ => throw new TestException()))
                .Should()
                .BeAResult<string, Fail<string>>();

        [Test]
        public void BindsCanceled()
        {
            var tokenSource = new CancellationTokenSource()
                .SideEffect(x => x.Cancel());
            var target = Faker.Random.Number().AsSuccess();
            Func<Result<string>> func = () => target
                .Bind((x, token) =>
                {
                    token.ThrowIfCancellationRequested();
                    return x.ToString();
                }, tokenSource.Token);
            func
                .Should()
                .NotThrow();
            var result = func();
            result
                .Should()
                .BeAResult<string, Fail<string>>();
            result
                .As<Fail<string>>().Exception
                .Should()
                .BeOfType<OperationCanceledException>();
        }

        [Test]
        public void BindsException()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(_ => new TestException())
                .Should()
                .BeAResult<TestException, Success<TestException>>();

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
                    .Should()
                    .BeAResult<string, Success<string>>()
                );

        [Test]
        public Task BindsAsyncThrownException()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(_ => Task
                    .Delay(Faker.Random.Number(0, 100))
                    .ContinueWith<string>(_ => throw new TestException())
                )
                .ContinueWith(task => task.Result
                    .Should()
                    .BeAResult<string, Fail<string>>()
                );

        [Test]
        public async Task BindsAsyncCanceled()
        {
            var tokenSource = new CancellationTokenSource();
#pragma warning disable 4014
            // ReSharper disable once MethodSupportsCancellation
            Task.Delay(Faker.Random.Number(100, 200))
                // ReSharper disable once MethodSupportsCancellation
                .ContinueWith(_ => tokenSource.Cancel());
#pragma warning restore 4014
            var result = await Faker.Random
                .Number()
                .AsSuccess()
                .Bind((x, token) => Task
                    .Delay(Timeout.InfiniteTimeSpan, token)
                    .ContinueWith(_ => x.ToString(), token), tokenSource.Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<string, Fail<string>>(),
                    x => x
                        .As<Fail<string>>().Exception
                        .SideEffects(exception => exception
                                .Should()
                                .BeOfType<AggregateException>(),
                            exception => exception
                                .As<AggregateException>().InnerException
                                .ShouldMuch(should => should.NotBeNull(),
                                    should => should.BeOfType<TaskCanceledException>())
                        )
                );
        }

        [Test]
        public Task BindsAsyncException()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(_ => Task
                    .Delay(Faker.Random.Number(0, 100))
                    .ContinueWith(_ => new TestException())
                )
                .ContinueWith(task => task.Result
                    .Should()
                    .BeAResult<TestException, Success<TestException>>()
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
                    .Should()
                    .BeAResult<string, Success<string>>());

        [Test]
        public Task BindsAsyncFailFromTask()
            => Task
                .FromResult(Faker.Random.Number().AsSuccess())
                .Bind(_ => Task
                    .Delay(Faker.Random.Number(0, 100))
                    .ContinueWith<string>(_ => throw new TestException())
                )
                .ContinueWith(task => task.Result
                    .Should()
                    .BeAResult<string, Fail<string>>());

        [Test]
        public Task BindsAsyncSuccessFromTaskThatReturnsException()
            => Task
                .Delay(Faker.Random.Number(0, 100))
                .ContinueWith(_ => Faker.Random.Number().AsSuccess())
                .Bind(_ => Task
                    .Delay(Faker.Random.Number(0, 100))
                    .ContinueWith(_ => new TestException())
                )
                .ContinueWith(task => task.Result
                    .Should()
                    .BeAResult<TestException, Success<TestException>>());

        [Test]
        public async Task BindsAsyncFailFromCanceledTask()
        {
            var tokenSource = new CancellationTokenSource();
#pragma warning disable 4014
            Task
                // ReSharper disable once MethodSupportsCancellation
                .Delay(Faker.Random.Number(100, 200))
                // ReSharper disable once MethodSupportsCancellation
                .ContinueWith(_ => tokenSource.Cancel());
#pragma warning restore 4014

            var result = await Task
                // ReSharper disable once MethodSupportsCancellation
                .Delay(Faker.Random.Number(0, 100))
                // ReSharper disable once MethodSupportsCancellation
                .ContinueWith(_ => Faker.Random.Number().AsSuccess())
                .Bind((x, token) => Task
                    .Delay(Timeout.InfiniteTimeSpan, token)
                    .ContinueWith(_ => x.ToString(), token), tokenSource.Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<string, Fail<string>>(),
                    x => x
                        .As<Fail<string>>().Exception
                        .SideEffects(exception => exception
                                .Should()
                                .BeOfType<AggregateException>(),
                            exception => exception
                                .As<AggregateException>().InnerException
                                .ShouldMuch(should => should.NotBeNull(),
                                    should => should.BeOfType<TaskCanceledException>())
                        )
                );
        }

        [Test]
        public async Task BindsAsyncResultFromTaskWithCancellationToken()
        {
            var tokenSource = new CancellationTokenSource();
            var result = await Task
                // ReSharper disable once MethodSupportsCancellation
                .Delay(Faker.Random.Number(0, 100))
                // ReSharper disable once MethodSupportsCancellation
                .ContinueWith(_ => Faker.Random.Number().AsSuccess())
                .Bind((x, token) => Task
                    .Delay(Faker.Random.Number(0, 100), token)
                    .ContinueWith(_ => x.ToString(), token), tokenSource.Token);
            result
                .Should()
                .BeAResult<string, Success<string>>();
        }

        [Test]
        public async Task BindsAsyncFailWithThrownExceptionFromTaskWithCancellationToken()
        {
            var tokenSource = new CancellationTokenSource();
            var result = await Task
                // ReSharper disable once MethodSupportsCancellation
                .Delay(Faker.Random.Number(0, 100))
                // ReSharper disable once MethodSupportsCancellation
                .ContinueWith(_ => Faker.Random.Number().AsSuccess())
                .Bind((x, token) => Task
                    .Delay(Faker.Random.Number(0, 100), token)
                    .ContinueWith<string>(_ => throw new TestException(), token), tokenSource.Token);
            result
                .Should()
                .BeAResult<string, Fail<string>>();
        }

        [Test]
        public async Task BindsAsyncSuccessWithExceptionFromTaskWithCancellationToken()
        {
            var tokenSource = new CancellationTokenSource();
            var result = await Task
                // ReSharper disable once MethodSupportsCancellation
                .Delay(Faker.Random.Number(0, 100))
                // ReSharper disable once MethodSupportsCancellation
                .ContinueWith(_ => Faker.Random.Number().AsSuccess())
                .Bind((x, token) => Task
                    .Delay(Faker.Random.Number(0, 100), token)
                    .ContinueWith(_ => new TestException(), token), tokenSource.Token);
            result
                .Should()
                .BeAResult<TestException, Success<TestException>>();
        }

        [Test]
        public Task NotBindsFailAsSuccess()
            => Task.FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(async x =>
                {
                    await Task.CompletedTask;
                    throw new TestException();
#pragma warning disable 162
                    return x.ToString();
#pragma warning restore 162
                })
                .ContinueWith(task => task.Result
                    .SideEffect(x => x
                        .Should()
                        .BeAResult<string, Fail<string>>()).Successful
                    .Should()
                    .BeFalse());

        [Test]
        public Task NotBindsCanceledAsSuccess()
            => Task
                .FromCanceled<Result<int>>(new CancellationTokenSource()
                    .SideEffect(x => x.Cancel())
                    .Token)
                .Bind(async x =>
                {
                    await Task.CompletedTask;
                    throw new TestException();
#pragma warning disable 162
                    return x.ToString();
#pragma warning restore 162
                })
                .ContinueWith(task => task.Result
                    .SideEffect(x => x
                        .Should()
                        .BeAResult<string, Fail<string>>()).Successful
                    .Should()
                    .BeFalse());

        [Test]
        public async Task BindingCanceledTaskNotThrows()
        {
            Func<Task<Result<string>>> func = () => Task
                .FromCanceled<Result<int>>(new CancellationTokenSource()
                    .SideEffect(x => x.Cancel())
                    .Token)
                .Bind(async x =>
                {
                    await Task.CompletedTask;
                    throw new TestException();
#pragma warning disable 162
                    return x.ToString();
#pragma warning restore 162
                });

            await func
                .Should()
                .NotThrowAsync<AggregateException>();
            await func()
                .ContinueWith(task => task.Result
                    .SideEffects(x => x
                            .Should()
                            .BeAResult<string, Fail<string>>(),
                        x => x.Successful
                            .Should()
                            .BeFalse(),
                        x => x
                            .As<Fail<string>>()
                            .SideEffects(fail => fail.Exception
                                    .Should()
                                    .BeOfType<AggregateException>(),
                                fail => fail.Exception
                                    .As<AggregateException>()
                                    .SideEffect(ae => ae
                                        .Should()
                                        .NotBeNull()).InnerException
                                    .Should()
                                    .BeOfType<TaskCanceledException>())
                    ));
        }
    }
}