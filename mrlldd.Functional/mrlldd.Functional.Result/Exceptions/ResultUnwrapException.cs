namespace mrlldd.Functional.Result.Exceptions
{
    /// <summary>
    /// The exception that represent an error during unwrapping of <see cref="Result"/> and <see cref="Result{T}"/> instances.
    /// </summary>
    public class ResultUnwrapException : FunctionalResultException
    {
        internal ResultUnwrapException(string message) : base(message)
        {
        }
    }
}