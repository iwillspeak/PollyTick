using System;
using System.Diagnostics;

namespace PollyTick;

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
        var start = Stopwatch.GetTimestamp();
        var end = start;
        int exceptions = 0;
        T? result = default(T);
        Exception? capturedException = null;
        try
        {
            result = action();
            end = Stopwatch.GetTimestamp();
            return result;
        }
        catch (Exception e)
        {
            end = Stopwatch.GetTimestamp();
            OnException(e, observer);
            capturedException = e;
            exceptions++;
            throw;
        }
        finally
        {
            var stats = new Statistics<T>(1, exceptions, TimeSpanFromTicks(end - start), capturedException, result);
            OnExecute(stats, observer);
        }
    }
}
