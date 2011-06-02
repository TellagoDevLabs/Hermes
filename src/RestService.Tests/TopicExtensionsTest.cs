using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.RestService.Extensions;
using NUnit.Framework;
using TellagoStudios.Hermes.RestService.Resources;
using F = TellagoStudios.Hermes.Facade;

namespace RestService.Tests
{
    [TestFixture]
    public class TopicExtensionsTest
    {
        private string _description;
        private Identity _groupId;
        private string _name;



        [SetUp]
        public void SetUp()
        {
            _description = "Description";
            _groupId = Identity.Random();
            _name = "The awesome group";
        }

        [Test]
        public void FacadeToModelReturnsNullWhenFacadeIsNull()
        {
            F.TopicPost post = null;
            Assert.Null(post.ToModel());

            F.TopicPost put = null;
            Assert.Null(put.ToModel());
        }


        [Test]
        public void FacadePostToModelMapsCorrectly()
        {
            var post = new F.TopicPost
                           {
                               Description = _description,
                               GroupId = _groupId.ToFacade(),
                               Name = _name
                           };

            var model = post.ToModel();

            Assert.That(model.Description, Is.EqualTo(post.Description));
            Assert.That(model.GroupId, Is.EqualTo(post.GroupId.ToModel()));
            Assert.That(model.Name, Is.EqualTo(post.Name));
        }

        [Test]
        public void FacadePutToModelMapsCorrectly()
        {
            var post = new F.TopicPut
            {
                Description = _description,
                GroupId = _groupId.ToFacade(),
                Name = _name
            };

            var model = post.ToModel();

            Assert.That(model.Description, Is.EqualTo(post.Description));
            Assert.That(model.GroupId, Is.EqualTo(post.GroupId.ToModel()));
            Assert.That(model.Name, Is.EqualTo(post.Name));
        }

        [Test]
        public void ModelToFacadeMapsCorrectly()
        {
            var model = new Topic
                            {
                                Id = Identity.Random(),
                                Description = _description,
                                Name = _name,
                                GroupId = _groupId
                            };

            var facade = model.ToFacade();

            Assert.That(facade.Id, Is.EqualTo(model.Id.ToFacade()));
            Assert.That(facade.Description, Is.EqualTo(model.Description));
            Assert.That(facade.Name, Is.EqualTo(model.Name));
            Assert.That(facade.Group, Is.Not.Null);
            Assert.That(facade.Group.rel, Is.EqualTo(TellagoStudios.Hermes.RestService.Constants.Relationships.Group));
            Assert.That(facade.Group.href, Is.EqualTo(ResourceLocation.OfGroup(_groupId)));
        }
    }
}