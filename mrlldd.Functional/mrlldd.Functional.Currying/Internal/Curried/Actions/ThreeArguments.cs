using System;
using mrlldd.Functional.Currying.Abstractions;

namespace mrlldd.Functional.Currying.Internal.Curried.Actions
{
    internal class CurriedAction<T1, T2, T3> : Curried<Action<T1, T2, T3>>,
        ICurriedAction<T1, T2, T3>
    {
        public CurriedAction(Action<T1, T2, T3> source) : base(source)
        {
        }

        public IFullyCurried<Action> With(T1 firstArgument, T2 secondArgument, T3 thirdArgument)
            => new FullyCurriedAction<T1, T2, T3>(Source, firstArgument, secondArgument, thirdArgument);

        public ICurriedAction<T3> With(T1 firstArgument, T2 secondArgument)
            => new TwoArgumentsCurriedActionAction<T1, T2, T3>(Source, firstArgument, secondArgument);

        public ICurriedAction<T2, T3> With(T1 argument)
            => new OneArgumentCurriedAction<T1, T2, T3>(Source, argument);

        public Action<T1, T2, T3> Delegate => Source;
    }

    internal class OneArgumentCurriedAction<T1, T2, T3> : Curried<Action<T1, T2, T3>>,
        ICurriedAction<T2, T3>
    {
        private readonly T1 first;

        public OneArgumentCurriedAction(Action<T1, T2, T3> source, T1 first) : base(source)
        {
            this.first = first;
        }

        public IFullyCurried<Action> With(T2 firstArgument, T3 secondArgument)
            => new FullyCurriedAction<T1, T2, T3>(Source, first, firstArgument, secondArgument);

        public ICurriedAction<T3> With(T2 argument)
            => new TwoArgumentsCurriedActionAction<T1, T2, T3>(Source, first, argument);

        public Action<T2, T3> Delegate => (x, y) => Source(first, x, y);
    }

    internal class TwoArgumentsCurriedActionAction<T1, T2, T3> : Curried<Action<T1, T2, T3>>, ICurriedAction<T3>
    {
        private readonly T1 first;
        private readonly T2 second;

        public TwoArgumentsCurriedActionAction(Action<T1, T2, T3> source, T1 first, T2 second) : base(source)
        {
            this.first = first;
            this.second = second;
        }

        public IFullyCurried<Action> With(T3 argument)
            => new FullyCurriedAction<T1, T2, T3>(Source, first, second, argument);

        public Action<T3> Delegate => x => Source(first, second, x);
    }

    internal class FullyCurriedAction<T1, T2, T3> : Curried<Action<T1, T2, T3>>, IFullyCurried<Action>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;

        public FullyCurriedAction(Action<T1, T2, T3> source, T1 first, T2 second, T3 third) : base(source)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        public Action Delegate => () => Source(first, second, third);
    }
}