using System;

namespace Functional.Result.Exceptions
{
    /// <summary>
    /// The exception that represents an error during interaction with <see cref="Result"/> instances.
    /// </summary>
    public class FunctionalResultException : Exception
    {
        /// <inheritdoc />
        public FunctionalResultException(string message) : base(message)
        {
        }
    }
}