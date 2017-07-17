using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Polly;
using System.Threading;

namespace PollyTick
{
    public class TickerInstance<T> : TickerInstanceBase
    {
        public TickerInstance(Policy<T> policy)
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
        ///   Execute the Instrumented body without capturing
        ///   exceptions or intercepting the result.
        /// </summary>
        public T ExecuteNoCapture(Func<T> action, IStatisticsObserver observer)
        {
            return ExecuteNoCaptureInternal(() => _policy.Execute(action), observer);
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
        ///   Execute an awaitable action with instrumentation,
        ///   returning the statistics and execution result.
        /// </summary>
        public Task<Statistics<T>> ExecuteAsync(
            Func<CancellationToken, Task<T>> action,
            CancellationToken token)
        {
            return ExecuteAsync(action, new NullObserver(), token);
        }

        /// <summary>
        ///   Execute an awaitable action with instrumentation and a statistics
        ///   observer, returning the execution statistics. The statistics observer
        ///   will recieve a callback with the outcome of the execution.
        /// </summary>
        public Task<Statistics<T>> ExecuteAsync(
            Func<Task<T>> action,
            IStatisticsObserver observer)
        {
            return ExecuteAsync(_ => action(), observer, CancellationToken.None);
        }

        /// <summary>
        ///   Execute the Instrumented body without capturing
        ///   exceptions or intercepting the result.
        /// </summary>
        public Task<T> ExecuteNoCaptureAsync(Func<Task<T>> action, IStatisticsObserver observer)
        {
            return ExecuteNoCaptureAsync(_ => action(), observer, CancellationToken.None);
        }

        /// <summary>
        ///   Execute an awaitable action with instrumentation and a statistics
        ///   observer, returning the execution statistics. The statistics observer
        ///   will recieve a callback with the outcome of the execution.
        /// </summary>
        public async Task<Statistics<T>> ExecuteAsync(
            Func<CancellationToken, Task<T>> action,
            IStatisticsObserver observer,
            CancellationToken token)
        {
            var sw = Stopwatch.StartNew();
            var result = await _policy.ExecuteAndCaptureAsync(action, token);
            sw.Stop();

            return StatisticsFromResult(result, sw, observer);
        }

        /// <summary>
        ///   Execute the Instrumented body without capturing
        ///   exceptions or intercepting the result.
        /// </summary>
        public Task<T> ExecuteNoCaptureAsync(
            Func<CancellationToken, Task<T>> action,
            IStatisticsObserver observer,
            CancellationToken token)
        {
            return ExecuteNoCaptureInternalAsync(
                ct => _policy.ExecuteAsync(action, ct),
                observer,
                token);
        }

        private Policy<T> _policy;
    }
}
