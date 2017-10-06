using System;
using PollyTick;
using Xunit;

namespace PollyTickTests
{
    public class CompositeObserverTests
    {
        [Fact]
        public void Composite_OnExecute_CallsAllChildObservers()
        {
            var book1 = new BookkeepingObserver();
            var book2 = new BookkeepingObserver();
            var observer = new CompositeObserver(book1, book2);
                                                 
            observer.OnExecute(new Statistics(1, 2, TimeSpan.FromSeconds(3), new Exception("hello")));

            Assert.Equal(1, book1.Executions);
            Assert.Equal(1, book2.Executions);
            Assert.Equal(2, book1.Exceptions);
            Assert.Equal(2, book2.Exceptions);
            Assert.Equal(3, book1.Elapsed.TotalSeconds);
            Assert.Equal(3, book2.Elapsed.TotalSeconds);
        }

        [Fact]
        public void Composite_OnException_AllChildObserversReceiveException()
        {
            var book1 = new BookkeepingObserver();
            var book2 = new BookkeepingObserver();
            var observer = new CompositeObserver(book1, book2);
                                                 
            var ex = new NotSupportedException();
            observer.OnException(ex);

            Assert.Equal(ex, book1.LastException);
            Assert.Equal(ex, book2.LastException);
        }
    }
}
