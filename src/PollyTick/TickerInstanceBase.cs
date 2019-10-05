using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace PollyTick
{
    /// <summary>
    ///  The base class for all ticker instances
    /// </summary>
    public abstract class TickerInstanceBase
    {
        private List<IStatisticsObserver> _observers;
        
        /// <summary>
        ///  Create an instance of a ticker instance
        /// </summary>
        protected TickerInstanceBase()
        {
            _observers = new List<IStatisticsObserver>(3);
        }

        /// <summary>
        ///   Add a global observer to this ticker instance.
        /// </summary>
        protected void AddObserver(IStatisticsObserver observer)
        {
            _observers.Add(observer);
        }
        
        /// <summary>
        ///   Convert a Polly `ExecutionResult` into an instance of
        ///   our `Statistics` class.
        /// </summary>
        protected Statistics<TResult> StatisticsFromResult<TResult>(
            PolicyResult<TResult> result,
            Stopwatch sw,
            IStatisticsObserver observer)
        {
            var failures = result.Outcome == OutcomeType.Successful ? 0 : 1;
            var stats = new Statistics<TResult>(1, failures, sw.Elapsed, result.FinalException, result.Result);

            if (result.FinalException != null)
            {
                OnException(result.FinalException, observer);
            }

            OnExecute(stats, observer);

            return stats;
        }

        /// <summary>
        /// Notify observers of the result of an excecution
        /// </summary>
        /// <param name="stats">The statistics for the execution</param>
        /// <param name="observer">An immediate observer to also notify</param>
        protected void OnExecute(Statistics stats, IStatisticsObserver observer)
        {
            observer.OnExecute(stats);
            foreach (var obs in _observers)
            {
                obs.OnExecute(stats);
            }
        }

        /// <summary>
        /// Notify observers of an exception
        /// </summary>
        /// <param name="exception">The exceptions observed</param>
        /// <param name="observer">An immediate observer to also notify</param>
        protected void OnException(Exception exception, IStatisticsObserver observer)
        {
            observer.OnException(exception);
            foreach (var obs in _observers)
            {
                obs.OnException(exception);
            }
        }
    }
}
