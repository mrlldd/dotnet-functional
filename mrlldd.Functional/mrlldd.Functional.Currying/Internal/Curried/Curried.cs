using System;

namespace Functional.Currying.Internal.Curried
{
    internal abstract class Curried<TSourceDelegate> where TSourceDelegate : Delegate
    {
        protected readonly TSourceDelegate Source;

        protected Curried(TSourceDelegate source)
        {
            Source = source;
        }
    }
}