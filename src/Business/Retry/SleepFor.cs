using System;

namespace TellagoStudios.Hermes.Business.Retry
{
    /// <summary>
    /// 
    /// </summary>
    public static class SleepFor
    {
        /// <summary>
        /// Randoms the exponential.
        /// </summary>
        /// <param name="minimumDelay">The minimum delay.</param>
        /// <returns></returns>
        public static ISleepEvaluator RandomExponential(int minimumDelay)
        {
            return new RandomExponentialSleepEvaluator(minimumDelay);
        }

        /// <summary>
        /// Millisecondses the specified milliseconds.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <returns></returns>
        public static ISleepEvaluator Milliseconds(int milliseconds)
        {
            return new CustomSleepEvaluator((retryCount, exception) => milliseconds);
        }

        /// <summary>
        /// Customs the specified function.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <returns></returns>
        public static ISleepEvaluator Custom(Func<int, Exception, int> function)
        {
            return new CustomSleepEvaluator(function);
        }
    }
}