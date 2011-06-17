using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Retries;

namespace TellagoStudios.Hermes.RestService.Pushing
{
    public class RetryService : IRetryService
    {
        public IEntityById entityById; 
        public ICreateRetryCommand createRetryCommand;
        public IDeleteRetryCommand deleteRetryCommand;
        public IUpdateRetryCommand updateRetryCommand;
        public IGenericJsonPagedQuery genericJsonPagedQuery;

        public bool IsRunning { get; private set; } 

        #region Public Methods
        public RetryService(IEntityById entityById,
            ICreateRetryCommand createRetryCommand,
            IDeleteRetryCommand deleteRetryCommand,
            IUpdateRetryCommand updateRetryCommand,
            IGenericJsonPagedQuery genericJsonPagedQuery)
        {
            this.entityById = entityById;
            this.createRetryCommand = createRetryCommand;
            this.deleteRetryCommand = deleteRetryCommand;
            this.updateRetryCommand = updateRetryCommand;
            this.genericJsonPagedQuery = genericJsonPagedQuery;
        }

        public Retry Add(Retry retry)
        {
            createRetryCommand.Execute(retry);

            ProcessRetries();

            return retry;
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
                var retries = genericJsonPagedQuery.Execute<Retry>(string.Empty, null, null);

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

                        deleteRetryCommand.Execute(retry.Id.Value);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.TraceError(
                            string.Format(Texts.ErrorPushingCallback, retry.Message.Id, retry.Subscription.Id) +
                            "\r\n" + ex);
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
                deleteRetryCommand.Execute(retry.Id.Value);
            else
                updateRetryCommand.Execute(retry);
        } 

        #endregion
    }
}