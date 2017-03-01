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
        public Statistics Execute(Action a)
        {
            return Execute(() =>
                    {
                        a();
                        return 0;
                    });
        }

        /// <summary>
        ///  Execute the instrumented body, returning the execution
        ///  statistics and execution result.
        /// </summary>
        public Statistics<T> Execute<T>(Func<T> action)
        {
            var sw = Stopwatch.StartNew();
            var result = _policy.ExecuteAndCapture(action);
            sw.Stop();
            
            return StatisticsFromResult(result, sw);
        }

        /// <summary>
        ///   Execute an awaitable action with instrumentation,
        ///   returning the statistics for this execution.
        /// </summary>
        public async Task<Statistics> ExecuteAsync(Func<Task> action)
        {
            return await ExecuteAsync(async () => {
                    await action();
                    return 0;
                });
        }

        /// <summary>
        ///   Execute an awaitable action with instrumentation,
        ///   returning the statistics and execution result.
        /// </summary>
        public async Task<Statistics<T>> ExecuteAsync<T>(Func<Task<T>> action)
        {
            var sw = Stopwatch.StartNew();
            var result = await _policy.ExecuteAndCaptureAsync(action);
            sw.Stop();

            return StatisticsFromResult(result, sw);
        }

        /// <summary>
        ///   Convert a Polly `ExecutionResult` into an instance of
        ///   our `Statistics` class.
        /// </summary>
        private Statistics<T> StatisticsFromResult<T>(PolicyResult<T> result, Stopwatch sw)
        {
            var failures = result.Outcome == OutcomeType.Successful ?  0 : 1;
            return new Statistics<T>(1, failures, sw.ElapsedMilliseconds, result.Result);
        }

        private Policy _policy;
    }
}
