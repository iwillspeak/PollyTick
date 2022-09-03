using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Polly;
using System.Threading;

namespace PollyTick
{
    /// <summary>
    ///  A ticker instance which observes an <see cref="ISyncPolicy" />.
    /// </summary>
    public class TickerInstance : SyncTickerInstanceBase
    {
        private ISyncPolicy _policy;

        internal TickerInstance(ISyncPolicy policy)
        {
            _policy = policy;
        }

        /// <summary>
        ///   Register a statistics observer. The observer will be called
        ///   for all executions of this TickerInstance.
        /// </summary>
        public TickerInstance WithObserver(IStatisticsObserver observer)
        {
            AddObserver(observer);
            return this;
        }

        /// <summary>
        ///   Execute the instrumented body, returning the execution
        ///   statistics for this execution.
        /// </summary>
        public Statistics Execute(Action action)
        {
            return Execute(action, new NullObserver());
        }

        /// <summary>
        ///   Execute the instrumented body with a statistics observer,
        ///   returning the execution statistics. The statistics observer
        ///   will receive a callback with the outcome of the execution.
        /// </summary>
        public Statistics Execute(Action action, IStatisticsObserver observer)
        {
            return Execute(() => { action(); return 0; }, observer);
        }

        /// <summary>
        ///   Execute the Instrumented body without capturing
        ///   exceptions or intercepting the result.
        /// </summary>
        public void ExecuteNoCapture(Action action, IStatisticsObserver observer)
        {
            ExecuteNoCapture(() => { action(); return 0; }, observer);
        }

        /// <summary>
        ///  Execute the instrumented body, returning the execution
        ///  statistics and execution result.
        /// </summary>
        public Statistics<T> Execute<T>(Func<T> action)
        {
            return Execute(action, new NullObserver());
        }

        /// <summary>
        ///   Execute the instrumented body with a statistics observer,
        ///   returning the execution statistics. The statistics observer
        ///   will receive a callback with the outcome of the execution.
        /// </summary>
        public Statistics<T> Execute<T>(Func<T> action, IStatisticsObserver observer)
        {
            var start = Stopwatch.GetTimestamp();
            var result = _policy.ExecuteAndCapture(action);
            var end = Stopwatch.GetTimestamp();
            
            return StatisticsFromResult(result, TimeSpanFromTicks(end - start), observer);
        }

        /// <summary>
        ///   Execute the Instrumented body without capturing
        ///   exceptions or intercepting the result.
        /// </summary>
        public T ExecuteNoCapture<T>(Func<T> action, IStatisticsObserver observer)
        {
            return ExecuteNoCaptureInternal(() => _policy.Execute(action), observer);
        }
    }
}
