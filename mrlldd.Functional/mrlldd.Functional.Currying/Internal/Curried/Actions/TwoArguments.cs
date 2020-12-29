using System;
using Functional.Currying.Abstractions;

namespace Functional.Currying.Internal.Curried.Actions
{
    internal class CurriedAction<T1, T2> : Curried<Action<T1, T2>>, ICurriedAction<T1, T2>
    {
        public CurriedAction(Action<T1, T2> source) : base(source)
        {
        }

        public IFullyCurried<Action> With(T1 firstArgument, T2 secondArgument)
            => new FullyCurriedAction<T1, T2>(Source, firstArgument, secondArgument);

        public ICurriedAction<T2> With(T1 argument)
            => new ArgumentedCurriedActionAction<T1, T2>(Source, argument);

        public Action<T1, T2> Delegate => Source;
    }

    internal class ArgumentedCurriedActionAction<T1, T2> : Curried<Action<T1, T2>>, ICurriedAction<T2>
    {
        private readonly T1 first;

        public ArgumentedCurriedActionAction(Action<T1, T2> source, T1 first) : base(source)
        {
            this.first = first;
        }

        public IFullyCurried<Action> With(T2 argument)
            => new FullyCurriedAction<T1, T2>(Source, first, argument);

        public Action<T2> Delegate => x => Source(first, x);
    }

    internal class FullyCurriedAction<T1, T2> : Curried<Action<T1, T2>>, IFullyCurried<Action>
    {
        private readonly T1 first;
        private readonly T2 second;

        public FullyCurriedAction(Action<T1, T2> source, T1 first, T2 second) : base(source)
        {
            this.first = first;
            this.second = second;
        }

        public Action Delegate => () => Source(first, second);
    }
}