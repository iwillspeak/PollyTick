using System;

using Polly;

namespace PollyTick
{
    public static class Ticker
    {
		public static TickerInstance WithPolicy(Policy policy)
        {
			return new TickerInstance(policy);
        }
    }
}
