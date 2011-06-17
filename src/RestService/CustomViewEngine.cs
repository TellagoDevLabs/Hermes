using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TellagoStudios.Hermes.RestService
{
    public class CustomViewEngine : WebFormViewEngine
    {
        public CustomViewEngine()
        {
            var locs = new List<string>(base.ViewLocationFormats);
            locs.Add("~/Public/{1}/{0}"); //My personal choice
            locs.Add("~/Views/{1}/{0}");  //An alternative choice
            base.ViewLocationFormats = locs.ToArray();
        }
    }
}