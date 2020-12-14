using System;
using mrlldd.Functional.Currying.Abstractions;

namespace mrlldd.Functional.Currying.Internal.Curried.Actions
{
    internal class CurriedAction<T1, T2, T3, T4, T5, T6> : Curried<Action<T1, T2, T3, T4, T5, T6>>,
        ICurriedAction<T1, T2, T3, T4, T5, T6>
    {
        public CurriedAction(Action<T1, T2, T3, T4, T5, T6> source) : base(source)
        {
        }

        public IFullyCurried<Action> With(T1 first,
            T2 second,
            T3 third,
            T4 fourth,
            T5 fifth,
            T6 sixth)
            => new FullyCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth, fifth, sixth);

        public ICurriedAction<T6> With(T1 first,
            T2 second,
            T3 third,
            T4 fourth,
            T5 fifth)
            => new FiveArgumentsCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth, fifth);

        public ICurriedAction<T5, T6> With(T1 first,
            T2 second,
            T3 third,
            T4 fourth)
            => new FourArgumentsCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth);

        public ICurriedAction<T4, T5, T6> With(T1 first,
            T2 second,
            T3 third)
            => new ThreeArgumentsCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third);

        public ICurriedAction<T3, T4, T5, T6> With(T1 first,
            T2 second)
            => new TwoArgumentsCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second);

        public ICurriedAction<T2, T3, T4, T5, T6> With(T1 argument)
            => new OneArgumentCurriedAction<T1, T2, T3, T4, T5, T6>(Source, argument);

        public Action<T1, T2, T3, T4, T5, T6> Delegate => Source;
    }

    internal class OneArgumentCurriedAction<T1, T2, T3, T4, T5, T6> : Curried<Action<T1, T2, T3, T4, T5, T6>>,
        ICurriedAction<T2, T3, T4, T5, T6>
    {
        private readonly T1 first;

        public OneArgumentCurriedAction(Action<T1, T2, T3, T4, T5, T6> source, T1 first) : base(source)
        {
            this.first = first;
        }

        public IFullyCurried<Action> With(T2 second,
            T3 third,
            T4 fourth,
            T5 fifth,
            T6 sixth)
            => new FullyCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth, fifth, sixth);

        public ICurriedAction<T6> With(T2 second, T3 third, T4 fourth, T5 fifth)
            => new FiveArgumentsCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth, fifth);

        public ICurriedAction<T5, T6> With(T2 second, T3 third, T4 fourth)
            => new FourArgumentsCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth);

        public ICurriedAction<T4, T5, T6> With(T2 second, T3 third)
            => new ThreeArgumentsCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third);

        public ICurriedAction<T3, T4, T5, T6> With(T2 argument)
            => new TwoArgumentsCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, argument);

        public Action<T2, T3, T4, T5, T6> Delegate => (x, y, z, u, m) => Source(first, x, y, z, u, m);
    }

    internal class TwoArgumentsCurriedAction<T1, T2, T3, T4, T5, T6> : Curried<Action<T1, T2, T3, T4, T5, T6>>,
        ICurriedAction<T3, T4, T5, T6>
    {
        private readonly T1 first;
        private readonly T2 second;

        public TwoArgumentsCurriedAction(Action<T1, T2, T3, T4, T5, T6> source,
            T1 first,
            T2 second) : base(source)
        {
            this.first = first;
            this.second = second;
        }

        public IFullyCurried<Action> With(T3 third, T4 fourth, T5 fifth, T6 sixth)
            => new FullyCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth, fifth, sixth);

        public ICurriedAction<T6> With(T3 third, T4 fourth, T5 fifth)
            => new FiveArgumentsCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth, fifth);

        public ICurriedAction<T5, T6> With(T3 third, T4 fourth)
            => new FourArgumentsCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth);

        public ICurriedAction<T4, T5, T6> With(T3 argument)
            => new ThreeArgumentsCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, argument);

        public Action<T3, T4, T5, T6> Delegate => (x, y, z, u) => Source(first, second, x, y, z, u);
    }

    internal class ThreeArgumentsCurriedAction<T1, T2, T3, T4, T5, T6> : Curried<Action<T1, T2, T3, T4, T5, T6>>,
        ICurriedAction<T4, T5, T6>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;

        public ThreeArgumentsCurriedAction(Action<T1, T2, T3, T4, T5, T6> source,
            T1 first,
            T2 second,
            T3 third) : base(source)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        public IFullyCurried<Action> With(T4 fourth, T5 fifth, T6 sixth)
            => new FullyCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth, fifth, sixth);

        public ICurriedAction<T6> With(T4 fourth, T5 fifth)
            => new FiveArgumentsCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth, fifth);

        public ICurriedAction<T5, T6> With(T4 argument)
            => new FourArgumentsCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third, argument);

        public Action<T4, T5, T6> Delegate => (x, y, z) => Source(first, second, third, x, y, z);
    }

    internal class FourArgumentsCurriedAction<T1, T2, T3, T4, T5, T6> : Curried<Action<T1, T2, T3, T4, T5, T6>>,
        ICurriedAction<T5, T6>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;
        private readonly T4 fourth;

        public FourArgumentsCurriedAction(Action<T1, T2, T3, T4, T5, T6> source,
            T1 first,
            T2 second,
            T3 third,
            T4 fourth) : base(source)
        {
            this.first = first;
            this.second = second;
            this.third = third;
            this.fourth = fourth;
        }

        public IFullyCurried<Action> With(T5 fifth, T6 sixth)
            => new FullyCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth, fifth, sixth);

        public ICurriedAction<T6> With(T5 argument)
            => new FiveArgumentsCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth, argument);

        public Action<T5, T6> Delegate => (x, y) => Source(first, second, third, fourth, x, y);
    }

    internal class FiveArgumentsCurriedAction<T1, T2, T3, T4, T5, T6> : Curried<Action<T1, T2, T3, T4, T5, T6>>,
        ICurriedAction<T6>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;
        private readonly T4 fourth;
        private readonly T5 fifth;

        public FiveArgumentsCurriedAction(Action<T1, T2, T3, T4, T5, T6> source,
            T1 first,
            T2 second,
            T3 third,
            T4 fourth,
            T5 fifth) : base(source)
        {
            this.first = first;
            this.second = second;
            this.third = third;
            this.fourth = fourth;
            this.fifth = fifth;
        }

        public IFullyCurried<Action> With(T6 argument)
            => new FullyCurriedAction<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth, fifth, argument);

        public Action<T6> Delegate => x => Source(first, second, third, fourth, fifth, x);
    }

    internal class FullyCurriedAction<T1, T2, T3, T4, T5, T6> : Curried<Action<T1, T2, T3, T4, T5, T6>>,
        IFullyCurried<Action>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;
        private readonly T4 fourth;
        private readonly T5 fifth;
        private readonly T6 sixth;

        public FullyCurriedAction(Action<T1, T2, T3, T4, T5, T6> source,
            T1 first,
            T2 second,
            T3 third,
            T4 fourth,
            T5 fifth,
            T6 sixth) : base(source)
        {
            this.first = first;
            this.second = second;
            this.third = third;
            this.fourth = fourth;
            this.fifth = fifth;
            this.sixth = sixth;
        }

        public Action Delegate => () => Source(first, second, third, fourth, fifth, sixth);
    }
}