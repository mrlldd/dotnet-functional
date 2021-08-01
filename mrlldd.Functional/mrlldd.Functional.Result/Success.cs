namespace Functional.Result
{
    /// <summary>
    /// The class that represents a valueless success.
    /// </summary>
    public sealed class Success : Result
    {
        /// <inheritdoc />
        public override bool Successful => true;

        internal Success()
        {
        }
    }

    /// <summary>
    /// The class that represents a generic success and has a wrapped value of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of wrapped value.</typeparam>
    public sealed class Success<T> : Result<T>
    {
        /// <inheritdoc />
        public override bool Successful => true;

        /// <value>
        /// The wrapped value.
        /// </value>
        public T Value { get; }

        internal Success(T value)
            => Value = value;

        /// <inheritdoc />
        public override string ToString()
            => $"{base.ToString()}value: {Value}";

        /// <summary>
        /// The operator that wraps <typeparamref name="T"/> to <see cref="Success{T}"/>.
        /// </summary>
        /// <param name="value">The value that will be wrapped.</param>
        /// <returns>The <see cref="Success{T}"/>.</returns>
        public static implicit operator Success<T>(T value)
            => new(value);

        /// <summary>
        /// The operator that unwraps <see cref="Success{T}"/> to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="success">The <see cref="Success{T}"/> that will be unwrapped.</param>
        /// <returns>The <typeparamref name="T"/>.</returns>
        public static implicit operator T(Success<T> success)
            => success.Value;
    }
}