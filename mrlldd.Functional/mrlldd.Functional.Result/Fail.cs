using System;

namespace mrlldd.Functional.Result
{
    public sealed class Fail : Result
    {
        public override bool Successful => false;
        public Exception Exception { get; }

        internal Fail(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public override string ToString()
        {
            return $"{base.ToString()}, exception: {Exception}";
        }
        
        public static implicit operator Fail(Exception exception) => new(exception);
        
        public static implicit operator Exception(Fail fail) => fail.Exception;
    }
    
    public sealed class Fail<T> : Result<T>
    {
        public override bool Successful => false;
        public Exception Exception { get; }

        internal Fail(Exception exception)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }
        public override string ToString() 
            => $"{base.ToString()}exception: {Exception}";
        public static implicit operator Fail<T>(Exception exception) => new(exception);
        
        public static implicit operator Exception(Fail<T> fail) => fail.Exception;
    }
}