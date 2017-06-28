using System.Collections.Generic;
using System.Threading.Tasks;
using PollyTick;
using Xunit;
using System;

namespace PollyTickTests
{
    public class BookkeepingObserverTests
    {
        [Fact]
        public async Task Bookkeeper_WithMulitpleUpdatesInParallel_IsThreadSafe()
        {
            var bookkeeper = new BookkeepingObserver();

            var tasks = new List<Task>(100);
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    bookkeeper.OnExecute(new Statistics(1, 2, TimeSpan.FromSeconds(3)));
                }));
            }

            await Task.WhenAll(tasks);

            Assert.Equal(100, bookkeeper.Executions);
            Assert.Equal(200, bookkeeper.Exceptions);
            Assert.Equal(300, bookkeeper.Elapsed.TotalSeconds);
        }
 
        [Fact]
        public void Bookkeeper_WhenExceptionsAreThwon_StoresLastException()
        {
            var bookkeeper = new BookkeepingObserver();

            var ex1 = new Exception("one");
            bookkeeper.OnException(ex1);

            Assert.Equal(ex1, bookkeeper.LastException);
        }

        [Fact]
        public void Bookkeeper_ConvertedToStatistics_ExposesStatisticsSnapshot()
        {
            var bookkeeper = new BookkeepingObserver();

            bookkeeper.OnExecute(new Statistics(1, 2, TimeSpan.FromTicks(3)));
            bookkeeper.OnExecute(new Statistics(4, 5, TimeSpan.FromTicks(6)));

            var stats = bookkeeper.IntoStatistics();

            Assert.Equal(5, stats.Executions);
            Assert.Equal(7, stats.Exceptions);
            Assert.Equal(9, stats.Elapsed.Ticks);
        }
   }
}
