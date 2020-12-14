using System;
using mrlldd.Functional.Currying.Abstractions;

namespace mrlldd.Functional.Currying.Internal.Curried.Funcs
{
    internal class CurriedFunc<T1, T2, T3, T4> : Curried<Func<T1, T2, T3, T4>>, ICurriedFunc<T1, T2, T3, T4>
    {
        public CurriedFunc(Func<T1, T2, T3, T4> source) : base(source)
        {
        }

        public IFullyCurried<Func<T4>> With(T1 first, T2 second, T3 third)
            => new FullyCurriedFunc<T1, T2, T3, T4>(Source, first, second, third);

        public ICurriedFunc<T3, T4> With(T1 first, T2 second)
            => new TwoArgumentsCurriedFunc<T1, T2, T3, T4>(Source, first, second);

        public ICurriedFunc<T2, T3, T4> With(T1 argument)
            => new OneArgumentCurriedFunc<T1, T2, T3, T4>(Source, argument);

        public Func<T1, T2, T3, T4> Delegate => Source;
    }
    
    internal class OneArgumentCurriedFunc<T1, T2, T3, T4> : Curried<Func<T1, T2, T3, T4>>, ICurriedFunc<T2, T3, T4>
    {
        private readonly T1 first;

        public OneArgumentCurriedFunc(Func<T1, T2, T3, T4> source, T1 first) : base(source)
        {
            this.first = first;
        }

        public IFullyCurried<Func<T4>> With(T2 second, T3 third)
            => new FullyCurriedFunc<T1, T2, T3, T4>(Source, first, second, third);

        public ICurriedFunc<T3, T4> With(T2 argument)
            => new TwoArgumentsCurriedFunc<T1, T2, T3, T4>(Source, first, argument);

        public Func<T2, T3, T4> Delegate => (x, y) => Source(first, x, y);
    }
    
    internal class TwoArgumentsCurriedFunc<T1, T2, T3, T4> : Curried<Func<T1, T2, T3, T4>>, ICurriedFunc<T3, T4>
    {
        private readonly T1 first;
        private readonly T2 second;

        public TwoArgumentsCurriedFunc(Func<T1, T2, T3, T4> source,
            T1 first,
            T2 second) : base(source)
        {
            this.first = first;
            this.second = second;
        }

        public IFullyCurried<Func<T4>> With(T3 argument)
            => new FullyCurriedFunc<T1, T2, T3, T4>(Source, first, second, argument);

        public Func<T3, T4> Delegate => x => Source(first, second, x);
    }
    
    internal class FullyCurriedFunc<T1, T2, T3, T4> : Curried<Func<T1, T2, T3, T4>>, IFullyCurried<Func<T4>>
    {
        private readonly T1 first;
        private readonly T2 second;
        private readonly T3 third;

        public FullyCurriedFunc(Func<T1, T2, T3, T4> source,
            T1 first,
            T2 second,
            T3 third) : base(source)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        public Func<T4> Delegate => () => Source(first, second, third);
    }
}