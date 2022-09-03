using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace PollyTick
{
    /// <summary>
    ///  A ticker instance which observes an <see cref="IAsyncPolicy{T}" />
    /// </summary>
    /// <typeparam name="T">The return type of the policy execution</typeparam>
    public class AsyncTickerInstance<T> : AsyncTickerInstanceBase
    {
        private IAsyncPolicy<T> _policy;

        internal AsyncTickerInstance(IAsyncPolicy<T> policy)
        {
            _policy = policy;
        }
        
        /// <summary>
        ///   Register a statistics observer. The observer will be called
        ///   for all executions of this TickerInstance.
        /// </summary>
        public AsyncTickerInstance<T> WithObserver(IStatisticsObserver observer)
        {
            AddObserver(observer);
            return this;
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
        ///   will receive a callback with the outcome of the execution.
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
        ///   will receive a callback with the outcome of the execution.
        /// </summary>
        public async Task<Statistics<T>> ExecuteAsync(
            Func<CancellationToken, Task<T>> action,
            IStatisticsObserver observer,
            CancellationToken token)
        {
            var start = Stopwatch.GetTimestamp();
            var result = await _policy.ExecuteAndCaptureAsync(action, token);
            var end = Stopwatch.GetTimestamp();

            return StatisticsFromResult(result, TimeSpanFromTicks(end - start), observer);
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
    }
}
