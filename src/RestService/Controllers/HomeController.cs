using System;
using System.Web.Mvc;
using TellagoStudios.Hermes.Business.Data.Queries;

namespace TellagoStudios.Hermes.RestService.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITopicsStatistics topicsStatistics;

        public HomeController(ITopicsStatistics topicsStatistics)
        {
            this.topicsStatistics = topicsStatistics;
        }

        public ActionResult Index()
        {
            ViewBag.TopicsStatistics = topicsStatistics.Execute();
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
