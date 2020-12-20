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
    public class GenericResultExtensionsTests : TestFixtureBase
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
                .Bind(_ => Task.FromException<string>(new TestException()))
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
                .Bind(_ => Task.FromException<string>(new TestException()))
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
                .Bind(_ => Task.FromException<string>(new TestException()));


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

        [Test]
        public async Task MapsSuccessResultTaskToAnotherResultSynchronously()
        {
            var target = Faker.Random.Number(1, 100);
            var multiplier = Faker.Random.Number() + 1;
            var result = await Task
                .Delay(Faker.Random
                    .Number(0, 100))
                .ContinueWith(_ => target
                    .AsSuccess())
                .Bind(x => x * multiplier);

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<int, Success<int>>(),
                    x => x.As<Success<int>>().Value
                        .Should()
                        .Be(target * multiplier));
        }

        [Test]
        public async Task NotMapsFailResultTaskToAnotherResultSynchronously()
        {
            var result = await Task
                .Delay(Faker.Random
                    .Number(0, 100))
                .ContinueWith(_ => Faker.Random
                    .Number(1, 100)
                    .AsSuccess())
                .Bind(x =>
                {
                    throw new TestException();
#pragma warning disable 162
                    return x;
#pragma warning restore 162
                });


            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<int, Fail<int>>(),
                    x => x.As<Fail<int>>().Exception
                        .Should()
                        .BeOfType<TestException>());
        }

        [Test]
        public async Task NotMapsCanceledFailResultTaskToAnotherResultSynchronously()
        {
            var tokenSource = new CancellationTokenSource()
                .SideEffect(x => x.Cancel());
            var result = await Task
                // ReSharper disable once MethodSupportsCancellation
                .Delay(Faker.Random.Number(0, 100))
                // ReSharper disable once MethodSupportsCancellation
                .ContinueWith(_ => Faker.Random.Number().AsSuccess())
                .Bind((x, token) =>
                {
                    token.ThrowIfCancellationRequested();
                    return x * 2;
                }, tokenSource.Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<int, Fail<int>>(),
                    x => x
                        .As<Fail<int>>().Exception
                        .SideEffect(exception => exception
                            .Should()
                            .BeOfType<OperationCanceledException>()));
        }

        [Test]
        public async Task NotMapsThrownCancellationFailResultTaskToAnotherResultSynchronously()
        {
            var tokenSource = new CancellationTokenSource();
            var result = await Task
                // ReSharper disable once MethodSupportsCancellation
                .Delay(Faker.Random.Number(0, 100))
                // ReSharper disable once MethodSupportsCancellation
                .ContinueWith(_ => Faker.Random.Number().AsSuccess())
                .Bind((x, _) =>
                {
                    throw new TestException();
#pragma warning disable 162
                    return x;
#pragma warning restore 162
                }, tokenSource.Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<int, Fail<int>>(),
                    x => x
                        .As<Fail<int>>().Exception
                        .SideEffect(exception => exception
                            .Should()
                            .BeOfType<TestException>()));
        }

        [Test]
        public async Task WrapsAsyncThrownException()
        {
            Func<Task<int>> func = () =>Task.FromException<int>(new TestException());
            var result = await func()
                .ThenWrapAsResult();
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<int, Fail<int>>(),
                    x => x
                        .As<Fail<int>>().Exception
                        .SideEffects(exception => exception
                                .Should()
                                .BeOfType<AggregateException>(),
                            exception => exception
                                .As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TestException>())
                );
        }
        
        [Test]
        public async Task WrapsTaskThatRanToCompletion()
        {
            var target = Faker.Random.Number();
            Func<Task<int>> func = async () =>
            {
                await Task.CompletedTask;
                return target;
            };
            var result = await func()
                .ThenWrapAsResult();
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<int, Success<int>>(),
                    x => x
                        .As<Success<int>>().Value
                        .Should()
                        .Be(target)
                );
        }
    }
    
    public class ValuelessResultExtensionsTests : TestFixtureBase
    {
        [Test]
        public void ConvertsExceptionToFailResult()
            => new TestException()
                .AsFail()
                .SideEffect(x => x
                    .Should()
                    .BeAResult<Fail>()).Successful
                .Should()
                .BeFalse();

        [Test]
        public void BindsSuccess()
            => Result.Success
                .Bind(() =>
                {
                    for (var i = 0; i < Faker.Random.Number(1, 50); i++)
                    {
                    }
                })
                .Should()
                .BeAResult<Success>();

        [Test]
        public void BindsThrownException()
            => Result.Success
                .Bind(new Action(() =>
                {
#pragma warning disable 162
                    for (var i = 0; i < Faker.Random.Number(1, 50); i++)
                    {
                        
                    }
#pragma warning restore 162
                    throw new TestException();
                }))
                .Should()
                .BeAResult<Fail>();

        [Test]
        public void BindsCanceled()
        {
            var tokenSource = new CancellationTokenSource()
                .SideEffect(x => x.Cancel());
            Func<Result> func = () => Result.Success
                .Bind(token =>
                {
                    token.ThrowIfCancellationRequested();
                }, tokenSource.Token);
            func
                .Should()
                .NotThrow();
            var result = func();
            result
                .Should()
                .BeAResult<Fail>();
            result
                .As<Fail>().Exception
                .Should()
                .BeOfType<OperationCanceledException>();
        }

        [Test]
        public Task BindsAsyncSuccess()
            => Result.Success
                .Bind(() => Task
                    .Delay(Faker.Random.Number(0, 100)))
                .ContinueWith(task => task.Result
                    .Should()
                    .BeAResult<Success>()
                );

        [Test]
        public Task BindsAsyncThrownException()
            => Result.Success
                .Bind(() => Task
                    .Delay(Faker.Random.Number(0, 100))
                    .ContinueWith<string>(_ => throw new TestException())
                )
                .ContinueWith(task => task.Result
                    .Should()
                    .BeAResult<Fail>()
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
            var result = await Result.Success
                .Bind(token => Task
                    .Delay(Timeout.InfiniteTimeSpan, token), tokenSource.Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x
                        .As<Fail>().Exception
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
            => Result.Success
                .Bind(() => Task
                    .Delay(Faker.Random.Number(0, 100))
                    .ContinueWith(_ => new TestException())
                )
                .ContinueWith(task => task.Result
                    .Should()
                    .BeAResult<Success>()
                );

        [Test]
        public Task BindsAsyncSuccessFromTask()
            => Task
                .Delay(Faker.Random.Number(0, 100))
                .ContinueWith(_ => Result.Success)
                .Bind(() => Task
                    .Delay(Faker.Random.Number(0, 100))
                )
                .ContinueWith(task => task.Result
                    .Should()
                    .BeAResult<Success>());

        [Test]
        public Task BindsAsyncFailFromTask()
            => Task
                .FromResult(Result.Success)
                .Bind(() => Task
                    .Delay(Faker.Random.Number(0, 100))
                    .ContinueWith<string>(_ => throw new TestException())
                )
                .ContinueWith(task => task.Result
                    .Should()
                    .BeAResult<Fail>());

        [Test]
        public Task BindsAsyncSuccessFromTaskThatReturnsException()
            => Task
                .Delay(Faker.Random.Number(0, 100))
                .ContinueWith(_ => Result.Success)
                .Bind(() => Task
                    .Delay(Faker.Random.Number(0, 100))
                    .ContinueWith(_ => new TestException())
                )
                .ContinueWith(task => task.Result
                    .Should()
                    .BeAResult<Success>());

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
                .ContinueWith(_ => Result.Success)
                .Bind(token => Task
                    .Delay(Timeout.InfiniteTimeSpan, token), tokenSource.Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x
                        .As<Fail>().Exception
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
                .ContinueWith(_ => Result.Success)
                .Bind(token => Task
                    .Delay(Faker.Random.Number(0, 100), token), tokenSource.Token);
            result
                .Should()
                .BeAResult<Success>();
        }

        [Test]
        public async Task BindsAsyncFailWithThrownExceptionFromTaskWithCancellationToken()
        {
            var tokenSource = new CancellationTokenSource();
            var result = await Task
                // ReSharper disable once MethodSupportsCancellation
                .Delay(Faker.Random.Number(0, 100))
                // ReSharper disable once MethodSupportsCancellation
                .ContinueWith(_ => Result.Success)
                .Bind(token => Task
                    .Delay(Faker.Random.Number(0, 100), token)
                    .ContinueWith<string>(_ => throw new TestException(), token), tokenSource.Token);
            result
                .Should()
                .BeAResult<Fail>();
        }

        [Test]
        public async Task BindsAsyncSuccessWithExceptionFromTaskWithCancellationToken()
        {
            var tokenSource = new CancellationTokenSource();
            var result = await Task
                // ReSharper disable once MethodSupportsCancellation
                .Delay(Faker.Random.Number(0, 100))
                // ReSharper disable once MethodSupportsCancellation
                .ContinueWith(_ => Result.Success)
                .Bind(token => Task
                    .Delay(Faker.Random.Number(0, 100), token)
                    .ContinueWith(_ => new TestException(), token), tokenSource.Token);
            result
                .Should()
                .BeAResult<Success>();
        }

        [Test]
        public Task NotBindsFailAsSuccess()
            => Task.FromResult(Result.Success)
                .Bind(() => Task.FromException(new TestException()))
                .ContinueWith(task => task.Result
                    .SideEffect(x => x
                        .Should()
                        .BeAResult<Fail>()).Successful
                    .Should()
                    .BeFalse());

        [Test]
        public Task NotBindsCanceledAsSuccess()
            => Task
                .FromCanceled<Result>(new CancellationTokenSource()
                    .SideEffect(x => x.Cancel())
                    .Token)
                .Bind(() => Task.FromException(new TestException()))
                .ContinueWith(task => task.Result
                    .SideEffect(x => x
                        .Should()
                        .BeAResult<Fail>()).Successful
                    .Should()
                    .BeFalse());

        [Test]
        public async Task BindingCanceledTaskNotThrows()
        {
            Func<Task<Result>> func = () => Task
                .FromCanceled<Result>(new CancellationTokenSource()
                    .SideEffect(x => x.Cancel())
                    .Token)
                .Bind(() => Task.FromException(new TestException()));

            await func
                .Should()
                .NotThrowAsync<AggregateException>();
            await func()
                .ContinueWith(task => task.Result
                    .SideEffects(x => x
                            .Should()
                            .BeAResult<Fail>(),
                        x => x.Successful
                            .Should()
                            .BeFalse(),
                        x => x
                            .As<Fail>()
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

        [Test]
        public async Task MapsSuccessResultTaskToAnotherResultSynchronously()
        {
            var result = await Task
                .Delay(Faker.Random
                    .Number(0, 100))
                .ContinueWith(_ => Result.Success)
                .Bind(() =>
                {
                    for (var i = 0; i < Faker.Random.Number(1, 50); i++)
                    {

                    }
                });

            result
                .SideEffect(x => x
                        .Should()
                        .BeAResult<Success>());
        }

        [Test]
        public async Task NotMapsFailResultTaskToAnotherResultSynchronously()
        {
            var result = await Task
                .Delay(Faker.Random
                    .Number(0, 100))
                .ContinueWith(_ => Result.Success)
                .Bind(new Action(() => throw new TestException()));

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeOfType<TestException>());
        }

        [Test]
        public async Task NotMapsCanceledFailResultTaskToAnotherResultSynchronously()
        {
            var tokenSource = new CancellationTokenSource()
                .SideEffect(x => x.Cancel());
            var result = await Task
                // ReSharper disable once MethodSupportsCancellation
                .Delay(Faker.Random.Number(0, 100))
                // ReSharper disable once MethodSupportsCancellation
                .ContinueWith(_ => Result.Success)
                .Bind(token => token.ThrowIfCancellationRequested(), tokenSource.Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x
                        .As<Fail>().Exception
                        .SideEffect(exception => exception
                            .Should()
                            .BeOfType<OperationCanceledException>()));
        }

        [Test]
        public async Task NotMapsThrownCancellationFailResultTaskToAnotherResultSynchronously()
        {
            var tokenSource = new CancellationTokenSource();
            var result = await Task
                // ReSharper disable once MethodSupportsCancellation
                .Delay(Faker.Random.Number(0, 100))
                // ReSharper disable once MethodSupportsCancellation
                .ContinueWith(_ => Result.Success)
                .Bind(new Action<CancellationToken>(_ => throw new TestException()), tokenSource.Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x
                        .As<Fail>().Exception
                        .SideEffect(exception => exception
                            .Should()
                            .BeOfType<TestException>()));
        }

        [Test]
        public async Task WrapsAsyncThrownException()
        {
            Func<Task> func = async () =>
            {
                await Task.CompletedTask;
                throw new TestException();
            };
            var result = await func()
                .ThenWrapAsResult();
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x
                        .As<Fail>().Exception
                        .SideEffects(exception => exception
                                .Should()
                                .BeOfType<AggregateException>(),
                            exception => exception
                                .As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TestException>())
                );
        }
        
        [Test]
        public async Task WrapsTaskThatRanToCompletion()
        {
            var result = await Task.CompletedTask
                .ThenWrapAsResult();
            result
                .SideEffect(x => x
                        .Should()
                        .BeAResult<Success>()
                );
        }
    }
}