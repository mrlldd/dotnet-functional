using FluentAssertions;
using mrlldd.Functional.Result.Extensions;
using mrlldd.Functional.Tests.Core;
using mrlldd.Functional.Tests.Core.Internal.Extensions;
using NUnit.Framework;

namespace mrlldd.Functional.Result.Tests
{
    public class GenericSuccessTests : TestFixtureBase
    {
        [Test]
        public void AlwaysSuccessful() 
            => Faker.Random.Number()
                .AsSuccess()
                .SideEffects(x => x.Successful
                    .Should()
                    .BeTrue());

        [Test]
        public void AlwaysStringifiesLikeThat() 
            => Faker.Random.Number()
                .Map(x => x.AsSuccess()
                    .ToString()
                    .Should()
                    .BeEquivalentTo($"Success: true, value: {x}"));
    }
    
    public class ValuelessSuccessTests : TestFixtureBase
    {
        [Test]
        public void AlwaysSuccessful() 
            => Result.Success.Successful
                    .Should()
                    .BeTrue();

        [Test]
        public void AlwaysStringifiesLikeThat() 
            => Result.Success
                    .ToString()
                    .Should()
                    .BeEquivalentTo("Success: true");
    }
}