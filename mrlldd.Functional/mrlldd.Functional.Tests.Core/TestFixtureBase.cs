using Bogus;
using NUnit.Framework;

namespace Functional.Tests.Core
{
    [TestFixture]
    public abstract class TestFixtureBase
    {
        protected readonly Faker Faker = new();
    }
}