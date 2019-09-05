using Polly;
using System.ComponentModel;

namespace PollyTick
{
    public static class Ticker
    {
        public static TickerInstance WithPolicy(ISyncPolicy policy)
        {
            return new TickerInstance(policy);
        }
        public static TickerInstance<T> WithPolicy<T>(ISyncPolicy<T> policy)
        {
            return new TickerInstance<T>(policy);
        }

        public static AsyncTickerInstance WithPolicy(IAsyncPolicy policy)
        {
            return new AsyncTickerInstance(policy);
        }
        public static AsyncTickerInstance<T> WithPolicy<T>(IAsyncPolicy<T> policy)
        {
            return new AsyncTickerInstance<T>(policy);
        }
    }
}
