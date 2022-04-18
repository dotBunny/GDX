namespace GDX.Collections.Generic
{
    /// <summary>
    ///     State of Iterator
    /// </summary>
    public enum IteratorState : byte
    {
        /// <summary>
        ///     Unable to iterate any further.
        /// </summary>
        Finished = 0,
        /// <summary>
        ///     Found an entry meeting criteria.
        /// </summary>
        FoundEntry = 1,
        /// <summary>
        ///     The data being iterated over has changed.
        /// </summary>
        InvalidVersion = 2
    }
}
