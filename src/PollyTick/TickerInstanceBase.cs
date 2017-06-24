using System.Collections.Generic;
using System.Diagnostics;
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
            var failures = result.Outcome == OutcomeType.Successful ?  0 : 1;
            var stats =  new Statistics<TResult>(1, failures, sw.ElapsedMilliseconds, result.Result);

            if (result.FinalException != null)
            {
                observer.OnException(result.FinalException);
                foreach (var obs in _observers)
                {
                    obs.OnException(result.FinalException);
                } 
            }

            observer.OnExecute(stats);
            foreach (var obs in _observers)
            {
                obs.OnExecute(stats);
            }

            return stats;
        }

        private List<IStatisticsObserver> _observers;
    }
}
