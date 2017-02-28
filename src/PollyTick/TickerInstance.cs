using System;
using Polly;

namespace PollyTick
{
    public class TickerInstance
    {
        public TickerInstance(Policy policy)
        {
            _policy = policy;
        }

        public void Execute(Action a) {
            _policy.Execute(a);
        }

        private Policy _policy;
    }
}
