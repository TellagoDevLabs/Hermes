using System;
using System.Linq;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.RestService;
using TellagoStudios.Hermes.RestService.Extensions;
using TellagoStudios.Hermes.RestService.Resources;
using M = TellagoStudios.Hermes.Business.Model;
using F = TellagoStudios.Hermes.Facade;
using NUnit.Framework;

namespace RestService.Tests
{
    [TestFixture]
    public class SubscriptionExtensionsTest
    {
        private M.Identity _id;
        private M.Identity _topicId;
        private M.Identity _groupId;
        private string _filter;
        private F.Callback _callback;

        [SetUp]
        public void Setup()
        {
            _id = Identity.Random();
            _topicId = Identity.Random();
            _groupId = Identity.Random();
            _filter = "{\"name1\":value1}";
            _callback = new F.Callback
                            {
                                Url = "http://host/service/resource?param1=val1",
                                Kind = F.CallbackKind.Key
                            };
        }

        [Test]
        public void MapsPostToModelCorrectly()
        {
            var post = new F.SubscriptionPost
            {
                Filter = _filter,
                TopicId = _topicId.ToFacade(),
                Callback = _callback,
            };

            var model = post.ToModel();

            Assert.That(model.TargetId, Is.EqualTo(_topicId));
            Assert.That(model.TargetKind, Is.EqualTo(M.TargetKind.Topic));
            Assert.IsNotNull(model.Callback);
            Assert.That(model.Callback.Kind.ToString(), Is.EqualTo(_callback.Kind.ToString()));
            Assert.That(model.Callback.Url.ToString(), Is.EqualTo(_callback.Url));
            Assert.That(model.Filter, Is.EqualTo(_filter));
        }

        [Test]
        public void MapsPutToModelCorrectly()
        {
            var post = new F.SubscriptionPut
            {
                Id = _id.ToFacade(),
                Filter = _filter,
                Callback = _callback
            };

            var model = post.ToModel();

            Assert.That(model.Id, Is.EqualTo(_id));
            Assert.IsNull(model.TargetId);
            Assert.That(model.TargetKind, Is.EqualTo(M.TargetKind.None));
            Assert.IsNotNull(model.Callback);
            Assert.That(model.Callback.Kind.ToString(), Is.EqualTo(_callback.Kind.ToString()));
            Assert.That(model.Callback.Url.ToString(), Is.EqualTo(_callback.Url));
            Assert.That(model.Filter, Is.EqualTo(_filter));
        }

        [Test]
        public void MapsPostToModelCorrectlyGroup()
        {
            var post = new F.SubscriptionPost
            {
                Filter = _filter,
                GroupId = _groupId.ToFacade(),
                Callback = _callback,
            };

            var model = post.ToModel();

            Assert.That(model.TargetId, Is.EqualTo(_groupId));
            Assert.That(model.TargetKind, Is.EqualTo(M.TargetKind.Group));
            Assert.IsNotNull(model.Callback);
            Assert.That(model.Callback.Kind.ToString(), Is.EqualTo(_callback.Kind.ToString()));
            Assert.That(model.Callback.Url.ToString(), Is.EqualTo(_callback.Url));
            Assert.That(model.Filter, Is.EqualTo(_filter));
        }

        [Test]
        public void MapsPutToModelCorrectlyGroup()
        {
            var post = new F.SubscriptionPut
            {
                Id = _id.ToFacade(),
                Filter = _filter,
                Callback = _callback
            };

            var model = post.ToModel();

            Assert.That(model.Id, Is.EqualTo(_id));
            Assert.IsNull(model.TargetId);
            Assert.That(model.TargetKind, Is.EqualTo(M.TargetKind.None));
            Assert.IsNotNull(model.Callback);
            Assert.That(model.Callback.Kind.ToString(), Is.EqualTo(_callback.Kind.ToString()));
            Assert.That(model.Callback.Url.ToString(), Is.EqualTo(_callback.Url));
            Assert.That(model.Filter, Is.EqualTo(_filter));
        }

