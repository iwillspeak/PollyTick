using System;

namespace PollyTick
{
    public class CompositeObserver : IStatisticsObserver
    {
        private IStatisticsObserver[] _observers;

        public CompositeObserver(params IStatisticsObserver[] observers)
        {
            _observers = observers;
        }

        public void OnException(Exception exception)
        {
            foreach (var obs in _observers)
                obs.OnException(exception);
        }

        public void OnExecute(Statistics statistics)
        {
            foreach (var obs in _observers)
                obs.OnExecute(statistics);
        }
    }
}
