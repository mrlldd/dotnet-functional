using Bogus;
using NUnit.Framework;
    
namespace mrlldd.Functional.Tests.Core
{
    [TestFixture]
    public abstract class TestFixtureBase
    {
        protected readonly Faker Faker = new();
    }
}