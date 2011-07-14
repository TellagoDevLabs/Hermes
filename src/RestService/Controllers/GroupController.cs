using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Groups;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.RestService.Models;

namespace TellagoStudios.Hermes.RestService.Controllers
{
    public class GroupController : Controller
    {
        readonly IGenericJsonPagedQuery genericJsonPagedQuery;
        readonly IEntityById entityById;
        readonly ICreateGroupCommand createGroupCommand;
        readonly IUpdateGroupCommand updateGroupCommand;
        readonly IDeleteGroupCommand deleteGroupCommand;

        public GroupController(
            IGenericJsonPagedQuery genericJsonPagedQuery,
            IEntityById entityById,
            ICreateGroupCommand createGroupCommand,
            IUpdateGroupCommand updateGroupCommand,
            IDeleteGroupCommand deleteGroupCommand
            )
        {
            this.genericJsonPagedQuery = genericJsonPagedQuery;
            this.entityById = entityById;
            this.createGroupCommand = createGroupCommand;
            this.updateGroupCommand = updateGroupCommand;
            this.deleteGroupCommand = deleteGroupCommand;
        }

        public ActionResult Index()
        {
            var groups = genericJsonPagedQuery.Execute<Group>(null, null, null);
            
            return View(groups);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            var groupId = new Identity(id);
            var group = entityById.Get<Group>(groupId);
            if (group == null)
                return View("GroupDoesNotExist");


            var model = new GroupEditModel
            {
                Group = new GroupModel 
                    {
                    Id = group.Id.ToString(),
                    Name = group.Name,
                    Description = group.Description,
                    ParentId = group.ParentId.ToString()
                    },
                GroupList = GetGroupList(groupId)
            };

            return View(model);
        }

        private IEnumerable<GroupModel> GetGroupList(Identity? exclude = null)
        {
           return new[] {new GroupModel {Name = "(none)"}}
                .Union(genericJsonPagedQuery.Execute<Group>(null, null, null)
                           .Where(g => g.Id != exclude)
                           .Select(g => new GroupModel {Id = g.Id.ToString(), Name = g.Name}));
        }

        [HttpPost]
        public ActionResult Edit(GroupEditModel model)
        {
            try
            {
                var group = new Group
                                {
                                    Id = new Identity(model.Group.Id),
                                    Name = model.Group.Name,
                                    Description = model.Group.Description,
                                    ParentId =
                                        model.Group.ParentId == null
                                            ? new Identity?()
                                            : new Identity(model.Group.ParentId)
                                };

                updateGroupCommand.Execute(group);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(Guid.NewGuid().ToString(), e);
            }
            model.GroupList = GetGroupList(new Identity(model.Group.Id));
            return View(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var groupList = new[] { new GroupModel { Name = "(none)" } }
                .Union(genericJsonPagedQuery.Execute<Group>(null, null, null)
                    .Select(g => new GroupModel { Id = g.Id.ToString(), Name = g.Name }));

            var model = new GroupCreateModel
            {
                GroupList = groupList
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(GroupCreateModel model)
        {
            try
            {
                var group = new Group
                {
                    Name = model.Name,
                    Description = model.Description,
                    ParentId = model.ParentId == null ? new Identity?() : new Identity(model.ParentId)
                };

                createGroupCommand.Execute(group);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(Guid.NewGuid().ToString(), e);
            }
            model.GroupList = GetGroupList();
            return View(model);
        }

        [HttpGet]
        public ActionResult Delete(string id)
        {
            try
            {
                deleteGroupCommand.Execute(new Identity(id));
            }
            catch (Exception e)
            {
                ModelState.AddModelError(Guid.NewGuid().ToString(), e);
            }
            return RedirectToAction("Index");
        }
    }
}
