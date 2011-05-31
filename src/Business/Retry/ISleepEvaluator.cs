using System;

namespace TellagoStudios.Hermes.Business.Retry
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISleepEvaluator
    {
        /// <summary>
        /// Calculates the number of milliseconds to sleep after an action fails.
        /// </summary>
        /// <param name="retryCount">The number of times the action has been retried.</param>
        /// <param name="exception">The exception that caused the action to fail.</param>
        /// <returns>The number of milliseconds to sleep.</returns>
        int SleepMilliseconds(int retryCount, Exception exception);
    }
}