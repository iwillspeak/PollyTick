using System;
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
            var exceptions = 0;
            var result = default(T);

            try
            {
                result = _policy.Execute(action);
                sw.Stop();
            }
            catch
            {
                sw.Stop();
                exceptions = 1;
            }
            
            return new Statistics<T>(1, exceptions, sw.ElapsedMilliseconds, result);
        }

        private Policy _policy;
    }
}
