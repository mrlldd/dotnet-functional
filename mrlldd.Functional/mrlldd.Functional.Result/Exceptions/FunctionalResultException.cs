using System;

namespace mrlldd.Functional.Result.Exceptions
{
    public class FunctionalResultException : Exception
    {
        public FunctionalResultException(string message) : base(message)
        {
        }
    }
}