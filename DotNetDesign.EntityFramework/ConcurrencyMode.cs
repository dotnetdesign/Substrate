namespace DotNetDesign.EntityFramework
{
    /// <summary>
    /// Modes of concurrency
    /// </summary>
    public enum ConcurrencyMode
    {
        /// <summary>
        /// If changes are detected, the system should attempt to automatically merge with those changes and save if no overlapping changes are found.
        /// If overlapping changes are found an exception will be thrown.
        /// </summary>
        Merge = 0,

        /// <summary>
        /// Last in wins. If a save has been performed on the current object since it was last read from the database, 
        /// the final save will overwrite previous changes without taking those changes into account. 
        /// </summary>
        Overwrite = 1,

        /// <summary>
        /// If changes are detected since the last read, the save operation will be aborted and an exception will be thrown.
        /// </summary>
        Fail = 2
    }
}