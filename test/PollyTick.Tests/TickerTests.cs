using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

using Polly;
using PollyTick;

namespace PollyTickTests
{
    public class Tests
    {
        [Fact]
        public void Ticker_Execute_ExecutesDelegate()
        {
            var foo = false;
            Ticker
                .WithPolicy(Policy.NoOp())
                .Execute(() => foo = true);
            Assert.True(foo);
        }

        [Fact]
        public void Ticker_Execute_ReturnsExecutionStats()
        {
            var ticker = Ticker
                .WithPolicy(Policy.NoOp());

            var stats = ticker.Execute(() => {});
            Assert.Equal(1, stats.Executions);
            Assert.Equal(0, stats.Exceptions);

            stats = ticker.Execute(() => {});
            Assert.Equal(1, stats.Executions);
            Assert.Equal(0, stats.Exceptions);
        }

        [Fact]
        public void Ticker_ExecuteWithException_StatisticsShowException()
        {
            var ticker = Ticker
                .WithPolicy(Policy.NoOp());

            var stats = ticker.Execute(() => { throw new Exception(); });

            Assert.Equal(1, stats.Exceptions);
        }

        [Fact]
        public void Ticker_WhenBodyTakesTime_TimeIsRecorded()
        {
            var stats = Ticker
                .WithPolicy(Policy.NoOp())
                .Execute(() => Thread.Sleep(TimeSpan.FromSeconds(0.1)));
            Assert.Equal(1, stats.Executions);
            Assert.Equal(0, stats.Exceptions);
            Assert.NotEqual(0, stats.TotalMilliseconds);
        }

        [Fact]
        public void Ticker_WhenBodyReturnsValue_ValueIsAvailable()
        {
            var ticker = Ticker
                .WithPolicy(Policy.NoOp());

            var stats = ticker.Execute(() => 1337);
            Assert.Equal(1337, stats.Result);

            stats = ticker.Execute(() => -9000);
            Assert.Equal(-9000, stats.Result);

            var boolStats = ticker.Execute(() => false);
            Assert.False(boolStats.Result);
        }

        [Fact]
        public async Task Ticker_WithAsyncBody_ExecutesBody()
        {
            var ticker = Ticker
                .WithPolicy(Policy.NoOpAsync());

            {
                var stats = await ticker.ExecuteAsync(() => Task.Delay(100));
                Assert.Equal(1, stats.Executions);
                Assert.Equal(0, stats.Exceptions);
                Assert.NotEqual(0, stats.TotalMilliseconds);
            }

            {
                var stats = await ticker.ExecuteAsync(() => Task.FromResult(1701));
                Assert.Equal(1701, stats.Result);
            }
        }

        [Fact]
        public void Ticker_ExecuteWithObserver_ObserverSeesStatistics()
        {
            var ticker = Ticker
                .WithPolicy(Policy.NoOp());
            var observer = new BookkeepingObserver();
            var stats = ticker.Execute(() => { }, observer);

            Assert.Equal(1, stats.Executions);
            Assert.Equal(0, observer.Exceptions);
            Assert.Equal(1, observer.Executions);
            Assert.Equal(stats.TotalMilliseconds, observer.TotalMilliseconds);

            stats = ticker.Execute(() => { throw new Exception(); }, observer);
            Assert.Equal(1, stats.Executions);
            Assert.Equal(1, observer.Exceptions);
            Assert.Equal(2, observer.Executions);
        }

        [Fact]
        public async Task Ticker_AsyncExecuteWithObserver_ObserverSeesStatistics()
        {
            var ticker = Ticker
                .WithPolicy(Policy.NoOpAsync());
            var observer = new BookkeepingObserver();

            var stats = await ticker.ExecuteAsync(() => Task.CompletedTask, observer);
            
            Assert.Equal(1, stats.Executions);
            Assert.Equal(0, observer.Exceptions);
            Assert.Equal(1, observer.Executions);
            Assert.Equal(stats.TotalMilliseconds, observer.TotalMilliseconds);
        }
    }
}
