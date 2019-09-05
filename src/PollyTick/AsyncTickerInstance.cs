using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace PollyTick
{
    public class AsyncTickerInstance : AsyncTickerInstanceBase
    {
        private IAsyncPolicy _policy;

        public AsyncTickerInstance(IAsyncPolicy policy)
        {
            _policy = policy;
        }

        /// <summary>
        ///   Register a statistics observer. The observer will be called
        ///   for all executions of this TickerInstance.
        /// </summary>
        public AsyncTickerInstance WithObserver(IStatisticsObserver observer)
        {
            AddObserver(observer);
            return this;
        }

        /// <summary>
        ///   Execute an awaitable action with instrumentation,
        ///   returning the statistics for this execution.
        /// </summary>
        public Task<Statistics> ExecuteAsync(Func<Task> action)
        {
            return ExecuteAsync(action, new NullObserver());
        }

        /// <summary>
        ///   Execute an awaitable action with instrumentation,
        ///   returning the statistics for this execution.
        /// </summary>
        public Task<Statistics> ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken token)
        {
            return ExecuteAsync(action, new NullObserver(), token);
        }

        /// <summary>
        ///   Execute an awaitable action with instrumentation and a statistics
        ///   observer, returning the execution statistics. The statistics observer
        ///   will receive a callback with the outcome of the execution.
        /// </summary>
        public Task<Statistics> ExecuteAsync(Func<Task> action, IStatisticsObserver observer)
        {
            return ExecuteAsync(_ => action(), observer, CancellationToken.None);
        }

        /// <summary>
        ///   Execute the Instrumented body without capturing
        ///   exceptions or intercepting the result.
        /// </summary>
        public Task ExecuteNoCaptureAsync(Func<Task> action, IStatisticsObserver observer)
        {
            return ExecuteNoCaptureAsync(_ => action(), observer, CancellationToken.None);
        }

        /// <summary>
        ///   Execute an awaitable action with instrumentation and a statistics
        ///   observer, returning the execution statistics. The statistics observer
        ///   will receive a callback with the outcome of the execution.
        /// </summary>
        public async Task<Statistics> ExecuteAsync(
            Func<CancellationToken, Task> action,
            IStatisticsObserver observer,
            CancellationToken token)
        {
            return await ExecuteAsync(async ct => {
                    await action(ct);
                    return 0;
                },
                observer,
                token);
        }

        /// <summary>
        ///   Execute the Instrumented body without capturing
        ///   exceptions or intercepting the result.
        /// </summary>
        public async Task ExecuteNoCaptureAsync(
            Func<CancellationToken, Task> action,
            IStatisticsObserver observer,
            CancellationToken token)
        {
            await ExecuteNoCaptureAsync(async ct => {
                    await action(ct);
                    return 0;
                },
                observer,
                token);
        }
        
        /// <summary>
        ///   Execute an awaitable action with instrumentation,
        ///   returning the statistics and execution result.
        /// </summary>
        public Task<Statistics<T>> ExecuteAsync<T>(Func<Task<T>> action)
        {
            return ExecuteAsync(action, new NullObserver());
        }

        /// <summary>
        ///   Execute an awaitable action with instrumentation,
        ///   returning the statistics and execution result.
        /// </summary>
        public Task<Statistics<T>> ExecuteAsync<T>(Func<CancellationToken, Task<T>> action, CancellationToken token)
        {
            return ExecuteAsync(action, new NullObserver(), token);
        }

        /// <summary>
        ///   Execute an awaitable action with instrumentation and a statistics
        ///   observer, returning the execution statistics. The statistics observer
        ///   will receive a callback with the outcome of the execution.
        /// </summary>
        public Task<Statistics<T>> ExecuteAsync<T>(Func<Task<T>> action, IStatisticsObserver observer)
        {
            return ExecuteAsync(_ => action(), observer, CancellationToken.None);
        }

        /// <summary>
        ///   Execute the Instrumented body without capturing
        ///   exceptions or intercepting the result.
        /// </summary>
        public Task<T> ExecuteNoCaptureAsync<T>(Func<Task<T>> action, IStatisticsObserver observer)
        {
            return ExecuteNoCaptureAsync(_ => action(), observer, CancellationToken.None);
        }

        /// <summary>
        ///   Execute an awaitable action with instrumentation and a statistics
        ///   observer, returning the execution statistics. The statistics observer
        ///   will receive a callback with the outcome of the execution.
        /// </summary>
        public async Task<Statistics<T>> ExecuteAsync<T>(
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
        public Task<T> ExecuteNoCaptureAsync<T>(
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
