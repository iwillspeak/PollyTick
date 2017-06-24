using Polly;

namespace PollyTick
{
    public static class Ticker
    {
        public static TickerInstance WithPolicy(Policy policy)
        {
            return new TickerInstance(policy);
        }

        public static TickerInstance<T> WithPolicy<T>(Policy<T> policy)
        {
            return new TickerInstance<T>(policy);
        }
    }
}
