using System;

namespace PollyTick
{
    public interface IStatisticsObserver
    {
        /// <summary>
        ///   Observes the result of an execution. Called with the statistics
        ///   from the execution when it completes.
        /// </summary>
        void OnExecute(Statistics statistics);

        /// <summary>
        ///   Observes an Exception during execution
        /// </summary>
        void OnException(Exception exception);
    }
}
