using System;
using Functional.Currying.Abstractions;

namespace Functional.Currying.Internal.Curried.Actions
{
    internal class CurriedAction<T1, T2, T3, T4> : Curried<Action<T1, T2, T3, T4>>,
        ICurriedAction<T1, T2, T3, T4>
    {
        public CurriedAction(Action<T1, T2, T3, T4> source) : base(source)
        {
        }

        public IFullyCurried<Action> With(T1 first, T2 second, T3 third, T4 fourth)
            => new FullyCurriedAction<T1, T2, T3, T4>(Source, first, second, third, fourth);

        public ICurriedAction<T4> With(T1 first, T2 second, T3 third)
            => new ThreeArgumentsCurriedActionAction<T1, T2, T3, T4>(Source, first, second, third);

        public ICurriedAction<T3, T4> With(T1 first, T2 second)
            => new TwoArgumentsCurriedAction<T1, T2, T3, T4>(Source, first, second);

        public ICurriedAction<T2, T3, T4> With(T1 argument)
            => new OneArgumentCurriedAction<T1, T2, T3, T4>(Source, argument);

        public Action<T1, T2, T3, T4> Delegate => Source;
    }

    internal class OneArgumentCurriedAction<T1, T2, T3, T4> : Curried<Action<T1, T2, T3, T4>>,
        ICurriedAction<T2, T3, T4>
    {
        private readonly T1 first;

        public OneArgumentCurriedAction(Action<T1, T2, T3, T4> source, T1 first) : base(source)
        {
            this.first = first;
        }

        public IFullyCurried<Action> With(T2 second, T3 third, T4 fourth)
            => new FullyCurriedAction<T1, T2, T3, T4>(Source, first, second, third, fourth);

        public ICurriedAction<T4> With(T2 second, T3 third)
            => new ThreeArgumentsCurriedActionAction<T1, T2, T3, T4>(Source, first, second, third);

        public ICurriedAction<T3, T4> With(T2 argument)
            => new TwoArgumentsCurriedAction<T1, T2, T3, T4>(Source, first, argument);

        public Action<T2, T3, T4> Delegate => (x, y, z) => Source(first, x, y, z);
    }

    internal class TwoArgumentsCurriedAction<T1, T2, T3, T4> : Curried<Action<T1, T2, T3, T4>>,
        ICurriedAction<T3, T4>
    {
        private readonly T1 first;
        private readonly T2 second;

        public TwoArgumentsCurriedAction(Action<T1, T2, T3, T4> source, T1 first, T2 second) : base(source)
        {
            this.first = first;
            this.second = second;
        }

        public IFullyCurried<Action> With(T3 third, T4 fourth)
            => new FullyCurriedAction<T1, T2, T3, T4>(Source, first, second, third, fourth);

        public ICurriedAction<T4> With(T3 argument)
            => new ThreeArgumentsCurriedActionAction<T1, T2, T3, T4>(Source, first, second, argument);

        public Action<T3, T4> Delegate => (x, y) => Source(first, second, x, y);
    }

    internal class ThreeArgumentsCurriedActionAction<T1, T2, T3, T4> : Curried<Action<T1, T2, T3, T4>>,
        ICurriedAction<T4>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;

        public ThreeArgumentsCurriedActionAction(Action<T1, T2, T3, T4> source, T1 first, T2 second, T3 third) : base(source)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        public IFullyCurried<Action> With(T4 argument)
            => new FullyCurriedAction<T1, T2, T3, T4>(Source, first, second, third, argument);

        public Action<T4> Delegate => x => Source(first, second, third, x);
    }

    internal class FullyCurriedAction<T1, T2, T3, T4> : Curried<Action<T1, T2, T3, T4>>, IFullyCurried<Action>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;
        private readonly T4 fourth;

        public FullyCurriedAction(Action<T1, T2, T3, T4> source, T1 first, T2 second, T3 third, T4 fourth) :
            base(source)
        {
            this.first = first;
            this.second = second;
            this.third = third;
            this.fourth = fourth;
        }

        public Action Delegate => () => Source(first, second, third, fourth);
    }
}