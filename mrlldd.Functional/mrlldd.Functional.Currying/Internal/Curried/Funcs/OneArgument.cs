using System;
using Functional.Currying.Abstractions;

namespace Functional.Currying.Internal.Curried.Funcs
{
    internal class CurriedFunc<T1, T2> : Curried<Func<T1, T2>>, ICurriedFunc<T1, T2>
    {
        public CurriedFunc(Func<T1, T2> source) : base(source)
        {
        }

        public IFullyCurried<Func<T2>> With(T1 argument)
            => new FullyCurriedFunc<T1, T2>(Source, argument);

        public Func<T1, T2> Delegate => Source;
    }
    
    internal class FullyCurriedFunc<T1, T2> : Curried<Func<T1, T2>>, IFullyCurried<Func<T2>>
    {
        private readonly T1 argument;

        public FullyCurriedFunc(Func<T1, T2> source, T1 argument) : base(source)
        {
            this.argument = argument;
        }

        public Func<T2> Delegate => () => Source(argument);
    }
}