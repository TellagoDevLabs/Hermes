using System;
using System.Net;
using System.Net.Http;
using Microsoft.ApplicationServer.Http;
using Microsoft.ApplicationServer.Http.Dispatcher;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business;

namespace TellagoStudios.Hermes.RestService.Resources
{
    public abstract class Resource
    {
        protected HttpResponseMessage ProcessPost(Func<Uri> action)
        {
            return Process(HttpStatusCode.Created,
                           (response) => response.Headers.Location = action());
        }

        protected HttpResponseMessage ProcessPut(Action action)
        {
            return Process(HttpStatusCode.NoContent, (response)=>action());
        }

        protected HttpResponseMessage ProcessDelete(Action action)
        {
            return Process(HttpStatusCode.NoContent, (response) => action());
        }

        private HttpResponseMessage Process(HttpStatusCode statusCode, Action<HttpResponseMessage> action)
        {
            var response = new HttpResponseMessage(statusCode, string.Empty);
            DoProcess(() => action(response));
            return response;
        }

        protected HttpResponseMessage<T> ProcessGet<T>(Func<T> getEntity)
            where T: class
        {
            var response = new HttpResponseMessage<T>(HttpStatusCode.OK);

            DoProcess(() =>
            {
                var resource = getEntity();
                if (resource == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                }
                else
                {
                    response.Content = new ObjectContent<T>(resource); 
                }
            });
            
            return response;  
        }

        protected void DoProcess(Action doProcessAction)
        {
            Guard.Instance.ArgumentNotNull(() => doProcessAction, doProcessAction);

            try
            {
                doProcessAction();
            }
            catch (EntityNotFoundException enfe)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Content = new StringContent(enfe.Message)
                });
            }
            catch (ValidationException ve)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(ve.Message)
                });
            }
            catch (ArgumentException ae)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(ae.Message)
                });
            }
            catch (Exception e)
            {         
                
                throw new HttpResponseException(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
#if DEBUG
                    Content = new StringContent(e.ToString())
#else
                    Content = new StringContent("")
#endif
                });
            }
        }
    }
}
