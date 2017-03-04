using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Polly;

namespace PollyTick
{
    public class TickerInstance
    {
        public TickerInstance(Policy policy)
        {
            _policy = policy;
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
        ///   will recieve a callback with the outcome of the execution.
        /// </summary>
        public Statistics Execute(Action action, IStatisticsObserver observer)
        {
            return Execute(() => { action(); return 0; }, observer);
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
        ///   will recieve a callback with the outcome of the execution.
        /// </summary>
        public Statistics<T> Execute<T>(Func<T> action, IStatisticsObserver observer)
        {
            var sw = Stopwatch.StartNew();
            var result = _policy.ExecuteAndCapture(action);
            sw.Stop();
            
            return StatisticsFromResult(result, sw, observer);
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
        ///   Execute an awaitable action with instrumentation and a statistics
        ///   observer, returning the execution statistics. The statistics observer
        ///   will recieve a callback with the outcome of the execution.
        /// </summary>
        public async Task<Statistics> ExecuteAsync(Func<Task> action, IStatisticsObserver observer)
        {
            return await ExecuteAsync(async () => {
                    await action();
                    return 0;
                },
                observer);
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
        ///   Execute an awaitable action with instrumentation and a statistics
        ///   observer, returning the execution statistics. The statistics observer
        ///   will recieve a callback with the outcome of the execution.
        /// </summary>
        public async Task<Statistics<T>> ExecuteAsync<T>(Func<Task<T>> action, IStatisticsObserver observer)
        {
            var sw = Stopwatch.StartNew();
            var result = await _policy.ExecuteAndCaptureAsync(action);
            sw.Stop();

            return StatisticsFromResult(result, sw, observer);
        }

        /// <summary>
        ///   Convert a Polly `ExecutionResult` into an instance of
        ///   our `Statistics` class.
        /// </summary>
        private Statistics<T> StatisticsFromResult<T>(
            PolicyResult<T> result,
            Stopwatch sw,
            IStatisticsObserver observer)
        {
            var failures = result.Outcome == OutcomeType.Successful ?  0 : 1;
            var stats = new Statistics<T>(1, failures, sw.ElapsedMilliseconds, result.Result);
            
            observer.OnExecute(stats);

            return stats;
        }

        private Policy _policy;
    }
}
