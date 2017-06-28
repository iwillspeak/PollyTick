using System;

namespace PollyTick
{
    /// <summary>
    ///   Statistics calculated from a Ticker execution. Each time a
    ///   TickerInstance is executed a Statistics instance is returned
    ///   to keep track of what happened.
    /// </summary>
    public class Statistics
    {
        public Statistics(int executions, int exceptions, TimeSpan elapsed)
        {
            Executions = executions;
            Exceptions = exceptions;
            Elapsed = elapsed;
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
        ///   The total elapsed time for all executions.
        /// </summary>
        public TimeSpan Elapsed { get; }
    }

    /// <summary>
    ///   Statistics about a policy execution with a return value.
    /// </summary>
    public class Statistics<T> : Statistics
    {
        public Statistics(int executions, int exceptions, TimeSpan elapsed, T result)
            : base(executions, exceptions, elapsed)
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
