using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace Demo
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class CallbackListenerService
    {
        [OperationContract]
        [WebInvoke(Method="POST", UriTemplate="*")]
        public void Callback(HttpRequestMessage request)
        {
            MessageRecievedForm.ShowRequest(request);
        }
    }
}
