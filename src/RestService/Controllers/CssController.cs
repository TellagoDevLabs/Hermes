using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using dotless.Core;

namespace TellagoStudios.Hermes.RestService.Controllers
{
    public class CssController : Controller
    {
        [OutputCache(Duration = 10, VaryByParam = "")]
        public ActionResult Index(string filename)
        {
            var filepath = Server.MapPath("~/public/css/" + filename + ".less");
            if (filepath == null)
            {
                return HttpNotFound();
            }
            using (var stream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                var parameters = new Dictionary<string, string>();
                //parameters["backgroundcolor"] = "#1f1400"; // continue for all replaceable parameters
                return new DotLessResult(stream, parameters, true);
            }
        }
    }
}