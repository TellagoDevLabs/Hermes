using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.RestService.Models;

namespace TellagoStudios.Hermes.RestService.Controllers
{
    public class GroupController : Controller
    {
        IGenericJsonPagedQuery genericJsonPagedQuery;
        IEntityById entityById;

        public GroupController(IGenericJsonPagedQuery genericJsonPagedQuery, IEntityById entityById)
        {
            this.genericJsonPagedQuery = genericJsonPagedQuery;
            this.entityById = entityById;
        }

        public ActionResult Index()
        {
            var groups = genericJsonPagedQuery.Execute<Group>(null, null, null);
            
            if (groups.Any())
                return View(groups);
            
            return View("EmptyGroups");
        }

        public ActionResult Edit(string id)
        {
            var groupId = new Identity(id);
            var group = entityById.Get<Group>(groupId);
            if (group == null)
                return View("GroupDoesNotExist");
            return View(new GroupModel(group));
        }

        //public ActionResult Cancel()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Cancel(int id)
        //{
        //    //cancels
        //}

    }
}
