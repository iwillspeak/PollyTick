# 🐦⏳ PollyTick ⏳🐦

Execution statistics for Polly policies.

This library is aims to provide a simple wrapper for the wonderful [Polly](http://thepollyproject.org/) to collect statistics about policy executions. It aims to allow programmers keep track of what policies are costing them time, and provide a seam to observe policy executions.

## Example

PollyTick provides a fluent interface for observing the execution of Polly policies.

```C#
var stats = Ticker
    .WithPolicy(Policy.NoOp())
    .Execute(() => 1701);
Assert.Equal(1, stats.Executions);
Assert.Equal(1701, stats.Result);
```

Statistics aren't just returned from each execution. An observer can be registered when preparing the `Ticker`, and passed in to each `Execute` call.

```C#
var overall = new BookkeepingObserver();
var ticker = Ticker
    .WithPolicy(Policy.NoOp())
    .WithObserver(overall);

var one = new BookkeepingObserver();
ticker.Execute(() => 1000, one);
ticker.Execute(() => { throw new Exception(); });

Assert.Equal(2, overall.Executions);
Assert.Equal(1, overall.Exceptions);

Assert.Equal(1, one.Executions);
Assert.Equal(0, one.Exceptions);
```

Naturally there's `async` support too:

```C#
var stats = await Ticker
    .WithPolicy(Policy.NoOpAsync())
    .ExecuteAsync(() => Task.Delay(100));
Assert.True(stats.Elapsed >= TimeSpan.FromMilliseconds(100));
```

## Installation

You can get your hands on `PollyTick` from Nuget.

    PM> Install-Package PollyTick -Pre

or for .NET Core update `project.json`

    "PollyTick": "0.1.0-pre",

## Feature Status

 - [x] Keep track of Execution Time
 - [x] Allow `async` policy execution.
 - [x] Policies with results.
 - [ ] Make capturing of Exceptions optional? Yes: #4
 - [x] Allow observer registration.

## 🐉 Here be Dragons! 🐉

This library is quite new. It's pretty much feature complete but hasn't been tried in the wild yet.
