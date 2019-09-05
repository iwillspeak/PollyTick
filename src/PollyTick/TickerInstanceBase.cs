using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace PollyTick
{
    public abstract class TickerInstanceBase
    {
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

        protected void OnExecute(Statistics stats, IStatisticsObserver observer)
        {
            observer.OnExecute(stats);
            foreach (var obs in _observers)
            {
                obs.OnExecute(stats);
            }
        }

        protected void OnException(Exception exception, IStatisticsObserver observer)
        {
            observer.OnException(exception);
            foreach (var obs in _observers)
            {
                obs.OnException(exception);
            }
        }

        private List<IStatisticsObserver> _observers;
    }
}
