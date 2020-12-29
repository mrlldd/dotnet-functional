namespace mrlldd.Functional.Result
{
    /// <summary>
    /// The interface that represents the entity that has a success state.
    /// </summary>
    public interface IHasSuccessState
    {
        /// <value>
        /// Gets the success state.
        /// </value>
        bool Successful { get; }
    }
}