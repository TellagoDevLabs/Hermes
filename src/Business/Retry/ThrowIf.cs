using System;

namespace TellagoStudios.Hermes.Business.Retry
{
    /// <summary>
    /// </summary>
    public static class ThrowIf
    {
        /// <summary>
        /// Rethrows the exception if the <paramref name="function"/> evaluates to true.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <returns></returns>
        public static IRethrowEvaluator Custom(Func<int, Exception, bool> function)
        {
            return new CustomRethrowEvaluator(function);
        }

        /// <summary>
        /// Rethrows the exception if the count is equal to <paramref name="retryCount"/>.
        /// </summary>
        /// <param name="retryCount">The retry count.</param>
        /// <returns></returns>
        public static IRethrowEvaluator RetryCountIs(int retryCount)
        {
            return new CustomRethrowEvaluator((i, e) => i == retryCount);
        }

        /// <summary>
        /// Alwayses rethrows the exception.
        /// </summary>
        /// <returns></returns>
        public static IRethrowEvaluator Always
        {
            get { return new CustomRethrowEvaluator((i, e) => true); }
        }
    }
}
