using System;
using TellagoStudios.Hermes.Business.Util;

namespace TellagoStudios.Hermes.Business.Retry
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomRethrowEvaluator : IRethrowEvaluator
    {
        private readonly Func<int, Exception, bool> function;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomSleepEvaluator"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="function"/> is null.
        /// </exception>
        public CustomRethrowEvaluator(Func<int, Exception, bool> function)
        {
            Guard.Instance.ArgumentNotNull(()=>function, function);

            this.function = function;
        }

        /// <summary>
        /// Retruns a value that indicates if the thrown <see cref="Exception"/> should be rethrown or not.
        /// </summary>
        /// <param name="retryCount">The number of times the action has been retried.</param>
        /// <param name="exception">The exception that caused the action to fail.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="exception"/> should be rethrown, <c>false</c> otherwise.
        /// </returns>
        public bool Rethrow(int retryCount, Exception exception)
        {
            return function(retryCount, exception);
        }
    }
}