using System;

namespace TellagoStudios.Hermes.Business.Retry
{
    /// <summary>
    /// </summary>
    public class RandomExponentialSleepEvaluator : CustomSleepEvaluator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomExponentialSleepEvaluator"/> class.
        /// </summary>
        public RandomExponentialSleepEvaluator() : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomExponentialSleepEvaluator"/> class.
        /// </summary>
        /// <param name="minimumDelay">The minimum delay.</param>
        public RandomExponentialSleepEvaluator(int minimumDelay) 
            : base((retryCount, exception) => SleepMilliseconds(retryCount, minimumDelay))
        {
        }

        /// <summary>
        /// Sleeps the milliseconds.
        /// </summary>
        /// <param name="retryCount">The retry count.</param>
        /// <param name="minimumDelay">The minimum delay.</param>
        /// <returns></returns>
        private static int SleepMilliseconds(int retryCount, int minimumDelay)
        {
            var random = new Random(Guid.NewGuid().GetHashCode());

            return random.Next((int) Math.Pow(retryCount, 2) + minimumDelay, (int) Math.Pow(retryCount + 1, 2) + 1 + minimumDelay);
        }
    }
}