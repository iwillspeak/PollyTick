using System;

namespace PollyTick;

/// <summary>
///  A statistics observer that forwards on to a collection
///  of inner observers.
/// </summary>
public class CompositeObserver : IStatisticsObserver
{
    private IStatisticsObserver[] _observers;

    /// <summary>
    ///  Create a new observer that wraps the given set of
    ///  <paramref name="observers" />.
    /// </summary>
    /// <param name="observers">The observers to wrap</param>
    public CompositeObserver(params IStatisticsObserver[] observers)
    {
        _observers = observers;
    }

    /// <summary>
    ///  Called by a ticker ticker instance when an execution completes
    /// </summary>
    /// <param name="statistics">The statistics for the given execution</param>
    public void OnExecute(Statistics statistics)
    {
        foreach (var obs in _observers)
            obs.OnExecute(statistics);
    }

    /// <summary>
    ///  Called by a ticker instance when an exception is caught
    /// </summary>
    /// <param name="exception">The exception observed</param>
    public void OnException(Exception exception)
    {
        foreach (var obs in _observers)
            obs.OnException(exception);
    }
}
