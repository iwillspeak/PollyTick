using System;
using Xunit;

using Polly;
using PollyTick;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void CreateTicker_WithoutObserver_ExecutesDelegate()
        {
			var foo = false;
			Ticker
				.WithPolicy(Policy.NoOp())
				.Execute(() => foo = true);
			Assert.True(foo);
        }
    }
}
