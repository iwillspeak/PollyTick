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
        /// <summary>
        ///  Create a new statistics instance
        /// </summary>
        /// <param name="executions">The number of excecutions this statistics object covers</param>
        /// <param name="exceptions">The number of exceptions observed</param>
        /// <param name="elapsed">The total elapsed time observed</param>
        /// <param name="finalException">The last exception observed, if any</param>
        public Statistics(int executions, int exceptions, TimeSpan elapsed, Exception? finalException)
        {
            Executions = executions;
            Exceptions = exceptions;
            Elapsed = elapsed;
			FinalException = finalException;
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

		/// <summary>
		///   The final exception seen by this statistics, if any.
		/// </summary>
		public Exception? FinalException { get; }
    }

    /// <summary>
    ///   Statistics about a policy execution with a return value.
    /// </summary>
    public class Statistics<T> : Statistics
    {
        /// <summary>
        ///  Create a new statistics instance, with a result value.
        /// </summary>
        /// <param name="executions">The number of excecutions this statistics object covers</param>
        /// <param name="exceptions">The number of exceptions observed</param>
        /// <param name="elapsed">The total elapsed time observed</param>
        /// <param name="finalException">The last exception observed, if any</param>
        /// <param name="result">The result of the execution</param>
        public Statistics(int executions, int exceptions, TimeSpan elapsed, Exception? finalException, T? result)
            : base(executions, exceptions, elapsed, finalException)
        {
            Result = result;
        }

        /// <summary>
        ///   The result of the operation, if it completed
        ///   successfully.
        /// </summary>
        public T? Result { get; }
    }
}
