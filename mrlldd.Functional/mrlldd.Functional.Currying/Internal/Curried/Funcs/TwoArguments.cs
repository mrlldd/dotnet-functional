using System;
using Functional.Currying.Abstractions;

namespace Functional.Currying.Internal.Curried.Funcs
{
    internal class CurriedFunc<T1, T2, T3> : Curried<Func<T1, T2, T3>>, ICurriedFunc<T1, T2, T3>
    {
        public CurriedFunc(Func<T1, T2, T3> source) : base(source)
        {
        }

        public IFullyCurried<Func<T3>> With(T1 first, T2 second)
            => new FullyCurriedFunc<T1, T2, T3>(Source, first, second);

        public ICurriedFunc<T2, T3> With(T1 argument)
            => new OneArgumentCurriedFunc<T1, T2, T3>(Source, argument);

        public Func<T1, T2, T3> Delegate => Source;
    }

    internal class OneArgumentCurriedFunc<T1, T2, T3> : Curried<Func<T1, T2, T3>>, ICurriedFunc<T2, T3>
    {
        private readonly T1 first;

        public OneArgumentCurriedFunc(Func<T1, T2, T3> source, T1 first) : base(source)
        {
            this.first = first;
        }

        public IFullyCurried<Func<T3>> With(T2 argument)
            => new FullyCurriedFunc<T1, T2, T3>(Source, first, argument);

        public Func<T2, T3> Delegate => x => Source(first, x);
    }

    internal class FullyCurriedFunc<T1, T2, T3> : Curried<Func<T1, T2, T3>>, IFullyCurried<Func<T3>>
    {
        private readonly T1 first;
        private readonly T2 second;

        public FullyCurriedFunc(Func<T1, T2, T3> source,
            T1 first,
            T2 second) : base(source)
        {
            this.first = first;
            this.second = second;
        }

        public Func<T3> Delegate => () => Source(first, second);
    }
}