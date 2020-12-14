using System;
using mrlldd.Functional.Currying.Abstractions;

namespace mrlldd.Functional.Currying.Internal.Curried.Actions
{
    internal class CurriedAction<T1, T2, T3, T4, T5> : Curried<Action<T1, T2, T3, T4, T5>>,
        ICurriedAction<T1, T2, T3, T4, T5>
    {
        public CurriedAction(Action<T1, T2, T3, T4, T5> source) : base(source)
        {
        }

        public IFullyCurried<Action> With(T1 first,
            T2 second,
            T3 third,
            T4 fourth,
            T5 fifth)
            => new FullyCurriedAction<T1, T2, T3, T4, T5>(Source, first, second, third, fourth, fifth);

        public ICurriedAction<T5> With(T1 first,
            T2 second,
            T3 third,
            T4 fourth)
            => new FourArgumentsCurriedActionAction<T1, T2, T3, T4, T5>(Source, first, second, third, fourth);

        public ICurriedAction<T4, T5> With(T1 first,
            T2 second,
            T3 third)
            => new ThreeArgumentsCurriedAction<T1, T2, T3, T4, T5>(Source, first, second, third);

        public ICurriedAction<T3, T4, T5> With(T1 first,
            T2 second)
            => new TwoArgumentsCurriedAction<T1, T2, T3, T4, T5>(Source, first, second);

        public ICurriedAction<T2, T3, T4, T5> With(T1 argument)
            => new OneArgumentCurriedAction<T1, T2, T3, T4, T5>(Source, argument);

        public Action<T1, T2, T3, T4, T5> Delegate => Source;
    }

    internal class OneArgumentCurriedAction<T1, T2, T3, T4, T5> : Curried<Action<T1, T2, T3, T4, T5>>,
        ICurriedAction<T2, T3, T4, T5>
    {
        private readonly T1 first;

        public OneArgumentCurriedAction(Action<T1, T2, T3, T4, T5> source, T1 first) : base(source)
        {
            this.first = first;
        }
        public IFullyCurried<Action> With(T2 second, T3 third, T4 fourth, T5 fifth)
            => new FullyCurriedAction<T1, T2, T3, T4, T5>(Source, first, second, third, fourth, fifth);

        public ICurriedAction<T5> With(T2 second, T3 third, T4 fourth)
            => new FourArgumentsCurriedActionAction<T1, T2, T3, T4, T5>(Source, first, second, third, fourth);

        public ICurriedAction<T4, T5> With(T2 second, T3 third)
            => new ThreeArgumentsCurriedAction<T1, T2, T3, T4, T5>(Source, first, second, third);

        public ICurriedAction<T3, T4, T5> With(T2 argument)
            => new TwoArgumentsCurriedAction<T1, T2, T3, T4, T5>(Source, first, argument);

        public Action<T2, T3, T4, T5> Delegate => (x, y, z, u) => Source(first, x, y, z, u);
    }

    internal class TwoArgumentsCurriedAction<T1, T2, T3, T4, T5> : Curried<Action<T1, T2, T3, T4, T5>>,
        ICurriedAction<T3, T4, T5>
    {
        private readonly T1 first;
        private readonly T2 second;

        public TwoArgumentsCurriedAction(Action<T1, T2, T3, T4, T5> source, T1 first, T2 second) : base(source)
        {
            this.first = first;
            this.second = second;
        }

        public IFullyCurried<Action> With(T3 third, T4 fourth, T5 fifth)
            => new FullyCurriedAction<T1, T2, T3, T4, T5>(Source, first, second, third, fourth, fifth);

        public ICurriedAction<T5> With(T3 third, T4 fourth)
            => new FourArgumentsCurriedActionAction<T1, T2, T3, T4, T5>(Source, first, second, third, fourth);

        public ICurriedAction<T4, T5> With(T3 argument)
            => new ThreeArgumentsCurriedAction<T1, T2, T3, T4, T5>(Source, first, second, argument);

        public Action<T3, T4, T5> Delegate => (x, y, z) => Source(first, second, x, y, z);
    }

    internal class ThreeArgumentsCurriedAction<T1, T2, T3, T4, T5> : Curried<Action<T1, T2, T3, T4, T5>>,
        ICurriedAction<T4, T5>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;

        public ThreeArgumentsCurriedAction(Action<T1, T2, T3, T4, T5> source, T1 first, T2 second, T3 third) :
            base(source)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        public IFullyCurried<Action> With(T4 fourth, T5 fifth)
            => new FullyCurriedAction<T1, T2, T3, T4, T5>(Source, first, second, third, fourth, fifth);

        public ICurriedAction<T5> With(T4 argument)
            => new FourArgumentsCurriedActionAction<T1, T2, T3, T4, T5>(Source, first, second, third, argument);

        public Action<T4, T5> Delegate => (x, y) => Source(first, second, third, x, y);
    }

    internal class FourArgumentsCurriedActionAction<T1, T2, T3, T4, T5> : Curried<Action<T1, T2, T3, T4, T5>>,
        ICurriedAction<T5>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;
        private readonly T4 fourth;

        public FourArgumentsCurriedActionAction(Action<T1, T2, T3, T4, T5> source, T1 first, T2 second, T3 third,
            T4 fourth) : base(source)
        {
            this.first = first;
            this.second = second;
            this.third = third;
            this.fourth = fourth;
        }

        public IFullyCurried<Action> With(T5 argument)
            => new FullyCurriedAction<T1, T2, T3, T4, T5>(Source, first, second, third, fourth, argument);

        public Action<T5> Delegate => x => Source(first, second, third, fourth, x);
    }

    internal class FullyCurriedAction<T1, T2, T3, T4, T5> : Curried<Action<T1, T2, T3, T4, T5>>,
        IFullyCurried<Action>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;
        private readonly T4 fourth;
        private readonly T5 fifth;

        public FullyCurriedAction(Action<T1, T2, T3, T4, T5> source, T1 first, T2 second, T3 third, T4 fourth,
            T5 fifth) : base(source)
        {
            this.first = first;
            this.second = second;
            this.third = third;
            this.fourth = fourth;
            this.fifth = fifth;
        }

        public Action Delegate => () => Source(first, second, third, fourth, fifth);
    }
}