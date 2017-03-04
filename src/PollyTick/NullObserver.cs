namespace PollyTick
{
    /// <statistics>
    ///   An <see cref="IStatisticsObserver" /> insttance which does
    ///   nothing.
    /// </statistics>
    public class NullObserver : IStatisticsObserver
    {
        public void OnExecute(Statistics statistics)
        {
        }
    }
}