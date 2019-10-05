using System;
using System.Diagnostics;

namespace PollyTick
{
    /// <summary>
    ///   Base class for all synchronous ticker instances
    /// </summary>
    public class SyncTickerInstanceBase : TickerInstanceBase
    {
        /// <summary>
        ///   Internal implementation of the `ExecuteNoCapture`.
        /// </summary>
        protected T ExecuteNoCaptureInternal<T>(Func<T> action, IStatisticsObserver observer)
        {
            var sw = Stopwatch.StartNew();
            int exceptions = 0;
            T result = default(T);
			Exception capturedException = null;
            try
            {
                result = action();
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
