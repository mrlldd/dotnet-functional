using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Primitives;
using Moq;
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

        [Test]
        public async Task PerformsSideEffectsOnSuccessfulResults()
        {
            var moqed = new Mock<ISideEffectsWrapper<object>>();
            var token = new CancellationTokenSource().Token;
            var obj = new object();
            moqed
                .Setup(x => x.Effect())
                .Verifiable();
            moqed
                .Setup(x => x.ArgumentedEffect(It.Is<object>(o => o == obj)))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableArgumentedEffect(It.Is<object>(o => o == obj), It.Is<CancellationToken>(ct => ct.Equals(token))))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))))
                .Verifiable();
            moqed
                .Setup(x => x.AsyncEffect())
                .Verifiable();
            moqed
                .Setup(x => x.ArgumentedEffectAsync(It.Is<object>(o => o == obj)))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableArgumentedEffectAsync(It.Is<object>(o => o == obj), It.Is<CancellationToken>(ct => ct.Equals(token))))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))))
                .Verifiable();
            var result = obj.AsSuccess();
            result
                .SideEffectIfSuccessful(moqed.Object.Effect)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(result))
                .SideEffectIfSuccessful(moqed.Object.ArgumentedEffect)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(result))
                .SideEffectIfSuccessful(moqed.Object.CancellableEffect, token)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(result))
                .SideEffectIfSuccessful(moqed.Object.CancellableArgumentedEffect, token)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(result));
            await result
                .SideEffectIfSuccessfulAsync(moqed.Object.AsyncEffect)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(result));
            await result
                .SideEffectIfSuccessfulAsync(moqed.Object.ArgumentedEffectAsync)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(result));
            await result
                .SideEffectIfSuccessfulAsync(moqed.Object.AsyncCancellableEffect, token)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(result));
            await result
                .SideEffectIfSuccessfulAsync(moqed.Object.CancellableArgumentedEffectAsync, token)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(result));
            moqed
                .Verify(x => x.Effect(), Times.Once);
            moqed
                .Verify(x => x.ArgumentedEffect(It.Is<object>(o => o == obj)), Times.Once);
            moqed
                .Verify(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))), Times.Once);
            moqed
                .Verify(x => x.CancellableArgumentedEffect(It.Is<object>(o => o == obj),It.Is<CancellationToken>(ct => ct.Equals(token))), Times.Once);
            moqed
                .Verify(x => x.AsyncEffect(), Times.Once);
            moqed
                .Verify(x => x.ArgumentedEffectAsync(It.Is<object>(o => o == obj)), Times.Once);
            moqed
                .Verify(x => x.AsyncCancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))), Times.Once);
            moqed
                .Verify(x => x.CancellableArgumentedEffectAsync(It.Is<object>(o => o == obj), It.Is<CancellationToken>(ct => ct.Equals(token))), Times.Once);
        }
        
        [Test]
        public async Task NotPerformsSideEffectOnSuccessfulResults()
        {
            var moqed = new Mock<ISideEffectsWrapper<int>>();
            moqed
                .Setup(x => x.Effect())
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffect(It.IsAny<CancellationToken>())
                )
                .Verifiable();
            moqed
                .Setup(x => x.AsyncEffect())
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffect(It.IsAny<CancellationToken>()))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffectPopulatedWithException(It.IsAny<TestException>(),
                    It.IsAny<CancellationToken>()))
                .Verifiable();
            moqed
                .Setup(x => x.EffectPopulatedWithException(It.IsAny<TestException>()))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffectPopulatedWithExceptionAsync(It.IsAny<TestException>(),
                    It.IsAny<CancellationToken>()))
                .Verifiable();
            moqed
                .Setup(x => x.EffectPopulatedWithExceptionAsync(It.IsAny<TestException>()))
                .Verifiable();
            var result = Faker.Random
                .Number()
                .AsSuccess();
            result
                .SideEffectIfNotSuccessful(moqed.Object.Effect)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(result))
                .SideEffectIfNotSuccessful(moqed.Object.CancellableEffect, CancellationToken.None)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(result))
                .SideEffectIfNotSuccessful(moqed.Object.EffectPopulatedWithException)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(result))
                .SideEffectIfNotSuccessful(moqed.Object.CancellableEffectPopulatedWithException, CancellationToken.None)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(result));
            await result
                .SideEffectIfNotSuccessfulAsync(moqed.Object.AsyncEffect)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(result));
            await result
                .SideEffectIfNotSuccessfulAsync(moqed.Object.CancellableEffectPopulatedWithExceptionAsync,
                    CancellationToken.None)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(result));
            await result
                .SideEffectIfNotSuccessfulAsync(moqed.Object.AsyncCancellableEffect, CancellationToken.None);
            moqed
                .Verify(x => x.Effect(), Times.Never);
            moqed
                .Verify(x => x.CancellableEffect(It.IsAny<CancellationToken>()), Times.Never);
            moqed
                .Verify(x => x.AsyncEffect(), Times.Never);
            moqed
                .Verify(x => x.AsyncCancellableEffect(It.IsAny<CancellationToken>()), Times.Never);
            moqed
                .Verify(
                    x => x.CancellableEffectPopulatedWithException(It.IsAny<TestException>(),
                        It.IsAny<CancellationToken>()), Times.Never);
            moqed
                .Verify(
                    x => x.CancellableEffectPopulatedWithExceptionAsync(It.IsAny<TestException>(),
                        It.IsAny<CancellationToken>()), Times.Never);
            moqed
                .Verify(
                    x => x.EffectPopulatedWithExceptionAsync(It.IsAny<TestException>()), Times.Never);
            moqed
                .Verify(
                    x => x.EffectPopulatedWithException(It.IsAny<TestException>()), Times.Never);
        }
        
        [Test]
        public async Task NotPerformsSideEffectOnNotSuccessfulResults()
        {
            var moqed = new Mock<ISideEffectsWrapper<object>>();
            var token = new CancellationTokenSource().Token;
            moqed
                .Setup(x => x.Effect())
                .Verifiable();
            moqed
                .Setup(x => x.ArgumentedEffect(It.IsAny<object>()))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableArgumentedEffect(It.IsAny<object>(), It.Is<CancellationToken>(ct => ct.Equals(token))))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))))
                .Verifiable();
            moqed
                .Setup(x => x.AsyncEffect())
                .Verifiable();
            moqed
                .Setup(x => x.ArgumentedEffectAsync(It.IsAny<object>()))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableArgumentedEffectAsync(It.IsAny<object>(), It.Is<CancellationToken>(ct => ct.Equals(token))))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))))
                .Verifiable();
            var fail = new TestException()
                .AsFail<object>();
            fail
                .SideEffectIfSuccessful(moqed.Object.Effect)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail))
                .SideEffectIfSuccessful(moqed.Object.ArgumentedEffect)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail))
                .SideEffectIfSuccessful(moqed.Object.CancellableEffect, token)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail))
                .SideEffectIfSuccessful(moqed.Object.CancellableArgumentedEffect, token)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail));
            await fail
                .SideEffectIfSuccessfulAsync(moqed.Object.AsyncEffect)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            await fail
                .SideEffectIfSuccessfulAsync(moqed.Object.ArgumentedEffectAsync)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            await fail
                .SideEffectIfSuccessfulAsync(moqed.Object.AsyncCancellableEffect, token)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            await fail
                .SideEffectIfSuccessfulAsync(moqed.Object.CancellableArgumentedEffectAsync, token)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            moqed
                .Verify(x => x.Effect(), Times.Never);
            moqed
                .Verify(x => x.ArgumentedEffect(It.IsAny<object>()), Times.Never);
            moqed
                .Verify(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))), Times.Never);
            moqed
                .Verify(x => x.CancellableArgumentedEffect(It.IsAny<object>(),It.Is<CancellationToken>(ct => ct.Equals(token))), Times.Never);
            moqed
                .Verify(x => x.AsyncEffect(), Times.Never);
            moqed
                .Verify(x => x.ArgumentedEffectAsync(It.IsAny<object>()), Times.Never);
            moqed
                .Verify(x => x.AsyncCancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))), Times.Never);
            moqed
                .Verify(x => x.CancellableArgumentedEffectAsync(It.IsAny<object>(), It.Is<CancellationToken>(ct => ct.Equals(token))), Times.Never);
        }

        [Test]
        public async Task PerformsSideEffectOnNotSuccessfulResults()
        {
            var moqed = new Mock<ISideEffectsWrapper<object>>();
            var token = new CancellationTokenSource().Token;
            var obj = new object();
            moqed
                .Setup(x => x.Effect())
                .Verifiable();
            moqed
                .Setup(x => x.ArgumentedEffect(It.IsAny<object>()))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct == token)))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableArgumentedEffect(It.IsAny<object>(),It.Is<CancellationToken>(ct => ct == token)))
                .Verifiable();
            moqed
                .Setup(x => x.AsyncEffect())
                .Verifiable();
            moqed
                .Setup(x => x.ArgumentedEffectAsync(It.IsAny<object>()))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableArgumentedEffectAsync(It.IsAny<object>(), It.Is<CancellationToken>(ct => ct == token)))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct == token)))
                .Verifiable();
            var exception = new TestException();
            moqed
                .Setup(x => x.EffectPopulatedWithException(It.Is<TestException>(te => te == exception)))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffectPopulatedWithException(It.Is<TestException>(te => te == exception),
                    It.Is<CancellationToken>(ct => ct == token)))
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffectPopulatedWithExceptionAsync(It.Is<TestException>(te => te == exception),
                    It.Is<CancellationToken>(ct => ct == token)))
                .Verifiable();
            moqed
                .Setup(x => x.EffectPopulatedWithExceptionAsync(It.Is<TestException>(te => te == exception)))
                .Verifiable();
            var fail = exception
                .AsFail<object>();
            fail
                .SideEffectIfNotSuccessful(moqed.Object.Effect)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail))
                .SideEffectIfNotSuccessful(moqed.Object.ArgumentedEffect)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail))
                .SideEffectIfNotSuccessful(moqed.Object.CancellableEffect, token)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail))
                .SideEffectIfNotSuccessful(moqed.Object.CancellableArgumentedEffect, token)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail))
                .SideEffectIfNotSuccessful(moqed.Object.EffectPopulatedWithException)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail))
                .SideEffectIfNotSuccessful(moqed.Object.CancellableEffectPopulatedWithException, token)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail));

            await fail
                .SideEffectIfNotSuccessfulAsync(moqed.Object.AsyncEffect)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            await fail
                .SideEffectIfNotSuccessfulAsync(moqed.Object.ArgumentedEffectAsync)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            await fail
                .SideEffectIfNotSuccessfulAsync(moqed.Object.AsyncCancellableEffect, token)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            await fail
                .SideEffectIfNotSuccessfulAsync(moqed.Object.CancellableArgumentedEffectAsync, token)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            await fail
                .SideEffectIfNotSuccessfulAsync(moqed.Object.CancellableEffectPopulatedWithExceptionAsync, token)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            await fail
                .SideEffectIfNotSuccessfulAsync(moqed.Object.EffectPopulatedWithExceptionAsync)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            moqed
                .Verify(x => x.Effect(), Times.Once);
            moqed
                .Verify(x => x.ArgumentedEffect(It.IsAny<object>()), Times.Once);
            moqed
                .Verify(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct == token)), Times.Once);
            moqed
                .Verify(x => x.CancellableArgumentedEffect(It.IsAny<object>(),It.Is<CancellationToken>(ct => ct == token)), Times.Once);
            moqed
                .Verify(
                    x => x.CancellableEffectPopulatedWithException(It.Is<TestException>(te => te == exception),
                        It.Is<CancellationToken>(ct => ct == token)), Times.Once);

            moqed
                .Verify(
                    x => x.EffectPopulatedWithException(It.Is<TestException>(te => te == exception)), Times.Once);

            moqed
                .Verify(x => x.AsyncEffect(), Times.Once);
            moqed
                .Verify(x => x.ArgumentedEffectAsync(It.IsAny<object>()), Times.Once);
            moqed
                .Verify(x => x.AsyncCancellableEffect(It.Is<CancellationToken>(ct => ct == token)), Times.Once);
            moqed
                .Verify(x => x.CancellableArgumentedEffectAsync(It.IsAny<object>(),It.Is<CancellationToken>(ct => ct == token)), Times.Once);
            moqed
                .Verify(
                    x => x.CancellableEffectPopulatedWithExceptionAsync(It.Is<TestException>(te => te == exception),
                        It.Is<CancellationToken>(ct => ct == token)), Times.Once);
            moqed
                .Verify(
                    x => x.EffectPopulatedWithExceptionAsync(It.Is<TestException>(te => te == exception)), Times.Once);
        }
        

        public interface ISideEffectsWrapper<T>
        {
            void Effect();
            void ArgumentedEffect(T arg);
            void CancellableEffect(CancellationToken cancellationToken);

            void CancellableArgumentedEffect(T arg, CancellationToken cancellationToken);
            
            void EffectPopulatedWithException(Exception exception);

            void CancellableEffectPopulatedWithException(Exception exception,
                CancellationToken cancellationToken);

            Task AsyncEffect();

            Task AsyncCancellableEffect(CancellationToken cancellationToken);

            Task EffectPopulatedWithExceptionAsync(Exception exception);

            Task CancellableEffectPopulatedWithExceptionAsync(Exception exception,
                CancellationToken cancellationToken);

            Task ArgumentedEffectAsync(T arg);

            Task CancellableArgumentedEffectAsync(T arg, CancellationToken cancellationToken);
        }
        
    }
}