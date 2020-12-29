using System;

namespace Functional.Currying.Abstractions
{
    public interface IContainsDelegate<out TDelegate> where TDelegate : Delegate
    {
        TDelegate Delegate { get; }
    }
}