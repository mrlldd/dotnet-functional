namespace mrlldd.Functional.Result
{
    public sealed class Success<T> : Result<T>
    {
        public override bool Successful => true;
        
        public T Value { get; }

        internal Success(T value)
        {
            Value = value;
        }

        public override string ToString() 
            => base.ToString() + "value: " + Value;

        public static implicit operator Success<T>(T value)
            => new(value);
        
        public static implicit operator T(Success<T> success)
            => success.Value;
    }
}