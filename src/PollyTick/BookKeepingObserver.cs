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
        public void OnExecute(Statistics statistics)
        {
            Interlocked.Add(ref _executions, statistics.Executions);
            Interlocked.Add(ref _exceptions, statistics.Exceptions);
            Interlocked.Add(ref _totalMilliseconds, statistics.TotalMilliseconds);
        }

        public void OnException(Exception exception)
        {
            LastException = exception;
        }

        /// <summary>
        ///   The number of Executions observed by this instance overall.
        /// </summary>
        public int Executions => _executions;
        private int _executions;

        /// <summary>
        ///   The number of Exceptions observed by this instance overall.
        /// </summary>
        public int Exceptions => _exceptions;
        private int _exceptions;

        /// <summary>
        ///   The total number of Milliseconds taken by all observed executions.
        /// </summary>
        public long TotalMilliseconds => _totalMilliseconds;
        private long _totalMilliseconds;

        /// <summary>
        ///   The last exception observed by this listener.
        /// </summary>
        public Exception LastException;
    }
}
