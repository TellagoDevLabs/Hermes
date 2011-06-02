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
        protected HttpResponseMessage Process(Action action)
        {
            return Process((response) => action());
        }

        protected HttpResponseMessage Process(Action<HttpResponseMessage> action)
        {
            var response = new HttpResponseMessage();
            response.StatusCode = HttpStatusCode.OK;
            DoProcess(() => action(response));
            return response;
        }

        protected HttpResponseMessage<T> Process<T>(Func<T> getEntity)
        {
            return Process((response) => getEntity());
        }

        protected HttpResponseMessage<T> Process<T>(Func<HttpResponseMessage, T> getEntity)
        {
            var response = new HttpResponseMessage<T>(HttpStatusCode.OK);

            DoProcess(() =>
            {
                var resource = getEntity(response);
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

        private void DoProcess(Action doProcessAction)
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
