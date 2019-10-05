using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Polly;
using System.Threading;

namespace PollyTick
{
    /// <summary>
    ///  A ticker instance which observes an <see cref="ISyncPolicy{T}" />.
    /// </summary>
    public class TickerInstance<T> : SyncTickerInstanceBase
    {
        private ISyncPolicy<T> _policy;

        internal TickerInstance(ISyncPolicy<T> policy)
        {
            _policy = policy;
        }

        /// <summary>
        ///   Register a statistics observer. The observer will be called
        ///   for all executions of this TickerInstance.
        /// </summary>
        public TickerInstance<T> WithObserver(IStatisticsObserver observer)
        {
            AddObserver(observer);
            return this;
        }

        /// <summary>
        ///  Execute the instrumented body, returning the execution
        ///  statistics and execution result.
        /// </summary>
        public Statistics<T> Execute(Func<T> action)
        {
            return Execute(action, new NullObserver());
        }

        /// <summary>
        ///   Execute the instrumented body with a statistics observer,
        ///   returning the execution statistics. The statistics observer
        ///   will receive a callback with the outcome of the execution.
        /// </summary>
        public Statistics<T> Execute(Func<T> action, IStatisticsObserver observer)
        {
            var sw = Stopwatch.StartNew();
            var result = _policy.ExecuteAndCapture(action);
            sw.Stop();

            return StatisticsFromResult(result, sw, observer);
        }

        /// <summary>
        ///   Execute the Instrumented body without capturing
        ///   exceptions or intercepting the result.
        /// </summary>
        public T ExecuteNoCapture(Func<T> action, IStatisticsObserver observer)
        {
            return ExecuteNoCaptureInternal(() => _policy.Execute(action), observer);
        }
    }
}
