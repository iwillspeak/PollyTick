using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PollyTick
{
    public class AsyncTickerInstanceBase : TickerInstanceBase
    {
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
    }
}
