using System;

namespace Functional.Currying.Abstractions
{
    public interface IFullyCurried<out T> : IContainsDelegate<T>, ICurryResult where T : Delegate
    {
    }
}