using System;

namespace Functional.Currying.Abstractions
{
    public interface ICurriedAction<in T>
        : ICurrying<IFullyCurried<Action>, T>,
            IContainsDelegate<Action<T>>
    {
    }

    public interface ICurriedAction<in T1, in T2>
        : ICurrying<IFullyCurried<Action>, T1, T2>,
            ICurrying<ICurriedAction<T2>, T1>,
            IContainsDelegate<Action<T1, T2>>
    {
    }

    public interface ICurriedAction<in T1, in T2, in T3>
        : ICurrying<IFullyCurried<Action>, T1, T2, T3>,
            ICurrying<ICurriedAction<T3>, T1, T2>,
            ICurrying<ICurriedAction<T2, T3>, T1>,
            IContainsDelegate<Action<T1, T2, T3>>
    {
    }

    public interface ICurriedAction<in T1, in T2, in T3, in T4>
        : ICurrying<IFullyCurried<Action>, T1, T2, T3, T4>,
            ICurrying<ICurriedAction<T4>, T1, T2, T3>,
            ICurrying<ICurriedAction<T3, T4>, T1, T2>,
            ICurrying<ICurriedAction<T2, T3, T4>, T1>,
            IContainsDelegate<Action<T1, T2, T3, T4>>
    {
    }

    public interface ICurriedAction<in T1, in T2, in T3, in T4, in T5>
        : ICurrying<IFullyCurried<Action>, T1, T2, T3, T4, T5>,
            ICurrying<ICurriedAction<T5>, T1, T2, T3, T4>,
            ICurrying<ICurriedAction<T4, T5>, T1, T2, T3>,
            ICurrying<ICurriedAction<T3, T4, T5>, T1, T2>,
            ICurrying<ICurriedAction<T2, T3, T4, T5>, T1>,
            IContainsDelegate<Action<T1, T2, T3, T4, T5>>
    {
    }
    public interface ICurriedAction<in T1, in T2, in T3, in T4, in T5, in T6>
        : ICurrying<IFullyCurried<Action>, T1, T2, T3, T4, T5, T6>,
            ICurrying<ICurriedAction<T6>, T1, T2, T3, T4, T5>,
            ICurrying<ICurriedAction<T5, T6>, T1, T2, T3, T4>,
            ICurrying<ICurriedAction<T4, T5, T6>, T1, T2, T3>,
            ICurrying<ICurriedAction<T3, T4, T5, T6>, T1, T2>,
            ICurrying<ICurriedAction<T2, T3, T4, T5, T6>, T1>,
            IContainsDelegate<Action<T1, T2, T3, T4, T5, T6>>
    {
    }
}