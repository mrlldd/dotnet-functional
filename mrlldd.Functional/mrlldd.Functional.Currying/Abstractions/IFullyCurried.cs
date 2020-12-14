using System;

namespace mrlldd.Functional.Currying.Abstractions
{
    public interface IFullyCurried<out T> : IContainsDelegate<T>, ICurryResult where T : Delegate
    {
    }
}