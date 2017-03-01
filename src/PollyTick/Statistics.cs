namespace PollyTick
{
    /// <summary>
    ///   Statistics calculated from a Ticker execution. Each time a
    ///   TickerInstance is executed a Statistics instance is returned
    ///   to keep track of what happened.
    /// </summary>
    public class Statistics
    {
        public Statistics(int executions, int exceptions, long totalMillis)
        {
            Executions = executions;
            Exceptions = exceptions;
            TotalMilliseconds = totalMillis;
        }

        /// <summary>
        ///   Returns the number of executions that this statistics
        ///   instance is for.
        /// </summary>
        public int Executions { get; }

        /// <summary>
        ///   Returns the number of exceptions encountered during the
        ///   excecutions this instance is for.
        /// </summary>
        public int Exceptions { get; }

        /// <summary>
        ///   The total number of milliseconds taken for all
        ///   executions.
        /// </summary>
        public long TotalMilliseconds { get; }
    }

    /// <summary>
    ///   Statistics about a policy execution with a return value.
    /// </summary>
    public class Statistics<T> : Statistics
    {
        public Statistics(int executions, int exceptions, long totalMillis, T result)
            : base(executions, exceptions, totalMillis)
        {
            Result = result;
        }

        /// <summary>
        ///   The result of the operation, if it completed
        ///   successfully.
        /// </summary>
        public T Result { get; }
    }
}
