using System;
using System.Diagnostics.Contracts;
using System.Threading;
using TellagoStudios.Hermes.Business.Util;

namespace TellagoStudios.Hermes.Business.Retry
{
    /// <summary>
    /// 
    /// </summary>
    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    /// <summary>
    /// 
    /// </summary>
    public static class RetryableAction
    {
        /// <summary>
        /// Initializes the <see cref="RetryableAction"/> class.
        /// </summary>
        static RetryableAction()
        {
            DefaultRethrowEvaluator = new CustomRethrowEvaluator((i, e) => i == 4);
            DefaultSleepEvaluator = new RandomExponentialSleepEvaluator(5);
        }

        /// <summary>
        /// Gets the default <see cref="IRethrowEvaluator"/> if one is not supplied.
        /// </summary>
        /// <value>The default <see cref="IRethrowEvaluator"/>.</value>
        private static IRethrowEvaluator DefaultRethrowEvaluator { get; set; }

        /// <summary>
        /// Gets the default <see cref="ISleepEvaluator"/> if one is not supplied.
        /// </summary>
        /// <value>The default sleep evaluator.</value>
        private static ISleepEvaluator DefaultSleepEvaluator { get; set; }

        /// <summary>
        /// Retries the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> is null.
        /// </exception>
        public static void Execute(Action action)
        {
            Execute(action, DefaultRethrowEvaluator, DefaultSleepEvaluator);
        }

        /// <summary>
        /// Retries the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="rethrowEvaluator">
        /// A function that determines if the <paramref name="action"/> should be retried.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/> or <paramref name="rethrowEvaluator"/> is null.
        /// </exception>
        public static void Execute(Action action, IRethrowEvaluator rethrowEvaluator)
        {
            Execute(action, rethrowEvaluator, DefaultSleepEvaluator);
        }

