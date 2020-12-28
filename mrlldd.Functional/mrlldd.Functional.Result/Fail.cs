using System;

namespace mrlldd.Functional.Result
{
    /// <summary>
    /// The class that represents valueless fail.
    /// </summary>
    public sealed class Fail : Result
    {
        /// <inheritdoc />
        public override bool Successful => false;
        /// <value>
        /// The wrapped exception.
        /// </value>
        public Exception Exception { get; }

        internal Fail(Exception exception) 
            => Exception = exception ?? throw new ArgumentNullException(nameof(exception));

        /// <inheritdoc />
        public override string ToString() 
            => $"{base.ToString()}, exception: {Exception}";

        /// <summary>
        /// The operator that implicitly wraps <see cref="System.Exception"/> to <see cref="Fail"/>.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">Thrown when exception is null.</exception>
        /// <returns>The <see cref="Fail"/>.</returns>
        public static implicit operator Fail(Exception exception) => new(exception);
        
        public static implicit operator Exception(Fail fail) => fail.Exception;
    }
    
    public sealed class Fail<T> : Result<T>
    {
        /// <inheritdoc />
        public override bool Successful => false;
        public Exception Exception { get; }

        internal Fail(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        /// <inheritdoc />
        public override string ToString() 
            => $"{base.ToString()}exception: {Exception}";
        public static implicit operator Fail<T>(Exception exception) => new(exception);
        
        public static implicit operator Exception(Fail<T> fail) => fail.Exception;
    }
}