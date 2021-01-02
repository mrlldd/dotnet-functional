using System;
using Functional.Currying.Abstractions;

namespace Functional.Currying.Internal.Curried.Funcs
{
    internal class CurriedFunc<T1, T2, T3, T4, T5> : Curried<Func<T1, T2, T3, T4, T5>>, ICurriedFunc<T1, T2, T3, T4, T5>
    {
        public CurriedFunc(Func<T1, T2, T3, T4, T5> source) : base(source)
        {
        }

        public IFullyCurried<Func<T5>> With(T1 first, T2 second, T3 third, T4 fourth)
            => new FullyCurriedFunc<T1, T2, T3, T4, T5>(Source, first, second, third, fourth);

        public ICurriedFunc<T4, T5> With(T1 first, T2 second, T3 third)
            => new ThreeArgumentsCurriedFunc<T1, T2, T3, T4, T5>(Source, first, second, third);

        public ICurriedFunc<T3, T4, T5> With(T1 first, T2 second)
            => new TwoArgumentsCurriedFunc<T1, T2, T3, T4, T5>(Source, first, second);

        public ICurriedFunc<T2, T3, T4, T5> With(T1 argument)
            => new OneArgumentCurriedFunc<T1, T2, T3, T4, T5>(Source, argument);

        public Func<T1, T2, T3, T4, T5> Delegate => Source;
    }

    internal class OneArgumentCurriedFunc<T1, T2, T3, T4, T5> : Curried<Func<T1, T2, T3, T4, T5>>,
        ICurriedFunc<T2, T3, T4, T5>
    {
        private readonly T1 first;

        public OneArgumentCurriedFunc(Func<T1, T2, T3, T4, T5> source,
            T1 first) : base(source)
        {
            this.first = first;
        }

        public IFullyCurried<Func<T5>> With(T2 second, T3 third, T4 fourth)
            => new FullyCurriedFunc<T1, T2, T3, T4, T5>(Source, first, second, third, fourth);

        public ICurriedFunc<T4, T5> With(T2 second, T3 third)
            => new ThreeArgumentsCurriedFunc<T1, T2, T3, T4, T5>(Source, first, second, third);

        public ICurriedFunc<T3, T4, T5> With(T2 argument)
            => new TwoArgumentsCurriedFunc<T1, T2, T3, T4, T5>(Source, first, argument);

        public Func<T2, T3, T4, T5> Delegate => (x, y, z) => Source(first, x, y, z);
    }

    internal class TwoArgumentsCurriedFunc<T1, T2, T3, T4, T5> : Curried<Func<T1, T2, T3, T4, T5>>,
        ICurriedFunc<T3, T4, T5>
    {
        private readonly T1 first;
        private readonly T2 second;

        public TwoArgumentsCurriedFunc(Func<T1, T2, T3, T4, T5> source,
            T1 first,
            T2 second) : base(source)
        {
            this.first = first;
            this.second = second;
        }

        public IFullyCurried<Func<T5>> With(T3 third, T4 fourth)
            => new FullyCurriedFunc<T1, T2, T3, T4, T5>(Source, first, second, third, fourth);

        public ICurriedFunc<T4, T5> With(T3 argument)
            => new ThreeArgumentsCurriedFunc<T1, T2, T3, T4, T5>(Source, first, second, argument);

        public Func<T3, T4, T5> Delegate => (x, y) => Source(first, second, x, y);
    }

    internal class ThreeArgumentsCurriedFunc<T1, T2, T3, T4, T5> : Curried<Func<T1, T2, T3, T4, T5>>,
        ICurriedFunc<T4, T5>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;

        public ThreeArgumentsCurriedFunc(Func<T1, T2, T3, T4, T5> source,
            T1 first,
            T2 second,
            T3 third) : base(source)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        public IFullyCurried<Func<T5>> With(T4 argument)
            => new FullyCurriedFunc<T1, T2, T3, T4, T5>(Source, first, second, third, argument);

        public Func<T4, T5> Delegate => x => Source(first, second, third, x);
    }

    internal class FullyCurriedFunc<T1, T2, T3, T4, T5> : Curried<Func<T1, T2, T3, T4, T5>>, IFullyCurried<Func<T5>>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;
        private readonly T4 fourth;

        public FullyCurriedFunc(Func<T1, T2, T3, T4, T5> source,
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

        public Func<T5> Delegate => () => Source(first, second, third, fourth);
    }
}