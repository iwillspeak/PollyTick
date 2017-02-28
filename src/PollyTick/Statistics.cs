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
}
