using System;
using Functional.Currying.Abstractions;

namespace Functional.Currying.Internal.Curried.Actions
{
    internal class CurriedAction<T> : Curried<Action<T>>, ICurriedAction<T>
    {
        public CurriedAction(Action<T> source) : base(source)
        {
        }

        public IFullyCurried<Action> With(T argument)
            => new FullyCurriedAction<T>(Source, argument);

        public Action<T> Delegate => Source;
    }

    internal sealed class FullyCurriedAction<T> : Curried<Action<T>>, IFullyCurried<Action>
    {
        private readonly T argument;
        public Action Delegate => () => Source(argument);

        public FullyCurriedAction(Action<T> source, T argument) : base(source)
        {
            this.argument = argument;
        }
    }
}