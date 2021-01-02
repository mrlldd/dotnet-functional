using System;
using Functional.Currying.Abstractions;

namespace Functional.Currying.Internal.Curried.Funcs
{
    internal class CurriedFunc<T1, T2, T3, T4, T5, T6> : Curried<Func<T1, T2, T3, T4, T5, T6>>,
        ICurriedFunc<T1, T2, T3, T4, T5, T6>
    {
        public CurriedFunc(Func<T1, T2, T3, T4, T5, T6> source) : base(source)
        {
        }

        public IFullyCurried<Func<T6>> With(T1 first, T2 second, T3 third, T4 fourth, T5 fifth)
            => new FullyCurriedFunc<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth, fifth);

        public ICurriedFunc<T5, T6> With(T1 first, T2 second, T3 third, T4 fourth)
            => new FourArgumentsCurriedFunc<T1,T2,T3,T4,T5,T6>(Source, first, second, third, fourth);

        public ICurriedFunc<T4, T5, T6> With(T1 first, T2 second, T3 third)
            => new ThreeArgumentsCurriedFunc<T1, T2, T3, T4, T5, T6>(Source, first, second, third);

        public ICurriedFunc<T3, T4, T5, T6> With(T1 first, T2 second)
            => new TwoArgumentsCurriedFunc<T1, T2, T3, T4, T5, T6>(Source, first, second);

        public ICurriedFunc<T2, T3, T4, T5, T6> With(T1 argument)
            => new OneArgumentCurriedFunc<T1, T2, T3, T4, T5, T6>(Source, argument);

        public Func<T1, T2, T3, T4, T5, T6> Delegate => Source;
    }

    internal class OneArgumentCurriedFunc<T1, T2, T3, T4, T5, T6> : Curried<Func<T1, T2, T3, T4, T5, T6>>,
        ICurriedFunc<T2, T3, T4, T5, T6>
    {
        private readonly T1 first;

        public OneArgumentCurriedFunc(Func<T1, T2, T3, T4, T5, T6> source,
            T1 first) : base(source)
        {
            this.first = first;
        }

        public IFullyCurried<Func<T6>> With(T2 second, T3 third, T4 fourth, T5 fifth)
            => new FullyCurriedFunc<T1,T2,T3,T4,T5,T6>(Source, first, second, third, fourth, fifth);

        public ICurriedFunc<T5, T6> With(T2 second, T3 third, T4 fourth)
            => new FourArgumentsCurriedFunc<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth);

        public ICurriedFunc<T4, T5, T6> With(T2 second, T3 third)
            => new ThreeArgumentsCurriedFunc<T1, T2, T3, T4, T5, T6>(Source, first, second, third);

        public ICurriedFunc<T3, T4, T5, T6> With(T2 argument)
            => new TwoArgumentsCurriedFunc<T1, T2, T3, T4, T5, T6>(Source, first, argument);

        public Func<T2, T3, T4, T5, T6> Delegate => (x, y, z, u) => Source(first, x, y, z, u);
    }

    internal class TwoArgumentsCurriedFunc<T1, T2, T3, T4, T5, T6> : Curried<Func<T1, T2, T3, T4, T5, T6>>,
        ICurriedFunc<T3, T4, T5, T6>
    {
        private readonly T1 first;
        private readonly T2 second;

        public TwoArgumentsCurriedFunc(Func<T1, T2, T3, T4, T5, T6> source,
            T1 first,
            T2 second) : base(source)
        {
            this.first = first;
            this.second = second;
        }

        public IFullyCurried<Func<T6>> With(T3 third, T4 fourth, T5 fifth)
            => new FullyCurriedFunc<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth, fifth);

        public ICurriedFunc<T5, T6> With(T3 third, T4 fourth)
            => new FourArgumentsCurriedFunc<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth);

        public ICurriedFunc<T4, T5, T6> With(T3 argument)
            => new ThreeArgumentsCurriedFunc<T1, T2, T3, T4, T5, T6>(Source, first, second, argument);

        public Func<T3, T4, T5, T6> Delegate => (x, y, z) => Source(first, second, x, y, z);
    }

    internal class ThreeArgumentsCurriedFunc<T1, T2, T3, T4, T5, T6> : Curried<Func<T1, T2, T3, T4, T5, T6>>,
        ICurriedFunc<T4, T5, T6>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;

        public ThreeArgumentsCurriedFunc(Func<T1, T2, T3, T4, T5, T6> source,
            T1 first,
            T2 second,
            T3 third) : base(source)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        public IFullyCurried<Func<T6>> With(T4 fourth, T5 fifth)
            => new FullyCurriedFunc<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth, fifth);

        public ICurriedFunc<T5, T6> With(T4 argument)
            => new FourArgumentsCurriedFunc<T1, T2, T3, T4, T5, T6>(Source, first, second, third, argument);

        public Func<T4, T5, T6> Delegate => (x, y) => Source(first, second, third, x, y);
    }
    
    internal class FourArgumentsCurriedFunc<T1, T2, T3, T4, T5, T6> : Curried<Func<T1, T2, T3, T4, T5, T6>>,
        ICurriedFunc<T5, T6>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;
        private readonly T4 fourth;

        public FourArgumentsCurriedFunc(Func<T1, T2, T3, T4, T5, T6> source,
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

        public IFullyCurried<Func<T6>> With(T5 argument)
            => new FullyCurriedFunc<T1, T2, T3, T4, T5, T6>(Source, first, second, third, fourth, argument);

        public Func<T5, T6> Delegate => x => Source(first, second, third, fourth, x);
    }
    
    internal class FullyCurriedFunc<T1, T2, T3, T4, T5, T6> : Curried<Func<T1,T2,T3,T4,T5,T6>>, IFullyCurried<Func<T6>>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;
        private readonly T4 fourth;
        private readonly T5 fifth;

        public FullyCurriedFunc(Func<T1, T2, T3, T4, T5, T6> source,
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

        public Func<T6> Delegate => () => Source(first, second, third, fourth, fifth);
    }
}