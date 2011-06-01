using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.RestService.Extensions;
using M = TellagoStudios.Hermes.Business.Model;
using NUnit.Framework;
using TellagoStudios.Hermes.Facade;

namespace RestService.Tests
{
    [TestFixture]
    public class GroupExtensionsTest
    {
        private string _description;
        private Identity _id;
        private string _name;
        private Identity _parentId;

        [SetUp]
        public void SetUp()
        {
            _description = "desc";
            _id = Identity.Random();
            _name = "Hey hey!";
            _parentId = Identity.Random();
        }

        [Test]
        public void FacadePutToModelMapsCorrectly()
        {
            var facade = new GroupPut
                             {
                                 Description = _description,
                                 Id = _id,
                                 Name = _name,
                                 ParentId = _parentId
                             };

            var model = facade.ToModel();

            Assert.That(model.Description, Is.EqualTo(_description));
            Assert.That(model.Name, Is.EqualTo(_name));
            Assert.That(model.Id, Is.EqualTo(_id.ToModel()));
            Assert.That(model.ParentId, Is.EqualTo(_parentId.ToModel()));
        }

        [Test]
        public void FacadePostToModelMapsCorrectly()
        {
            var facade = new GroupPost
            {
                Description = _description,
                Name = _name,
                ParentId = _parentId
            };

            var model = facade.ToModel();

            Assert.That(model.Id, Is.Null);
            Assert.That(model.Description, Is.EqualTo(_description));
            Assert.That(model.Name, Is.EqualTo(_name));
            Assert.That(model.ParentId, Is.EqualTo(_parentId.ToModel()));
        }

        [Test]
        public void ModelToFacadeMapsCorrectly()
        {
            var model = new M.Group
            {
                Description = _description,
                Id = _id.ToModel(),
                Name = _name,
                ParentId = _parentId.ToModel()
            };

            var facade = model.ToFacade();

            Assert.That(facade.Description, Is.EqualTo(_description));
            Assert.That(facade.Name, Is.EqualTo(_name));
            Assert.That(facade.Id, Is.EqualTo(_id));
            Assert.That(facade.Parent, Is.Not.Null);
            Assert.That(facade.Parent.rel, Is.EqualTo(TellagoStudios.Hermes.Business.Constants.Relationships.Parent));
            Assert.That(facade.Parent.href, Is.EqualTo(ResourceLocation.OfGroup(_parentId.ToModel())));
        }

        [Test]
        public void GroupsToLinksMapCorrectly()
        {
            var group = new Group { Id = _id };
            var link = group.Id.ToModel().ToLink(TellagoStudios.Hermes.Business.Constants.Relationships.Group);

            Assert.That(link.rel, Is.EqualTo(TellagoStudios.Hermes.Business.Constants.Relationships.Group));
            Assert.That(link.href, Is.EqualTo(ResourceLocation.OfGroup(_id.ToModel())));
        }
    }
}