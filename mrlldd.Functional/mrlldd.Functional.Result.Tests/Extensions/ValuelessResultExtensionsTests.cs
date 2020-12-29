using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
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
                .Bind(token => { token.ThrowIfCancellationRequested(); }, tokenSource.Token);
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
                    .ContinueWith(_ => throw new TestException())
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
                    .ContinueWith(_ => throw new Exception())
                )
                .ContinueWith(task => task.Result
                    .Should()
                    .BeAResult<Fail>()
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
                    .ContinueWith(_ => throw new TestException())
                )
                .ContinueWith(task => task.Result
                    .Should()
                    .BeAResult<Fail>());

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
                    .ContinueWith(_ => throw new TestException(), token), tokenSource.Token);
            result
                .Should()
                .BeAResult<Fail>();
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

        [Test]
        public async Task PerformsSideEffectOnSuccessfulResults()
        {
            var moqed = new Mock<ISideEffectsWrapper>();
            var token = new CancellationTokenSource().Token;
            moqed
                .Setup(x => x.Effect())
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))))
                .Verifiable();
            moqed
                .Setup(x => x.AsyncEffect())
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))))
                .Verifiable();

            Result.Success
                .EffectIfSuccessful(moqed.Object.Effect)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(Result.Success))
                .EffectIfSuccessful(moqed.Object.CancellableEffect, token)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(Result.Success));
            await Result.Success
                .EffectIfSuccessfulAsync(moqed.Object.AsyncEffect)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(Result.Success));
            await Result.Success
                .EffectIfSuccessfulAsync(moqed.Object.AsyncCancellableEffect, token)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(Result.Success));
            moqed
                .Verify(x => x.Effect(), Times.Once);
            moqed
                .Verify(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))), Times.Once);
            moqed
                .Verify(x => x.AsyncEffect(), Times.Once);
            moqed
                .Verify(x => x.AsyncCancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))), Times.Once);
        }

        [Test]
        public async Task NotPerformsSideEffectOnSuccessfulResults()
        {
            var moqed = new Mock<ISideEffectsWrapper>();
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
            Result.Success
                .EffectIfNotSuccessful(moqed.Object.Effect)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(Result.Success))
                .EffectIfNotSuccessful(moqed.Object.CancellableEffect, CancellationToken.None)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(Result.Success))
                .EffectIfNotSuccessful(moqed.Object.EffectPopulatedWithException)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(Result.Success))
                .EffectIfNotSuccessful(moqed.Object.CancellableEffectPopulatedWithException, CancellationToken.None)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(Result.Success));
            await Result.Success
                .EffectIfNotSuccessfulAsync(moqed.Object.AsyncEffect)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(Result.Success));
            await Result.Success
                .EffectIfNotSuccessfulAsync(moqed.Object.CancellableEffectPopulatedWithExceptionAsync,
                    CancellationToken.None)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(Result.Success));
            await Result.Success
                .EffectIfNotSuccessfulAsync(moqed.Object.AsyncCancellableEffect, CancellationToken.None);
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
            var moqed = new Mock<ISideEffectsWrapper>();
            var token = new CancellationTokenSource().Token;
            moqed
                .Setup(x => x.Effect())
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))))
                .Verifiable();
            moqed
                .Setup(x => x.AsyncEffect())
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))))
                .Verifiable();
            var fail = new TestException()
                .AsFail();
            fail
                .EffectIfSuccessful(moqed.Object.Effect)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail))
                .EffectIfSuccessful(moqed.Object.CancellableEffect, token)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail));
            await fail
                .EffectIfSuccessfulAsync(moqed.Object.AsyncEffect)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            await fail
                .EffectIfSuccessfulAsync(moqed.Object.AsyncCancellableEffect, token)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            moqed
                .Verify(x => x.Effect(), Times.Never);
            moqed
                .Verify(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))), Times.Never);
            moqed
                .Verify(x => x.AsyncEffect(), Times.Never);
            moqed
                .Verify(x => x.AsyncCancellableEffect(It.Is<CancellationToken>(ct => ct.Equals(token))), Times.Never);
        }

        [Test]
        public async Task PerformsSideEffectOnNotSuccessfulResults()
        {
            var moqed = new Mock<ISideEffectsWrapper>();
            var token = new CancellationTokenSource().Token;
            moqed
                .Setup(x => x.Effect())
                .Verifiable();
            moqed
                .Setup(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct == token)))
                .Verifiable();
            moqed
                .Setup(x => x.AsyncEffect())
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
                .AsFail();
            fail
                .EffectIfNotSuccessful(moqed.Object.Effect)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail))
                .EffectIfNotSuccessful(moqed.Object.CancellableEffect, token)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail))
                .EffectIfNotSuccessful(moqed.Object.EffectPopulatedWithException)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail))
                .EffectIfNotSuccessful(moqed.Object.CancellableEffectPopulatedWithException, token)
                .SideEffect(x => x
                    .Should()
                    .BeSameAs(fail));

            await fail
                .EffectIfNotSuccessfulAsync(moqed.Object.AsyncEffect)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            await fail
                .EffectIfNotSuccessfulAsync(moqed.Object.AsyncCancellableEffect, token)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            await fail
                .EffectIfNotSuccessfulAsync(moqed.Object.CancellableEffectPopulatedWithExceptionAsync, token)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            await fail
                .EffectIfNotSuccessfulAsync(moqed.Object.EffectPopulatedWithExceptionAsync)
                .ContinueWith(x => x.Result
                    .Should()
                    .BeSameAs(fail));
            moqed
                .Verify(x => x.Effect(), Times.Once);
            moqed
                .Verify(x => x.CancellableEffect(It.Is<CancellationToken>(ct => ct == token)), Times.Once);
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
                .Verify(x => x.AsyncCancellableEffect(It.Is<CancellationToken>(ct => ct == token)), Times.Once);
            moqed
                .Verify(
                    x => x.CancellableEffectPopulatedWithExceptionAsync(It.Is<TestException>(te => te == exception),
                        It.Is<CancellationToken>(ct => ct == token)), Times.Once);
            moqed
                .Verify(
                    x => x.EffectPopulatedWithExceptionAsync(It.Is<TestException>(te => te == exception)), Times.Once);
        }

        public interface ISideEffectsWrapper
        {
            void Effect();
            void CancellableEffect(CancellationToken cancellationToken);

            void EffectPopulatedWithException(Exception exception);


            void CancellableEffectPopulatedWithException(Exception exception,
                CancellationToken cancellationToken);

            Task AsyncEffect();

            Task AsyncCancellableEffect(CancellationToken cancellationToken);

            Task EffectPopulatedWithExceptionAsync(Exception exception);

            Task CancellableEffectPopulatedWithExceptionAsync(Exception exception,
                CancellationToken cancellationToken);
        }
    }
}