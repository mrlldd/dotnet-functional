using System;

namespace Functional.Currying.Abstractions
{
    public interface ICurriedFunc<in T1, out T2>
        : ICurrying<IFullyCurried<Func<T2>>, T1>,
            IContainsDelegate<Func<T1, T2>>
    {
    }

    public interface ICurriedFunc<in T1, in T2, out T3>
        : ICurrying<IFullyCurried<Func<T3>>, T1, T2>,
            ICurrying<ICurriedFunc<T2, T3>, T1>,
            IContainsDelegate<Func<T1, T2, T3>>
    {
    }

    public interface ICurriedFunc<in T1, in T2, in T3, out T4>
        : ICurrying<IFullyCurried<Func<T4>>, T1, T2, T3>,
            ICurrying<ICurriedFunc<T3, T4>, T1, T2>,
            ICurrying<ICurriedFunc<T2, T3, T4>, T1>,
            IContainsDelegate<Func<T1, T2, T3, T4>>
    {
    }

    public interface ICurriedFunc<in T1, in T2, in T3, in T4, out T5>
        : ICurrying<IFullyCurried<Func<T5>>, T1, T2, T3, T4>,
            ICurrying<ICurriedFunc<T4, T5>, T1, T2, T3>,
            ICurrying<ICurriedFunc<T3, T4, T5>, T1, T2>,
            ICurrying<ICurriedFunc<T2, T3, T4, T5>, T1>,
            IContainsDelegate<Func<T1, T2, T3, T4, T5>>
    {
    }
    
    public interface ICurriedFunc<in T1, in T2, in T3, in T4, in T5, out T6>
        : ICurrying<IFullyCurried<Func<T6>>, T1, T2, T3, T4, T5>,
            ICurrying<ICurriedFunc<T5, T6>, T1, T2, T3, T4>,
            ICurrying<ICurriedFunc<T4, T5, T6>, T1, T2, T3>,
            ICurrying<ICurriedFunc<T3, T4, T5, T6>, T1, T2>,
            ICurrying<ICurriedFunc<T2, T3, T4, T5, T6>, T1>,
            IContainsDelegate<Func<T1, T2, T3, T4, T5, T6>>
    {
    }
}