using System;

namespace TellagoStudios.Hermes.Business.Retry
{
    /// <summary>
    /// Defines an interface to allow <see cref="RetryableAction"/> to determine if the <see cref="Exception"/>
    /// that caused an <see cref="Action"/> to fail should be rethrown.
    /// </summary>
    public interface IRethrowEvaluator
    {
        /// <summary>
        /// Retruns a value that indicates if the thrown <see cref="Exception"/> should be rethrown or not.
        /// </summary>
        /// <param name="retryCount">The number of times the action has been retried.</param>
        /// <param name="exception">The exception that caused the action to fail.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="exception"/> should be rethrown, <c>false</c> otherwise.
        /// </returns>
        bool Rethrow(int retryCount, Exception exception);
    }
}