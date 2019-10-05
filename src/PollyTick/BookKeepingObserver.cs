using System;
using System.Collections.Generic;
using System.Threading;

namespace PollyTick
{
    /// <summary>
    ///   An IStatisticsObserver which collcts each event
    ///   internally for later observation.
    /// </summary>
    public class BookkeepingObserver : IStatisticsObserver
    {
        private int _executions;
        private int _exceptions;
        private long _totalTimespanTicks;

        /// <summary>
        ///   The number of Executions observed by this instance overall.
        /// </summary>
        public int Executions => _executions;

        /// <summary>
        ///   The number of Exceptions observed by this instance overall.
        /// </summary>
        public int Exceptions => _exceptions;

        /// <summary>
        ///   The total execution time observed by this instance
        ///   overall, as a <see cref="TimeSpan" />.
        /// </summary>
        public TimeSpan Elapsed => TimeSpan.FromTicks(Interlocked.Read(ref _totalTimespanTicks));

        /// <summary>
        ///   The last exception observed by this listener.
        /// </summary>
        public Exception LastException { get; private set; }

        /// <summary>
        ///  Called by a ticker ticker instance when an execution completes
        /// </summary>
        /// <param name="statistics">The statistics for the given execution</param>
        public void OnExecute(Statistics statistics)
        {
            Interlocked.Add(ref _executions, statistics.Executions);
            Interlocked.Add(ref _exceptions, statistics.Exceptions);
            Interlocked.Add(ref _totalTimespanTicks, statistics.Elapsed.Ticks);
        }

        /// <summary>
        ///  Called by a ticker instance when an exception is caught
        /// </summary>
        /// <param name="exception">The exception that was observed</param>
        public void OnException(Exception exception)
        {
            LastException = exception;
        }

        /// <summary>
        ///   Get the current state of this observer as a <see
        ///   cref="Statistics" /> instance.
        /// </summary>
        public Statistics IntoStatistics()
        {
            int executions = _executions;
            int exceptions = _exceptions;
            long ticks = Interlocked.Read(ref _totalTimespanTicks);
            return new Statistics(executions, exceptions, TimeSpan.FromTicks(ticks), LastException);
        }
    }
}
