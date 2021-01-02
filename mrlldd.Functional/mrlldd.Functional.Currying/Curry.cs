using System;
using Functional.Currying.Abstractions;
using Functional.Currying.Internal.Curried.Actions;
using Functional.Currying.Internal.Curried.Funcs;

namespace Functional.Currying
{
    public static class Curry
    {
        public static ICurriedAction<T> Action<T>(Action<T> action)
            => new CurriedAction<T>(action);

        public static ICurriedAction<T1, T2> Action<T1, T2>(Action<T1, T2> action)
            => new CurriedAction<T1, T2>(action);

        public static ICurriedAction<T1, T2, T3> Action<T1, T2, T3>(Action<T1, T2, T3> action)
            => new CurriedAction<T1, T2, T3>(action);

        public static ICurriedAction<T1, T2, T3, T4> Action<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action)
            => new CurriedAction<T1, T2, T3, T4>(action);

        public static ICurriedAction<T1, T2, T3, T4, T5> Action<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action)
            => new CurriedAction<T1, T2, T3, T4, T5>(action);
        
        public static ICurriedAction<T1, T2, T3, T4, T5, T6> Action<T1, T2, T3, T4, T5, T6>(Action<T1,T2,T3,T4,T5,T6> action)
            => new CurriedAction<T1, T2, T3, T4, T5, T6>(action);

        public static ICurriedFunc<T1, T2> Func<T1, T2>(Func<T1, T2> func)
            => new CurriedFunc<T1, T2>(func);

        public static ICurriedFunc<T1, T2, T3> Func<T1, T2, T3>(Func<T1, T2, T3> func)
            => new CurriedFunc<T1, T2, T3>(func);

        public static ICurriedFunc<T1, T2, T3, T4> Func<T1, T2, T3, T4>(Func<T1, T2, T3, T4> func)
            => new CurriedFunc<T1, T2, T3, T4>(func);

        public static ICurriedFunc<T1, T2, T3, T4, T5> Func<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5> func)
            => new CurriedFunc<T1, T2, T3, T4, T5>(func);
        public static ICurriedFunc<T1, T2, T3, T4, T5, T6> Func<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6> func)
            => new CurriedFunc<T1, T2, T3, T4, T5, T6>(func);
    }
}