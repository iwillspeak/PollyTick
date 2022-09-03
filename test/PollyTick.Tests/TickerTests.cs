using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

using Polly;
using PollyTick;

namespace PollyTickTests;

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

        var stats = ticker.Execute(() => { });
        Assert.Equal(1, stats.Executions);
        Assert.Equal(0, stats.Exceptions);

        stats = ticker.Execute(() => { });
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
        Assert.NotEqual(TimeSpan.Zero, stats.Elapsed);
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
    public void Ticker_WhenPolicyHasKnownReturnType_PolicyCanBeUsed()
    {
        var i = 100;
        var policy = Policy
            .HandleResult(100)
            .Retry();
        var ticker = Ticker
            .WithPolicy(policy);

        var stats1 = ticker.Execute(() => i++);
        var stats2 = ticker.Execute(() => i++);

        Assert.Equal(101, stats1.Result);
        Assert.Equal(102, stats2.Result);
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
            Assert.NotEqual(TimeSpan.Zero, stats.Elapsed);
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
        Assert.Equal(stats.Elapsed, observer.Elapsed);

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
        Assert.Equal(stats.Elapsed, observer.Elapsed);
    }

    [Fact]
    public async Task Ticker_WithGlobalObserver_AllObserversAreNotified()
    {
        var global = new BookkeepingObserver();
        var sync = new BookkeepingObserver();
        var async = new BookkeepingObserver();

        var ticker = Ticker
            .WithPolicy(Policy.NoOp())
            .WithObserver(global)
            .WithObserver(sync);

        var aticker = Ticker
            .WithPolicy(Policy.NoOpAsync())
            .WithObserver(global)
            .WithObserver(async);

        {
            var one = new BookkeepingObserver();
            ticker.Execute(() => 1000, one);
            await aticker.ExecuteAsync(() => Task.CompletedTask, one);
            ticker.Execute(() => { throw new Exception(); });
            await aticker.ExecuteAsync(() => Task.FromException(new Exception()));

            Assert.Equal(2, one.Executions);
            Assert.Equal(0, one.Exceptions);

            Assert.Equal(4, global.Executions);
            Assert.Equal(2, global.Exceptions);

            Assert.Equal(2, sync.Executions);
            Assert.Equal(1, sync.Exceptions);

            Assert.Equal(2, async.Executions);
            Assert.Equal(1, async.Exceptions);
        }
    }

    [Fact]
    public void Ticker_WhenExceptionIsThrown_ObserverIsNotified()
    {
        var global = new BookkeepingObserver();
        var one = new BookkeepingObserver();
        var ex = new Exception("hello");

        var ticker = Ticker
            .WithPolicy(Policy.NoOp())
            .WithObserver(global);

        ticker.Execute(() =>
                {
                    throw ex;
                }, one);

        Assert.Equal(1, global.Exceptions);
        Assert.Equal(1, global.Executions);
        Assert.Equal(1, one.Exceptions);
        Assert.Equal(1, one.Executions);
        Assert.Equal(ex, global.LastException);
        Assert.Equal(ex, one.LastException);
    }

    [Fact]
    public async Task Ticker_ExecuteWithCancellationToken_StopsExecution()
    {
        var ticker = Ticker.WithPolicy(Policy.NoOpAsync());
        var cts = new CancellationTokenSource();

        var exTask = ticker.ExecuteAsync(ct => Task.Delay(TimeSpan.FromSeconds(5), ct), cts.Token);
        Assert.False(exTask.IsCompleted);
        cts.Cancel();
        var res = await exTask;
    }

    [Fact]
    public void Ticker_ExecuteNoCapture_ReturnsDelegateResult()
    {
        var global = new BookkeepingObserver();
        var ticker = Ticker.WithPolicy(Policy.NoOp<int>())
            .WithObserver(global);
        var local = new BookkeepingObserver();

        var result = ticker.ExecuteNoCapture(() => 223, local);

        Assert.Equal(223, result);
        Assert.Equal(1, local.Executions);
        Assert.Equal(1, global.Executions);
        Assert.Equal(0, local.Exceptions);
        Assert.Equal(0, global.Exceptions);
        Assert.True(local.Elapsed > TimeSpan.Zero);
        Assert.True(global.Elapsed > TimeSpan.Zero);
        Assert.Equal(local.Elapsed, global.Elapsed);
    }

    [Fact]
    public void Ticker_ExecuteNoCaptureWithException_ExceptionIsReported()
    {
        var global = new BookkeepingObserver();
        var ticker = Ticker.WithPolicy(Policy.NoOp<double>())
            .WithObserver(global);
        var local = new BookkeepingObserver();

        Assert.Throws<Exception>(() =>
                {
                    ticker.ExecuteNoCapture(() => throw new Exception(), local);
                });

        Assert.Equal(1, local.Executions);
        Assert.Equal(1, global.Executions);
        Assert.Equal(1, local.Exceptions);
        Assert.Equal(1, global.Exceptions);
        Assert.NotNull(global.LastException);
        Assert.NotNull(local.LastException);
        Assert.True(local.Elapsed > TimeSpan.Zero);
        Assert.True(global.Elapsed > TimeSpan.Zero);
        Assert.Equal(local.Elapsed, global.Elapsed);
    }

    [Fact]
    public async Task Ticker_ExecuteNoCaptureAsync_StatisticsAreReported()
    {
        var global = new BookkeepingObserver();
        var ticker = Ticker.WithPolicy(Policy.NoOpAsync<int>())
            .WithObserver(global);
        var local = new BookkeepingObserver();

        var result = await ticker.ExecuteNoCaptureAsync(() => Task.FromResult(100), local);
        var cts = new CancellationTokenSource(100);
        await Assert.ThrowsAsync<TaskCanceledException>(() =>
                {
                    return ticker.ExecuteNoCaptureAsync(async t =>
                            {
                                await Task.Delay(10_000, t);
                                return 100;
                            }, local, cts.Token);
                });

        Assert.Equal(2, local.Executions);
        Assert.Equal(2, global.Executions);
        Assert.Equal(1, local.Exceptions);
        Assert.Equal(1, global.Exceptions);
        Assert.True(local.Elapsed > TimeSpan.Zero);
        Assert.True(global.Elapsed > TimeSpan.Zero);
        Assert.Equal(local.Elapsed, global.Elapsed);
        Assert.IsType<TaskCanceledException>(local.LastException);
        Assert.IsType<TaskCanceledException>(global.LastException);
    }

    [Fact]
    public void Ticker_ExecuteUntypedPolicyNoCapture_StatistcsAreReported()
    {
        var global = new BookkeepingObserver();
        var ticker = Ticker.WithPolicy(Policy.NoOp())
            .WithObserver(global);
        var local = new BookkeepingObserver();
        var zero = 1 - 1;

        var result = ticker.ExecuteNoCapture(() => 19534, local);
        Assert.Throws<DivideByZeroException>(() =>
                {
                    ticker.ExecuteNoCapture(() => checked(100 / zero), local);
                });

        Assert.Equal(2, local.Executions);
        Assert.Equal(2, global.Executions);
        Assert.Equal(1, local.Exceptions);
        Assert.Equal(1, global.Exceptions);
        Assert.True(local.Elapsed > TimeSpan.Zero);
        Assert.True(global.Elapsed > TimeSpan.Zero);
        Assert.Equal(local.Elapsed, global.Elapsed);
        Assert.IsType<DivideByZeroException>(local.LastException);
        Assert.IsType<DivideByZeroException>(global.LastException);
    }

    [Fact]
    public async Task Ticker_ExecuteUntypedPolicyNoCaptureAsync_StatisticsAreReported()
    {
        var global = new BookkeepingObserver();
        var ticker = Ticker.WithPolicy(Policy.NoOpAsync())
            .WithObserver(global);
        var local = new BookkeepingObserver();
        var run = false;

        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
                {
                    var cts = new CancellationTokenSource(100);
                    await ticker.ExecuteNoCaptureAsync(async ct =>
                            {
                                await Task.Delay(10_000, ct);
                            }, local, cts.Token);
                });
        await ticker.ExecuteNoCaptureAsync(() =>
        {
            run = true;
            return Task.CompletedTask;
        },
            local);

        Assert.True(run);
        Assert.Equal(2, local.Executions);
        Assert.Equal(2, global.Executions);
        Assert.Equal(1, local.Exceptions);
        Assert.Equal(1, global.Exceptions);
        Assert.True(local.Elapsed > TimeSpan.Zero);
        Assert.True(global.Elapsed > TimeSpan.Zero);
        Assert.Equal(local.Elapsed, global.Elapsed);
        Assert.IsType<TaskCanceledException>(local.LastException);
        Assert.IsType<TaskCanceledException>(global.LastException);
    }

    [Fact]
    public async Task Ticker_WhenExceptionIsCaptured_ExposedOnStatistics()
    {
        var ticker = Ticker.WithPolicy(Policy.NoOpAsync());

        var stats = await ticker.ExecuteAsync(() => Task.FromException(new Exception("test_exception")));

        Assert.NotNull(stats.FinalException);
        Assert.Equal("test_exception", stats.FinalException.Message);
    }
}
