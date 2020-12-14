namespace mrlldd.Functional.Currying.Abstractions
{
    public interface ICurrying<out TResult, in T> : ICurryResult where TResult : ICurryResult
    {
        TResult With(T argument);
    }

    public interface ICurrying<out TResult, in T1, in T2> : ICurryResult where TResult : ICurryResult
    {
        TResult With(T1 firstArgument, T2 secondArgument);
    }

    public interface ICurrying<out TResult, in T1, in T2, in T3> : ICurryResult where TResult : ICurryResult
    {
        TResult With(T1 firstArgument, T2 secondArgument, T3 thirdArgument);
    }

    public interface ICurrying<out TResult, in T1, in T2, in T3, in T4> : ICurryResult where TResult : ICurryResult
    {
        TResult With(T1 firstArgument, T2 secondArgument, T3 thirdArgument, T4 fourthArgument);
    }
    
    public interface ICurrying<out TResult, in T1, in T2, in T3, in T4, in T5> : ICurryResult where TResult : ICurryResult
    {
        TResult With(T1 firstArgument, T2 secondArgument, T3 thirdArgument, T4 fourthArgument, T5 fifthArgument);
    }
    
    public interface ICurrying<out TResult, in T1, in T2, in T3, in T4, in T5, in T6> : ICurryResult where TResult : ICurryResult
    {
        TResult With(T1 firstArgument, T2 secondArgument, T3 thirdArgument, T4 fourthArgument, T5 fifthArgument, T6 sixthArgument);
    }
}