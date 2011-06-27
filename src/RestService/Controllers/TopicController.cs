using System;
using System.Web.Mvc;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.RestService.Controllers
{
    public class TopicController : Controller
    {
        private readonly IRepository<Topic> topicsRepository;

        public TopicController(IRepository<Topic> topicsRepository)
        {
            this.topicsRepository = topicsRepository;
        }

        public ActionResult Index()
        {
            throw new NotImplementedException();
        }
    }
}