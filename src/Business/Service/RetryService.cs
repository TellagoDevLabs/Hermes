using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Repository;

namespace TellagoStudios.Hermes.Business.Service
{
    public class RetryService : IRetryService
    {
        public IRetryRepository Repository { get; set; }
        public ILogService LogService { get; set; }

        public bool IsRunning { get; private set; } 

        #region Public Methods

        public Retry Add(Retry message)
        {
            var newRetry = Repository.Create(message);

            ProcessRetries();

            return newRetry;
        }

        public void ProcessRetries()
        {
            lock (_sync)
            {
                if (IsRunning) return;
                IsRunning = true;
            }

            Task.Factory.StartNew(DoProcess);
        } 

        #endregion

        #region Private members

        private static object _sync = new object();

        private void DoProcess()
        {
            while (true) 
            {
                var retries = Repository.Find(string.Empty, null, null);

                if (retries == null || !retries.Any())
                    break;

                #region Iterate over each retry message

                foreach (var retry in retries)
                {
                    try
                    {
                        retry.Count++;
                        retry.UtcLastTry = DateTime.UtcNow;

                        retry.Message.PushToSubscription(retry.Subscription);

                        Repository.Delete(retry.Id.Value);
                    }
                    catch (Exception ex)
                    {
                        LogService.LogError(string.Format(Messages.ErrorPushingCallback, retry.Message.Id, retry.Subscription.Id), ex);
                        HandleRetryLogic(retry);
                    }
                }
                #endregion

                Thread.Sleep(TimeSpan.FromSeconds(Constants.RetryValues.DelaySeconds));
            }
            lock(_sync) IsRunning = false;
        }

        private void HandleRetryLogic(Retry retry)
        {
            Guard.Instance
                .ArgumentNotNull(() => retry, retry)
                .ArgumentNotNull(() => retry.Id, retry.Id);

            if (retry.Count == Constants.RetryValues.RetryMax)
                Repository.Delete(retry.Id.Value);
            else
                Repository.Update(retry);
        } 

        #endregion
    }
}