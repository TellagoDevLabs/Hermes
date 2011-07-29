using System;
using System.Web.Mvc;
using TellagoStudios.Hermes.Business.Data.Queries;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Topics;
using TellagoStudios.Hermes.RestService.Models;

namespace TellagoStudios.Hermes.RestService.Controllers
{
    public class TopicController : Controller
    {
        private readonly ITopicsSortedByName topicsSortedByName;
        private readonly IGroupsSortedByName groupsSortedByName;
        private readonly IEntityById entityById;
        readonly ICreateTopicCommand createTopicCommand;
        readonly IUpdateTopicCommand updateTopicCommand;
        readonly IDeleteTopicCommand deleteTopicCommand;

        public TopicController(
            IEntityById entityById,
            ITopicsSortedByName topicsSortedByName, 
            IGroupsSortedByName groupsSortedByName,
            ICreateTopicCommand createTopicCommand,
            IUpdateTopicCommand updateTopicCommand,
            IDeleteTopicCommand deleteTopicCommand)
        {
            this.entityById = entityById;
            this.topicsSortedByName = topicsSortedByName;
            this.groupsSortedByName = groupsSortedByName;
            this.createTopicCommand = createTopicCommand;
            this.updateTopicCommand = updateTopicCommand;
            this.deleteTopicCommand = deleteTopicCommand;
        }

        public ActionResult Index()
        {
            return View(topicsSortedByName.Execute());
        }

        public ActionResult Edit(string id)
        {
            var identity = new Identity(id);
            var entity = entityById.Get<Topic>(identity);
            if(entity == null) return new HttpStatusCodeResult(404);
            var model = new EditTopicModel(entity, groupsSortedByName.Execute());
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EditTopicModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var entity = entityById.Get<Topic>((Identity) model.TopicId);
            ModelToEntity(model, entity);
            updateTopicCommand.Execute(entity);
            return RedirectToAction("Index");
        }

        private static void ModelToEntity(EditTopicModel model, Topic entity)
        {
            entity.Name = model.Name;
            entity.Description = model.Description;
            if (!string.IsNullOrEmpty(model.Group))
            {
                entity.GroupId = (Identity) model.Group;
            }
        }

        public ActionResult Delete(string id)
        {
            try
            {
                deleteTopicCommand.Execute(new Identity(id));
            }
            catch (Exception e)
            {
                ModelState.AddModelError(Guid.NewGuid().ToString(), e);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            var model = new EditTopicModel(this.groupsSortedByName.Execute());
            return View(model);   
        }

        [HttpPost]
        public ActionResult Create(EditTopicModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var topic = new Topic();
            ModelToEntity(model, topic);
            createTopicCommand.Execute(topic);
            return RedirectToAction("Index");
        }
    }
}