        /// <summary>
        /// Retries the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="rethrowEvaluator">The should rethrow func.</param>
        /// <param name="sleepEvaluator">The sleep evaluator.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="action"/>, <paramref name="rethrowEvaluator"/> or <paramref name="sleepEvaluator"/> is null.
        /// </exception>
        public static void Execute(Action action, IRethrowEvaluator rethrowEvaluator, ISleepEvaluator sleepEvaluator)
        {
            Guard.Instance
                .ArgumentNotNull(()=>action, action)
                .ArgumentNotNull(()=>rethrowEvaluator, rethrowEvaluator)
                .ArgumentNotNull(()=>sleepEvaluator, sleepEvaluator);

            for (int i = 0; ; i++)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception exception)
                {
                    if (rethrowEvaluator.Rethrow(i, exception))
                    {
                        throw;
                    }

                    Thread.Sleep(sleepEvaluator.SleepMilliseconds(i, exception));
                }
            }
        }

        /// <summary>
        /// Executes the specified function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="rethrowEvaluator">The rethrow evaluator.</param>
        /// <param name="sleepEvaluator">The sleep evaluator.</param>
        /// <returns></returns>
        public static TResult Execute<TResult>(Func<TResult> function, IRethrowEvaluator rethrowEvaluator, ISleepEvaluator sleepEvaluator)
        {
            Guard.Instance.ArgumentNotNull(()=>function, function)
                .ArgumentNotNull(()=>rethrowEvaluator, rethrowEvaluator)
                .ArgumentNotNull(()=>sleepEvaluator, sleepEvaluator);

            for (int i = 0; ; i++)
            {
                try
                {
                    TResult result = function();
                    return result;
                }
                catch (Exception exception)
                {
                    if (rethrowEvaluator.Rethrow(i, exception))
                    {
                        throw;
                    }

                    Thread.Sleep(sleepEvaluator.SleepMilliseconds(i, exception));
                }
            }
        }

        /// <summary>
        /// Executes the specified function.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="argument1">The argument1.</param>
        /// <param name="rethrowEvaluator">The rethrow evaluator.</param>
        /// <param name="sleepEvaluator">The sleep evaluator.</param>
        /// <returns></returns>
        public static TResult Execute<T1, TResult>(Func<T1, TResult> function, T1 argument1, IRethrowEvaluator rethrowEvaluator, ISleepEvaluator sleepEvaluator)
        {
            Guard.Instance
                .ArgumentNotNull(()=>function, function)
                .ArgumentNotNull(()=>rethrowEvaluator, rethrowEvaluator)
                .ArgumentNotNull(()=>sleepEvaluator, sleepEvaluator);

            for (int i = 0; ; i++)
            {
                try
                {
                    TResult result = function(argument1);
                    return result;
                }
                catch (Exception exception)
                {
                    if (rethrowEvaluator.Rethrow(i, exception))
                    {
                        throw;
                    }

                    Thread.Sleep(sleepEvaluator.SleepMilliseconds(i, exception));
                }
            }
        }

        /// <summary>
        /// Executes the specified function.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="argument1">The argument1.</param>
        /// <param name="argument2">The argument2.</param>
        /// <param name="rethrowEvaluator">The rethrow evaluator.</param>
        /// <param name="sleepEvaluator">The sleep evaluator.</param>
        /// <returns></returns>
        public static TResult Execute<T1, T2, TResult>(Func<T1, T2, TResult> function, T1 argument1, T2 argument2, IRethrowEvaluator rethrowEvaluator, ISleepEvaluator sleepEvaluator)
        {
            Guard.Instance
                .ArgumentNotNull(()=>function, function)
                .ArgumentNotNull(()=>rethrowEvaluator, rethrowEvaluator)
                .ArgumentNotNull(()=>sleepEvaluator, sleepEvaluator);

            for (int i = 0; ; i++)
            {
                try
                {
                    TResult result = function(argument1, argument2);
                    return result;
                }
                catch (Exception exception)
                {
                    if (rethrowEvaluator.Rethrow(i, exception))
                    {
                        throw;
                    }

                    Thread.Sleep(sleepEvaluator.SleepMilliseconds(i, exception));
                }
            }
        }

        /// <summary>
        /// Executes the specified function.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="argument1">The argument1.</param>
        /// <param name="argument2">The argument2.</param>
        /// <param name="argument3">The argument3.</param>
        /// <param name="rethrowEvaluator">The rethrow evaluator.</param>
        /// <param name="sleepEvaluator">The sleep evaluator.</param>
        /// <returns></returns>
        public static TResult Execute<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function, T1 argument1, T2 argument2, T3 argument3, IRethrowEvaluator rethrowEvaluator, ISleepEvaluator sleepEvaluator)
        {
            Guard.Instance.ArgumentNotNull(()=>function, function)
                .ArgumentNotNull(()=>rethrowEvaluator, rethrowEvaluator)
                .ArgumentNotNull(()=>sleepEvaluator, sleepEvaluator);

            for (int i = 0; ; i++)
            {
                try
                {
                    TResult result = function(argument1, argument2, argument3);
                    return result;
                }
                catch (Exception exception)
                {
                    if (rethrowEvaluator.Rethrow(i, exception))
                    {
                        throw;
                    }

                    Thread.Sleep(sleepEvaluator.SleepMilliseconds(i, exception));
                }
            }
        }

        /// <summary>
        /// Executes the specified function.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <typeparam name="T3">The type of the 3.</typeparam>
        /// <typeparam name="T4">The type of the 4.</typeparam>
        /// <typeparam name="T5">The type of the 5.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="function">The function.</param>
        /// <param name="argument1">The argument1.</param>
        /// <param name="argument2">The argument2.</param>
        /// <param name="argument3">The argument3.</param>
        /// <param name="argument4">The argument4.</param>
        /// <param name="argument5">The argument5.</param>
        /// <param name="rethrowEvaluator">The rethrow evaluator.</param>
        /// <param name="sleepEvaluator">The sleep evaluator.</param>
        /// <returns></returns>
        public static TResult Execute<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> function, T1 argument1, T2 argument2, T3 argument3, T4 argument4, T5 argument5, IRethrowEvaluator rethrowEvaluator, ISleepEvaluator sleepEvaluator)
        {
            Guard.Instance.ArgumentNotNull(()=>function, function)
                .ArgumentNotNull(()=>rethrowEvaluator, rethrowEvaluator)
                .ArgumentNotNull(()=>sleepEvaluator, sleepEvaluator);

            for (int i = 0; ; i++)
            {
                try
                {
                    TResult result = function(argument1, argument2, argument3, argument4, argument5);
                    return result;
                }
                catch (Exception exception)
                {
                    if (rethrowEvaluator.Rethrow(i, exception))
                    {
                        throw;
                    }

                    Thread.Sleep(sleepEvaluator.SleepMilliseconds(i, exception));
                }
            }
        }
    }
}