using Polly;
using System.ComponentModel;

namespace PollyTick
{
    /// <summary>
    ///  Factory methods for creating ticker instances
    /// </summary>
    public static class Ticker
    {
        /// <summary>
        ///   Create a ticker instance wrapping a synchronous policy
        ///   with no return value.
        /// </summary>
        /// <param name="policy">The policy to wrap</param>
        /// <returns>A ticker instance to observe policy excecutions</returns>
        public static TickerInstance WithPolicy(ISyncPolicy policy)
        {
            return new TickerInstance(policy);
        }

        /// <summary>
        ///   Create a ticker instance wrapping a synchronous policy
        ///   which returns <typeparamref name="T" />.
        /// </summary>
        /// <param name="policy">The policy to wrap</param>
        /// <typeparam name="T">The return type of the policy execution</typeparam>
        /// <returns>A ticker instance to observe policy excecutions</returns>
        public static TickerInstance<T> WithPolicy<T>(ISyncPolicy<T> policy)
        {
            return new TickerInstance<T>(policy);
        }

        /// <summary>
        ///   Create a ticker instance wrapping an asynchronous policy
        ///   with no return value.
        /// </summary>
        /// <param name="policy">The policy to wrap</param>
        /// <returns>A ticker instance to observe policy excecutions</returns>
        public static AsyncTickerInstance WithPolicy(IAsyncPolicy policy)
        {
            return new AsyncTickerInstance(policy);
        }

        /// <summary>
        ///   Create a ticker instance wrapping an asynchronous policy
        ///   which returns <typeparamref name="T" />.
        /// </summary>
        /// <param name="policy">The policy to wrap</param>
        /// <typeparam name="T">The return type of the policy execution</typeparam>
        /// <returns>A ticker instance to observe policy excecutions</returns>
        public static AsyncTickerInstance<T> WithPolicy<T>(IAsyncPolicy<T> policy)
        {
            return new AsyncTickerInstance<T>(policy);
        }
    }
}
