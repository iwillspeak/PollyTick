using System;
using System.Diagnostics;
using Polly;

namespace PollyTick
{
    public class TickerInstance
    {
        public TickerInstance(Policy policy)
        {
            _policy = policy;
        }

        public Statistics Execute(Action a) {
            var sw = Stopwatch.StartNew();
            var exceptions = 0;

            try
            {
                _policy.Execute(a);
                sw.Stop();
            }
            catch
            {
                sw.Stop();
                exceptions = 1;
            }
            
            return new Statistics(1, exceptions, sw.ElapsedMilliseconds);
        }

        private Policy _policy;
    }
}
