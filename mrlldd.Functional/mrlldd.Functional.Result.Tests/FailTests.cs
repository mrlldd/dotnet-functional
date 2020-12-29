using FluentAssertions;
using mrlldd.Functional.Result.Extensions;
using mrlldd.Functional.Tests.Core;
using mrlldd.Functional.Tests.Core.Exceptions;
using mrlldd.Functional.Tests.Core.Internal.Extensions;
using NUnit.Framework;

namespace mrlldd.Functional.Result.Tests
{
    public class GenericFailTests : TestFixtureBase
    {
        [Test]
        public void FailAlwaysNotSuccessful() 
            => new TestException()
                .AsFail<object>().Successful
                    .Should()
                    .BeFalse();

        [Test]
        public void FailAlwaysStringifiesLikeThat() 
            => new TestException()
                .Map(x => x.AsFail<object>()
                    .ToString()
                    .Should()
                    .BeEquivalentTo($"Success: false, exception: {x}"));
    }

    public class ValuelessFailTests : TestFixtureBase
    {
        [Test]
        public void FailAlwaysNotSuccessful()
            => new TestException()
                .AsFail().Successful
                .Should()
                .BeFalse();
        
        [Test]
        public void FailAlwaysStringifiesLikeThat() 
            => new TestException()
                .Map(x => x.AsFail()
                    .ToString()
                    .Should()
                    .BeEquivalentTo($"Success: false, exception: {x}"));
    }
}