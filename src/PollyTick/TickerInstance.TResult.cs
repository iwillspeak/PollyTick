using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Polly;

namespace PollyTick
{
    public class TickerInstance<T> : TickerInstanceBase
    {
        public TickerInstance(Policy<T> policy)
            : base()
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
        ///   will recieve a callback with the outcome of the execution.
        /// </summary>
        public Statistics<T> Execute(Func<T> action, IStatisticsObserver observer)
        {
            var sw = Stopwatch.StartNew();
            var result = _policy.ExecuteAndCapture(action);
            sw.Stop();

            return StatisticsFromResult(result, sw, observer);
        }

        /// <summary>
        ///   Execute an awaitable action with instrumentation,
        ///   returning the statistics and execution result.
        /// </summary>
        public Task<Statistics<T>> ExecuteAsync(Func<Task<T>> action)
        {
            return ExecuteAsync(action, new NullObserver());
        }

        /// <summary>
        ///   Execute an awaitable action with instrumentation and a statistics
        ///   observer, returning the execution statistics. The statistics observer
        ///   will recieve a callback with the outcome of the execution.
        /// </summary>
        public async Task<Statistics<T>> ExecuteAsync(Func<Task<T>> action, IStatisticsObserver observer)
        {
            var sw = Stopwatch.StartNew();
            var result = await _policy.ExecuteAndCaptureAsync(action);
            sw.Stop();

            return StatisticsFromResult(result, sw, observer);
        }

        private Policy<T> _policy;
    }
}
