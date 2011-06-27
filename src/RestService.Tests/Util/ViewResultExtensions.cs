using System.Web.Mvc;

namespace RestService.Tests.Util
{
    public static class ViewResultExtensions
    {
        public  static TModel GetModel<TModel>(this ActionResult result)
        {
            var viewResult = ((ViewResult) result);
            return (TModel) viewResult.Model;
        }
    }
}