        [Test]
        public void MapsToFacadeCorrectly()
        {
            var model = new M.Subscription
            {
                Id = _id,
                Filter = _filter,
                TargetId = _topicId,
                TargetKind = M.TargetKind.Topic,
                Callback = new M.Callback
                               {
                                   Url = new Uri(_callback.Url),
                                   Kind = M.CallbackKind.Key
                               }
            };

            var facade = model.ToFacade();

            Assert.That(facade.Id, Is.EqualTo(_id.ToFacade()));
            Assert.IsNotNull(facade.Target);
            Assert.That(facade.Target.rel, Is.EqualTo(TellagoStudios.Hermes.RestService.Constants.Relationships.Topic));
            Assert.That(facade.Target.href, Is.EqualTo(ResourceLocation.OfTopic(_topicId)));
            Assert.IsNotNull(facade.Callback);
            Assert.That(model.Callback.Kind.ToString(), Is.EqualTo(facade.Callback.Kind.ToString()));
            Assert.That(facade.Callback.Url, Is.EqualTo(_callback.Url));
            Assert.That(facade.Filter, Is.EqualTo(_filter));
        }

        [Test]
        public void MapsToFacadeCorrectlyGroup()
        {
            var model = new M.Subscription
            {
                Id = _id,
                Filter = _filter,
                TargetId = _groupId,
                TargetKind = M.TargetKind.Group,
                Callback = new M.Callback
                {
                    Url = new Uri(_callback.Url),
                    Kind = M.CallbackKind.Message
                }
            };

            var facade = model.ToFacade();

            Assert.That(facade.Id, Is.EqualTo(_id.ToFacade()));
            Assert.IsNotNull(facade.Target);
            Assert.That(facade.Target.rel, Is.EqualTo(TellagoStudios.Hermes.RestService.Constants.Relationships.Group));
            Assert.That(facade.Target.href, Is.EqualTo(ResourceLocation.OfGroup(_groupId)));
            Assert.IsNotNull(facade.Callback);
            Assert.That(facade.Callback.Kind.ToString(), Is.EqualTo(facade.Callback.Kind.ToString()));
            Assert.That(facade.Callback.Url, Is.EqualTo(_callback.Url));
            Assert.That(facade.Filter, Is.EqualTo(_filter));
        }

        [Test]
        public void ValidCallbackKeyToModel()
        {
            var facade = new F.Callback
                             {
                                 Kind = F.CallbackKind.Key,
                                 Url = _callback.Url
                             };

            var model = facade.ToModel();
            Assert.IsNotNull(model);
            Assert.That(model.Kind.ToString(), Is.EqualTo(facade.Kind.ToString()));
            Assert.That(model.Url.ToString(), Is.EqualTo(facade.Url));
        }

        [Test]
        public void ValidCallbackFullToModel()
        {
            var facade = new F.Callback
            {
                Kind = F.CallbackKind.Message,
                Url = _callback.Url
            };

            var model = facade.ToModel();
            Assert.IsNotNull(model);
            Assert.That(model.Kind.ToString(), Is.EqualTo(facade.Kind.ToString()));
            Assert.That(model.Url.ToString(), Is.EqualTo(facade.Url));
        }

        [Test]
        public void NullCallbackToModel()
        {
            F.Callback callback = null;
            Assert.IsNull(callback.ToModel());
        }

        [Test]
        public void NullUrlCallbackToModel()
        {
            F.Callback callback = new F.Callback { Kind = F.CallbackKind.Key };
            Assert.IsNull(callback.ToModel());
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void InvalidCallbackToModel()
        {
            var facade = new F.Callback
            {
                Kind = F.CallbackKind.Message,
                Url = "invalid URL"
            };

            facade.ToModel();
        }

        [Test]
        public void ValidCallbackKeyToFacade()
        {
            var model = new M.Callback
            {
                Kind = M.CallbackKind.Key,
                Url = new Uri(_callback.Url)
            };

            var facade = model.ToFacade();
            Assert.IsNotNull(facade);
            Assert.That(facade.Kind.ToString(), Is.EqualTo(model.Kind.ToString()));
            Assert.That(facade.Url, Is.EqualTo(model.Url.ToString()));
        }

        [Test]
        public void ValidCallbackFullToFacade()
        {
            var model = new M.Callback
            {
                Kind = M.CallbackKind.Message,
                Url = new Uri(_callback.Url)
            };

            var facade = model.ToFacade();
            Assert.IsNotNull(facade);
            Assert.That(facade.Kind.ToString(), Is.EqualTo(model.Kind.ToString()));
            Assert.That(facade.Url, Is.EqualTo(model.Url.ToString()));
        }

        [Test]
        public void NullCallbackToFacade()
        {
            M.Callback callback = null;
            Assert.IsNull(callback.ToFacade());
        }

        [Test]
        public void NullUrlCallbackToFacade()
        {
            M.Callback callback = new M.Callback { Kind = M.CallbackKind.Key };
            Assert.IsNull(callback.ToFacade());
        }
    }
}