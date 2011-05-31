using System;
using TellagoStudios.Hermes.Business.Util;

namespace TellagoStudios.Hermes.Business.Retry
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomSleepEvaluator : ISleepEvaluator
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly Func<int, Exception, int> function;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomSleepEvaluator"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="function"/> is null.
        /// </exception>
        public CustomSleepEvaluator(Func<int, Exception, int> function)
        {
            Guard.Instance.ArgumentNotNull(()=>function, function);

            this.function = function;
        }

        /// <summary>
        /// Calculates the number of milliseconds to sleep after an action fails.
        /// </summary>
        /// <param name="retryCount">The number of times the action has been retried.</param>
        /// <param name="exception">The exception that caused the action to fail.</param>
        /// <returns>The number of milliseconds to sleep.</returns>
        public int SleepMilliseconds(int retryCount, Exception exception)
        {
            return function(retryCount, exception);
        }
    }
}