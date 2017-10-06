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

        /// <summary>
        ///   Internal implementation of the `ExecuteNoCapture`.
        /// </summary>
        protected T ExecuteNoCaptureInternal<T>(Func<T> action, IStatisticsObserver observer)
        {
            var sw = Stopwatch.StartNew();
            int exceptions = 0;
            T result = default(T);
			Exception capturedException = null;
            try
            {
                result = action();
                return result;
            }
            catch (Exception e)
            {
                OnException(e, observer);
				capturedException = e;
                exceptions++;
                throw;
            }
            finally
            {
                sw.Stop();
                var stats = new Statistics<T>(1, exceptions, sw.Elapsed, capturedException, result);
                OnExecute(stats, observer);
            }
        }

        /// <summary>
        ///   Internal implementation of the `ExecuteNoCaptureAsync`.
        /// </summary>
        protected async Task<T> ExecuteNoCaptureInternalAsync<T>(
            Func<CancellationToken, Task<T>> action,
            IStatisticsObserver observer,
            CancellationToken token)
        {
            var sw = Stopwatch.StartNew();
            var exceptions = 0;
            T result = default(T);
			Exception capturedException = null;
            try
            {
                result = await action(token);
                return result;
            }
            catch (Exception e)
            {
                OnException(e, observer);
				capturedException = e;
                exceptions++;
                throw;
            }
            finally
            {
                sw.Stop();
                var stats = new Statistics<T>(1, exceptions, sw.Elapsed, capturedException, result);
                OnExecute(stats, observer);
            }
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
