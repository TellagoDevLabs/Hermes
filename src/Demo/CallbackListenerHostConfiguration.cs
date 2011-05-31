using System;
using System.Collections.Generic;
using System.Linq;

namespace Demo
{
    class CallbackListenerHostConfiguration : HttpHostConfiguration, IProcessorProvider
    {
        public CallbackListenerHostConfiguration()
        {
        }

        public void RegisterRequestProcessorsForOperation(System.ServiceModel.Description.HttpOperationDescription operation, IList<System.ServiceModel.Dispatcher.Processor> processors, MediaTypeProcessorMode mode)
        {
            if (operation.DeclaringContract.Name == typeof (CallbackListenerService).Name &&
                operation.Name == "Callback")
            {
                var mtProcessors = processors.OfType<MediaTypeProcessor>().ToArray();
                mtProcessors.ForEach(mtp => processors.Remove(mtp));
            }
        }

        public void RegisterResponseProcessorsForOperation(System.ServiceModel.Description.HttpOperationDescription operation, IList<System.ServiceModel.Dispatcher.Processor> processors, MediaTypeProcessorMode mode)
        {
            processors.Add(new JsonProcessor(operation, MediaTypeProcessorMode.Response));
        }
    }
}
