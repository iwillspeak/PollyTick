using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PollyTick;

/// <summary>
/// Base class for all async ticker instances
/// </summary>
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
        var start = Stopwatch.GetTimestamp();
        var end = start;
        var exceptions = 0;
        T? result = default;
        Exception? capturedException = null;
        try
        {
            result = await action(token);
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
