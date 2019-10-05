using System;

namespace PollyTick
{
    /// <statistics>
    ///   An <see cref="IStatisticsObserver" /> insttance which does
    ///   nothing.
    /// </statistics>
    public class NullObserver : IStatisticsObserver
    {
        /// <summary>
        /// Does nothing when the statistics from an execution
        /// </summary>
        /// <param name="statistics">The statistics to ignore</param>
        public void OnExecute(Statistics statistics)
        {
        }

        /// <summary>
        ///  Does nothing with an exception observed during execution.
        /// </summary>
        /// <param name="exception">The exception to ignore</param>
        public void OnException(Exception exception)
        {
        }
    }
}
