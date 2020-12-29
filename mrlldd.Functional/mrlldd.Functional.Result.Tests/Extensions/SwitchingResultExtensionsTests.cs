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
// ReSharper disable EmptyForStatement

// ReSharper disable NotAccessedVariable

namespace mrlldd.Functional.Result.Tests.Extensions
{
    public class SwitchingResultExtensionsTests : TestFixtureBase
    {
        [Test]
        public void BindsGenericSuccessToValuelessSuccess()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(() =>
                {
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                    }
                })
                .ShouldMuch(x => x.BeAResult<Success>(),
                    x => x.BeSameAs(Result.Success));

        [Test]
        public void BindsGenericSuccessToValuelessFail()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(new Action(() =>
                {
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                    }

                    throw new TestException();
                }))
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x
                        .UnwrapAsFail()
                        .Should()
                        .BeOfType<TestException>());

        [Test]
        public void NotBindsGenericFailToValuelessResult()
        {
            var exception = new TestException();
            exception
                .AsFail<object>()
                .Bind(new Action(() =>
                {
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                    }

                    throw new Exception();
                }))
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x
                        .UnwrapAsFail()
                        .ShouldMuch(y => y.BeOfType<TestException>(),
                            y => y.BeSameAs(exception)));
        }

        [Test]
        public void BindsGenericSuccessToValuelessSuccessWithCancellation()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(ct =>
                {
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                    }

                    ct.ThrowIfCancellationRequested();
                }, new CancellationTokenSource().Token)
                .ShouldMuch(x => x.BeAResult<Success>(),
                    x => x.BeSameAs(Result.Success));

        [Test]
        public void BindsGenericSuccessToValuelessFailWithCancellation()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(new Action<CancellationToken>(ct =>
                {
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                    }

                    ct.ThrowIfCancellationRequested();
                    throw new TestException();
                }), new CancellationTokenSource().Token)
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x
                        .As<Fail>().Exception
                        .Should()
                        .BeOfType<TestException>());

        [Test]
        public void BindsGenericSuccessToCanceledValuelessFailWithCancellation()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(ct =>
                {
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                    }

                    ct.ThrowIfCancellationRequested();
                }, new CancellationTokenSource()
                    .SideEffect(x => x.Cancel()).Token)
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x
                        .As<Fail>().Exception
                        .Should()
                        .BeOfType<OperationCanceledException>());

        [Test]
        public void NotBindsGenericFailToValuelessResultWithCancellation()
        {
            var exception = new TestException();
            exception
                .AsFail<object>()
                .Bind(ct =>
                {
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                    }

                    ct.ThrowIfCancellationRequested();
                }, new CancellationTokenSource().Token)
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x
                        .UnwrapAsFail()
                        .ShouldMuch(y => y.BeOfType<TestException>(),
                            y => y.BeSameAs(exception)));
        }

        [Test]
        public void BindsGenericSuccessToValuelessSuccessWithArgument()
        {
            Faker.Random
                .Number()
                .AsSuccess()
                .Bind(x =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }
                })
                .ShouldMuch(x => x.BeAResult<Success>(),
                    x => x.BeSameAs(Result.Success));
        }

        [Test]
        public void BindsGenericSuccessToValuelessFailWithArgument()
        {
            Faker.Random
                .Number()
                .AsSuccess()
                .Bind(x =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    ThrowTestException();
                })
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x
                        .UnwrapAsFail()
                        .Should()
                        .BeOfType<TestException>());
        }

        [Test]
        public void NotBindsGenericFailToValuelessResultWithArgument()
        {
            var exception = new TestException();
            exception
                .AsFail<int>()
                .Bind(x =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    ThrowTestException();
                })
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x
                        .UnwrapAsFail()
                        .ShouldMuch(y => y.BeOfType<TestException>(),
                            y => y.BeSameAs(exception)));
        }

        [Test]
        public void BindsGenericSuccessToValuelessSuccessWithArgumentAndCancellation()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind((x, ct) =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    ct.ThrowIfCancellationRequested();
                }, new CancellationTokenSource().Token)
                .ShouldMuch(x => x.BeAResult<Success>(),
                    x => x.BeSameAs(Result.Success));

        [Test]
        public void BindsGenericSuccessToValuelessFailWithArgumentAndCancellation()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind(new Action<int, CancellationToken>((x, ct) =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    ct.ThrowIfCancellationRequested();
                    throw new TestException();
                }), new CancellationTokenSource().Token)
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x
                        .As<Fail>().Exception
                        .Should()
                        .BeOfType<TestException>());

        [Test]
        public void BindsGenericSuccessToCanceledValuelessFailWithArgumentAndCancellation()
            => Faker.Random
                .Number()
                .AsSuccess()
                .Bind((x, ct) =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    ct.ThrowIfCancellationRequested();
                }, new CancellationTokenSource()
                    .SideEffect(x => x.Cancel()).Token)
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x
                        .As<Fail>().Exception
                        .Should()
                        .BeOfType<OperationCanceledException>());

        [Test]
        public void NotBindsGenericFailToValuelessResultWithArgumentAndCancellation()
        {
            var exception = new TestException();
            exception
                .AsFail<int>()
                .Bind((x, ct) =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    ct.ThrowIfCancellationRequested();
                }, new CancellationTokenSource().Token)
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x
                        .UnwrapAsFail()
                        .ShouldMuch(y => y.BeOfType<TestException>(),
                            y => y.BeSameAs(exception)));
        }

        [Test]
        public async Task BindsGenericSuccessToValuelessSuccessTask()
        {
            var result = await Faker.Random
                .Number()
                .AsSuccess()
                .Bind(() => Task.Delay(50));
            result
                .Should()
                .BeAResult<Success>();
        }

        [Test]
        public async Task BindsGenericSuccessToValuelessFailTask()
        {
            var result = await Faker.Random
                .Number()
                .AsSuccess()
                .Bind(async () =>
                {
                    await Task.Delay(50);
                    throw new TestException();
                });
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .SideEffects(e => e
                                .Should()
                                .BeOfType<AggregateException>(),
                            e => e.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TestException>()));
        }

        [Test]
        public async Task NotBindsGenericFailToValuelessResultTask()
        {
            var result = await new TestException()
                .AsFail<object>()
                .Bind(async () =>
                {
                    await Task.Delay(50);
                    throw new TestException();
                });
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeOfType<TestException>());
        }

        [Test]
        public async Task BindsGenericSuccessToValuelessSuccessTaskWithCancellation()
        {
            var result = await Faker.Random
                .Number()
                .AsSuccess()
                .Bind(ct => Task.Delay(50, ct), new CancellationTokenSource().Token);
            result
                .Should()
                .BeAResult<Success>();
        }

        [Test]
        public async Task BindsGenericSuccessToValuelessFailTaskWithCancellation()
        {
            var result = await Faker.Random
                .Number()
                .AsSuccess()
                .Bind(async ct =>
                {
                    await Task.Delay(50, ct);
                    throw new TestException();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .SideEffects(e => e
                                .Should()
                                .BeOfType<AggregateException>(),
                            e => e.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TestException>()));
        }

        [Test]
        public async Task BindsGenericSuccessToCanceledValuelessFailTaskWithCancellation()
        {
            var result = await Faker.Random
                .Number()
                .AsSuccess()
                .Bind(async ct =>
                {
                    await Task.Delay(50, ct);
                    throw new TestException();
                }, new CancellationTokenSource()
                    .SideEffect(x => x.Cancel()).Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .SideEffects(e => e
                                .Should()
                                .BeOfType<AggregateException>(),
                            e => e.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TaskCanceledException>()));
        }

        [Test]
        public async Task NotBindsGenericFailToValuelessResultTaskWithCancellation()
        {
            var exception = new TestException();
            var result = await exception
                .AsFail<object>()
                .Bind(async ct =>
                {
                    await Task.Delay(50, ct);
                    throw new TestException();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeSameAs(exception));
        }

        [Test]
        public async Task BindsGenericSuccessToValuelessSuccessTaskWithArgument()
        {
            var result = await Faker.Random
                .Number()
                .AsSuccess()
                .Bind(async x =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    await Task.Delay(50);
                });
            result
                .Should()
                .BeAResult<Success>();
        }

        [Test]
        public async Task BindsGenericSuccessToValuelessFailTaskWithArgument()
        {
            var result = await Faker.Random
                .Number()
                .AsSuccess()
                .Bind(async x =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    await Task.Delay(50);
                    ThrowTestException();
                });
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .SideEffects(e => e
                                .Should()
                                .BeOfType<AggregateException>(),
                            e => e.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TestException>()));
        }

        [Test]
        public async Task NotBindsGenericFailToValuelessResultTaskWithArgument()
        {
            var result = await new TestException()
                .AsFail<int>()
                .Bind(async x =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    await Task.Delay(50);
                    ThrowTestException();
                });
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeOfType<TestException>());
        }

        [Test]
        public async Task BindsGenericSuccessToValuelessSuccessTaskWithArgumentAndCancellation()
        {
            var result = await Faker.Random
                .Number()
                .AsSuccess()
                .Bind(async (x, ct) =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    await Task.Delay(50, ct);
                }, new CancellationTokenSource().Token);
            result
                .Should()
                .BeAResult<Success>();
        }

        [Test]
        public async Task BindsGenericSuccessToValuelessFailTaskWithArgumentAndCancellation()
        {
            var result = await Faker.Random
                .Number()
                .AsSuccess()
                .Bind(async (x, ct) =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    await Task.Delay(50, ct);
                    throw new TestException();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .SideEffects(e => e
                                .Should()
                                .BeOfType<AggregateException>(),
                            e => e.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TestException>()));
        }

        [Test]
        public async Task BindsGenericSuccessToCanceledValuelessFailTaskWithArgumentAndCancellation()
        {
            var result = await Faker.Random
                .Number()
                .AsSuccess()
                .Bind(async (x, ct) =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    await Task.Delay(50, ct);
                    throw new TestException();
                }, new CancellationTokenSource()
                    .SideEffect(x => x.Cancel()).Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .SideEffects(e => e
                                .Should()
                                .BeOfType<AggregateException>(),
                            e => e.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TaskCanceledException>()));
        }

        [Test]
        public async Task NotBindsGenericFailToValuelessResultTaskWithArgumentAndCancellation()
        {
            var result = await new TestException()
                .AsFail<int>()
                .Bind(async (x, ct) =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    await Task.Delay(50, ct);
                    ThrowTestException();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeOfType<TestException>());
        }

        [Test]
        public void BindsValuelessSuccessToGenericSuccess()
            => Result.Success
                .Bind(() => Faker.Random.Number())
                .Should()
                .BeASuccess<int>();

        [Test]
        public void BindsValuelessSuccessToGenericFail()
            => Result.Success
                .Bind(() =>
                {
                    ThrowTestException();
                    return Faker.Random.Number();
                })
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .Should()
                        .BeOfType<TestException>());

        [Test]
        public void NotBindsValuelessFailToGenericResult()
        {
            var exception = new TestException();
            exception
                .AsFail()
                .Bind(() =>
                {
                    ThrowTestException();
                    return Faker.Random.Number();
                })
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .Should()
                        .BeSameAs(exception));
        }

        [Test]
        public void BindsValuelessSuccessToGenericSuccessWithCancellation()
            => Result.Success
                .Bind(ct =>
                {
                    ct.ThrowIfCancellationRequested();
                    return Faker.Random.Number();
                }, new CancellationTokenSource().Token)
                .Should()
                .BeASuccess<int>();

        [Test]
        public void BindsValuelessSuccessToGenericFailWithCancellation()
            => Result.Success
                .Bind(ct =>
                {
                    ct.ThrowIfCancellationRequested();
                    ThrowTestException();
                    return Faker.Random.Number();
                }, new CancellationTokenSource().Token)
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .Should()
                        .BeOfType<TestException>());

        [Test]
        public void BindsValuelessSuccessToCanceledGenericFailWithCancellation()
            => Result.Success
                .Bind(ct =>
                {
                    ct.ThrowIfCancellationRequested();
                    ThrowTestException();
                    return Faker.Random.Number();
                }, new CancellationTokenSource().SideEffect(x => x.Cancel()).Token)
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .Should()
                        .BeOfType<OperationCanceledException>());

        [Test]
        public void NotBindsValuelessFailToGenericResultWithCancellation()
        {
            var exception = new TestException();
            exception
                .AsFail()
                .Bind(ct =>
                {
                    ct.ThrowIfCancellationRequested();
                    ThrowTestException();
                    return Faker.Random.Number();
                }, new CancellationTokenSource().Token)
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .Should()
                        .BeSameAs(exception));
        }

        [Test]
        public async Task BindsValuelessSuccessToGenericSuccessTask()
        {
            var result = await Result.Success
                .Bind(async () =>
                {
                    await Task.Delay(50);
                    return Faker.Random.Number();
                });
            result
                .Should()
                .BeASuccess<int>();
        }

        [Test]
        public async Task BindsValuelessSuccessToGenericFailTask()
        {
            var result = await Result.Success
                .Bind(async () =>
                {
                    await Task.Delay(50);
                    ThrowTestException();
                    return Faker.Random.Number();
                });
            result
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .SideEffects(e => e
                                .Should()
                                .BeOfType<AggregateException>(),
                            e => e.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TestException>()));
        }

        [Test]
        public async Task NotBindsValuelessFailToGenericResultTask()
        {
            var exception = new TestException();
            var result = await exception
                .AsFail()
                .Bind(async () =>
                {
                    await Task.Delay(50);
                    return Faker.Random.Number();
                });
            result
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .Should()
                        .BeSameAs(exception));
        }

        [Test]
        public async Task BindsValuelessSuccessToGenericSuccessTaskWithCancellation()
        {
            var result = await Result.Success
                .Bind(async ct =>
                {
                    await Task.Delay(50, ct);
                    return Faker.Random.Number();
                }, new CancellationTokenSource().Token);
            result
                .Should()
                .BeASuccess<int>();
        }

        [Test]
        public async Task BindsValuelessSuccessToGenericFailTaskWithCancellation()
        {
            var result = await Result.Success
                .Bind(async ct =>
                {
                    await Task.Delay(50, ct);
                    ThrowTestException();
                    return Faker.Random.Number();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .SideEffects(e => e
                                .Should()
                                .BeOfType<AggregateException>(),
                            e => e.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TestException>()));
        }

        [Test]
        public async Task BindsValuelessSuccessToCanceledGenericFailTaskWithCancellation()
        {
            var result = await Result.Success
                .Bind(async ct =>
                {
                    await Task.Delay(Timeout.InfiniteTimeSpan, ct);
                    ThrowTestException();
                    return Faker.Random.Number();
                }, new CancellationTokenSource().SideEffect(x => x.Cancel()).Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .SideEffects(e => e
                                .Should()
                                .BeOfType<AggregateException>(),
                            e => e.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TaskCanceledException>()));
        }

        [Test]
        public async Task NotBindsValuelessFailToGenericResultTaskWithCancellation()
        {
            var exception = new TestException();
            var result = await exception
                .AsFail()
                .Bind(async ct =>
                {
                    await Task.Delay(50, ct);
                    return Faker.Random.Number();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .Should()
                        .BeSameAs(exception));
        }

        [Test]
        public async Task BindsGenericSuccessTaskToValuelessSuccessTask()
        {
            var result = await Task
                .FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(() => Task.Delay(50));

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Success>(),
                    x => x
                        .Should()
                        .BeSameAs(Result.Success));
        }

        [Test]
        public async Task BindsGenericSuccessTaskToValuelessFailTask()
        {
            var result = await Task
                .FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(async () =>
                {
                    await Task.Delay(50);
                    ThrowTestException();
                });

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .SideEffects(y => y
                                .Should()
                                .BeOfType<AggregateException>(),
                            y => y.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TestException>()));
        }

        [Test]
        public async Task NotBindsGenericSuccessTaskToValuelessResultTask()
        {
            var exception = new TestException();
            var result = await Task
                .FromResult(exception.AsFail<object>())
                .Bind(() => Task.Delay(50));

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeSameAs(exception));
        }

        [Test]
        public async Task BindsGenericSuccessTaskToValuelessSuccessTaskWithCancellation()
        {
            var result = await Task
                .FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(ct => Task.Delay(50, ct), new CancellationTokenSource().Token);

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Success>(),
                    x => x
                        .Should()
                        .BeSameAs(Result.Success));
        }

        [Test]
        public async Task BindsGenericSuccessTaskToValuelessFailTaskWithCancellation()
        {
            var result = await Task
                .FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(async ct =>
                {
                    await Task.Delay(50, ct);
                    ThrowTestException();
                }, new CancellationTokenSource().Token);

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .SideEffects(y => y
                                .Should()
                                .BeOfType<AggregateException>(),
                            y => y.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TestException>()));
        }

        [Test]
        public async Task BindsGenericSuccessTaskToCanceledValuelessFailTaskWithCancellation()
        {
            var result = await Task
                .FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(async ct =>
                {
                    await Task.Delay(Timeout.InfiniteTimeSpan, ct);
                    ThrowTestException();
                }, new CancellationTokenSource()
                    .SideEffect(x => x.Cancel()).Token);

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .SideEffects(y => y
                                .Should()
                                .BeOfType<AggregateException>(),
                            y => y.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TaskCanceledException>()));
        }

        [Test]
        public async Task NotBindsGenericSuccessTaskToValuelessResultTaskWithCancellation()
        {
            var exception = new TestException();
            var result = await Task
                .FromResult(exception.AsFail<object>())
                .Bind(ct => Task.Delay(50, ct), new CancellationTokenSource().Token);

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeSameAs(exception));
        }

        [Test]
        public async Task BindsGenericSuccessTaskToValuelessSuccessTaskWithArgument()
        {
            var result = await Task
                .FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(async x =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    await Task.Delay(50);
                });

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Success>(),
                    x => x
                        .Should()
                        .BeSameAs(Result.Success));
        }

        [Test]
        public async Task BindsGenericSuccessTaskToValuelessFailTaskWithArgument()
        {
            var result = await Task
                .FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(async x =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    await Task.Delay(50);
                    ThrowTestException();
                });

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .SideEffects(y => y
                                .Should()
                                .BeOfType<AggregateException>(),
                            y => y.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TestException>()));
        }

        [Test]
        public async Task NotBindsGenericSuccessTaskToValuelessFailTaskWithArgument()
        {
            var exception = new TestException();
            var result = await Task
                .FromResult(exception.AsFail<int>())
                .Bind(async x =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    await Task.Delay(50);
                });

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeSameAs(exception));
        }

        [Test]
        public async Task BindsGenericSuccessTaskToValuelessSuccessTaskWithArgumentAndCancellation()
        {
            var result = await Task
                .FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(async (x, ct) =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    await Task.Delay(50, ct);
                }, new CancellationTokenSource().Token);

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Success>(),
                    x => x
                        .Should()
                        .BeSameAs(Result.Success));
        }

        [Test]
        public async Task BindsGenericSuccessTaskToValuelessFailTaskWithArgumentAndCancellation()
        {
            var result = await Task
                .FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(async (x, ct) =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    await Task.Delay(50, ct);
                    ThrowTestException();
                }, new CancellationTokenSource().Token);

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .SideEffects(y => y
                                .Should()
                                .BeOfType<AggregateException>(),
                            y => y.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TestException>()));
        }

        [Test]
        public async Task BindsGenericSuccessTaskToCanceledValuelessFailTaskWithArgumentAndCancellation()
        {
            var result = await Task
                .FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(async (x, ct) =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    await Task.Delay(Timeout.InfiniteTimeSpan, ct);
                    ThrowTestException();
                }, new CancellationTokenSource()
                    .SideEffect(x => x.Cancel()).Token);

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .SideEffects(y => y
                                .Should()
                                .BeOfType<AggregateException>(),
                            y => y.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TaskCanceledException>()));
        }

        [Test]
        public async Task NotBindsGenericSuccessTaskToValuelessResultTaskWithArgumentAndCancellation()
        {
            var exception = new TestException();
            var result = await Task
                .FromResult(exception.AsFail<int>())
                .Bind(async (x, ct) =>
                {
                    var num = x;
                    // ReSharper disable once EmptyForStatement
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }

                    await Task.Delay(50, ct);
                }, new CancellationTokenSource().Token);

            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeSameAs(exception));
        }

        [Test]
        public async Task BindsValuelessSuccessTaskToGenericSuccessTask()
        {
            var result = await Task
                .FromResult(Result.Success)
                .Bind(async () =>
                {
                    await Task.Delay(50);
                    return Faker.Random.Number();
                });
            result
                .Should()
                .BeASuccess<int>();
        }

        [Test]
        public async Task BindsValuelessSuccessTaskToGenericFailTask()
        {
            var result = await Task
                .FromResult(Result.Success)
                .Bind(async () =>
                {
                    await Task.Delay(50);
                    ThrowTestException();
                    return Faker.Random.Number();
                });
            result
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .SideEffects(e => e
                                .Should()
                                .BeOfType<AggregateException>(),
                            e => e.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TestException>()));
        }

        [Test]
        public async Task NotBindsValuelessFailTaskToGenericResultTask()
        {
            var exception = new TestException();
            var result = await Task
                .FromResult(exception.AsFail())
                .Bind(async () =>
                {
                    await Task.Delay(50);
                    ThrowTestException();
                    return Faker.Random.Number();
                });
            result
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .Should()
                        .BeSameAs(exception));
        }
        
        [Test]
        public async Task BindsValuelessSuccessTaskToGenericSuccessTaskWithCancellation()
        {
            var result = await Task
                .FromResult(Result.Success)
                .Bind(async ct =>
                {
                    await Task.Delay(50, ct);
                    return Faker.Random.Number();
                }, new CancellationTokenSource().Token);
            result
                .Should()
                .BeASuccess<int>();
        }

        [Test]
        public async Task BindsValuelessSuccessTaskToGenericFailTaskWithCancellation()
        {
            var result = await Task
                .FromResult(Result.Success)
                .Bind(async ct =>
                {
                    await Task.Delay(50, ct);
                    ThrowTestException();
                    return Faker.Random.Number();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .SideEffects(e => e
                                .Should()
                                .BeOfType<AggregateException>(),
                            e => e.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TestException>()));
        }
        
        [Test]
        public async Task BindsValuelessSuccessTaskToCanceledGenericFailTaskWithCancellation()
        {
            var result = await Task
                .FromResult(Result.Success)
                .Bind(async ct =>
                {
                    await Task.Delay(Timeout.InfiniteTimeSpan, ct);
                    ThrowTestException();
                    return Faker.Random.Number();
                }, new CancellationTokenSource().SideEffect(x => x.Cancel()).Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .SideEffects(e => e
                                .Should()
                                .BeOfType<AggregateException>(),
                            e => e.As<AggregateException>().InnerException
                                .Should()
                                .BeOfType<TaskCanceledException>()));
        }

        [Test]
        public async Task NotBindsValuelessFailTaskToGenericResultTaskWithCancellation()
        {
            var exception = new TestException();
            var result = await Task
                .FromResult(exception.AsFail())
                .Bind(async ct =>
                {
                    await Task.Delay(50, ct);
                    ThrowTestException();
                    return Faker.Random.Number();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .Should()
                        .BeSameAs(exception));
        }

        [Test]
        public async Task BindsGenericSuccessToValuelessSuccessSynchronously()
        {
            var result = await Task.FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(() =>
                {
                    for (var i = 0; i < 50; i++)
                    {
                    }
                });
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Success>(),
                    x => x
                        .Should()
                        .BeSameAs(Result.Success));
        }
        
        [Test]
        public async Task BindsGenericSuccessToValuelessFailSynchronously()
        {
            var result = await Task.FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(() =>
                {
                    for (var i = 0; i < 50; i++)
                    {
                    }
                    ThrowTestException();
                });
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeOfType<TestException>());
        }
        
        [Test]
        public async Task NotBindsGenericFailToValuelessResultSynchronously()
        {
            var exception = new TestException();
            var result = await Task.FromResult(exception.AsFail())
                .Bind(() =>
                {
                    for (var i = 0; i < 50; i++)
                    {
                    }
                });
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeSameAs(exception));
        }
        
        [Test]
        public async Task BindsGenericSuccessToValuelessSuccessWithCancellationSynchronously()
        {
            var result = await Task.FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(ct =>
                {
                    for (var i = 0; i < 50; i++)
                    {
                    }
                    ct.ThrowIfCancellationRequested();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Success>(),
                    x => x
                        .Should()
                        .BeSameAs(Result.Success));
        }
        
        [Test]
        public async Task BindsGenericSuccessToValuelessFailWithCancellationSynchronously()
        {
            var result = await Task.FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(ct =>
                {
                    for (var i = 0; i < 50; i++)
                    {
                    }

                    ct.ThrowIfCancellationRequested();
                    ThrowTestException();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeOfType<TestException>());
        }
        
        [Test]
        public async Task BindsGenericSuccessToCanceledValuelessFailWithCancellationSynchronously()
        {
            var result = await Task.FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(ct =>
                {
                    for (var i = 0; i < 50; i++)
                    {
                    }

                    ct.ThrowIfCancellationRequested();
                    ThrowTestException();
                }, new CancellationTokenSource().SideEffect(x => x.Cancel()).Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeOfType<OperationCanceledException>());
        }
        
        [Test]
        public async Task NotBindsGenericFailToValuelessResultWithCancellationSynchronously()
        {
            var exception = new TestException();
            var result = await Task.FromResult(exception.AsFail<int>())
                .Bind(ct =>
                {
                    for (var i = 0; i < 50; i++)
                    {
                    }
                    ct.ThrowIfCancellationRequested();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeSameAs(exception));
        }

        [Test]
        public async Task BindsValuelessSuccessToGenericSuccessSynchronously()
        {
            var result = await Task
                .FromResult(Result.Success)
                .Bind(() => Faker.Random.Number());
            result
                .Should()
                .BeASuccess<int>();
        }
        
        
        [Test]
        public async Task BindsValuelessSuccessToGenericFailSynchronously()
        {
            var result = await Task
                .FromResult(Result.Success)
                .Bind(() =>
                {
                    ThrowTestException();
                    return Faker.Random.Number();
                });
            result
                .SideEffects(x => x
                .Should()
                .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .Should()
                        .BeOfType<TestException>());
        }
        
        [Test]
        public async Task NotBindsValuelessSuccessToGenericResultSynchronously()
        {
            var exception = new TestException();
            var result = await Task
                .FromResult(exception.AsFail())
                .Bind(() =>
                {
                    ThrowTestException();
                    return Faker.Random.Number();
                });
            result
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .Should()
                        .BeSameAs(exception));
        }

        [Test]
        public async Task BindsGenericSuccessToValuelessSuccessWithArgumentSynchronously()
        {
            var result = await Task.FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(x =>
                {
                    var num = x;
                    for (var i = 0; i < 50; i++)
                    {
                        x++;
                    }
                });
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Success>(),
                    x => x
                        .Should()
                        .BeSameAs(Result.Success));
        }
        
        [Test]
        public async Task BindsGenericSuccessToValuelessFailWithArgumentSynchronously()
        {
            var result = await Task.FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind(x =>
                {
                    var num = x;
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }
                    ThrowTestException();
                });
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeOfType<TestException>());
        }
        
        [Test]
        public async Task NotBindsGenericFailToValuelessResultWithArgumentSynchronously()
        {
            var exception = new TestException();
            var result = await Task.FromResult(exception.AsFail<int>())
                .Bind(x =>
                {
                    var num = x;
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }
                });
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeSameAs(exception));
        }
        
        [Test]
        public async Task BindsGenericSuccessToValuelessSuccessWithArgumentAndCancellationSynchronously()
        {
            var result = await Task.FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind((x,ct) =>
                {
                    var num = x;
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }
                    ct.ThrowIfCancellationRequested();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Success>(),
                    x => x
                        .Should()
                        .BeSameAs(Result.Success));
        }
        
        [Test]
        public async Task BindsGenericSuccessToValuelessFailWithArgumentAndCancellationSynchronously()
        {
            var result = await Task.FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind((x,ct) =>
                {
                    var num = x;
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }
                    ct.ThrowIfCancellationRequested();
                    ThrowTestException();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeOfType<TestException>());
        }
        
        [Test]
        public async Task BindsGenericSuccessToCanceledValuelessFailWithArgumentAndCancellationSynchronously()
        {
            var result = await Task.FromResult(Faker.Random
                    .Number()
                    .AsSuccess())
                .Bind((x,ct) =>
                {
                    var num = x;
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }
                    ct.ThrowIfCancellationRequested();
                    ThrowTestException();
                }, new CancellationTokenSource().SideEffect(x => x.Cancel()).Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeOfType<OperationCanceledException>());
        }
        
        [Test]
        public async Task NotBindsGenericFailToValuelessResultWithArgumentAndCancellationSynchronously()
        {
            var exception = new TestException();
            var result = await Task.FromResult(exception.AsFail<int>())
                .Bind((x,ct) =>
                {
                    var num = x;
                    for (var i = 0; i < 50; i++)
                    {
                        num++;
                    }
                    ct.ThrowIfCancellationRequested();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAResult<Fail>(),
                    x => x.As<Fail>().Exception
                        .Should()
                        .BeSameAs(exception));
        }
        
        [Test]
        public async Task BindsValuelessSuccessToGenericSuccessWithCancellationSynchronously()
        {
            var result = await Task
                .FromResult(Result.Success)
                .Bind(ct =>
                {
                    ct.ThrowIfCancellationRequested();
                    return Faker.Random.Number();
                }, new CancellationTokenSource().Token);
            result
                .Should()
                .BeASuccess<int>();
        }
        
        
        [Test]
        public async Task BindsValuelessSuccessToGenericFailWithCancellationSynchronously()
        {
            var result = await Task
                .FromResult(Result.Success)
                .Bind(ct =>
                {
                    ct.ThrowIfCancellationRequested();
                    ThrowTestException();
                    return Faker.Random.Number();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .Should()
                        .BeOfType<TestException>());
        }
        
        [Test]
        public async Task BindsValuelessSuccessToCanceledGenericFailWithCancellationSynchronously()
        {
            var result = await Task
                .FromResult(Result.Success)
                .Bind(ct =>
                {
                    ct.ThrowIfCancellationRequested();
                    ThrowTestException();
                    return Faker.Random.Number();
                }, new CancellationTokenSource().SideEffect(x => x.Cancel()).Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .Should()
                        .BeOfType<OperationCanceledException>());
        }
        
        [Test]
        public async Task NotBindsValuelessSuccessToGenericResultWithCancellationSynchronously()
        {
            var exception = new TestException();
            var result = await Task
                .FromResult(exception.AsFail())
                .Bind(ct =>
                {
                    ct.ThrowIfCancellationRequested();
                    ThrowTestException();
                    return Faker.Random.Number();
                }, new CancellationTokenSource().Token);
            result
                .SideEffects(x => x
                        .Should()
                        .BeAFail<int>(),
                    x => x.As<Fail<int>>().Exception
                        .Should()
                        .BeSameAs(exception));
        }
        
        private static void ThrowTestException() => throw new TestException();
    }
    
}