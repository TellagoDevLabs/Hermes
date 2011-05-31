using System;
using NUnit.Framework;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.DataAccess.MongoDB;

namespace DataAccess.Tests.Repository
{
    class MongoDbSubscriptionFixture : MongoDbBaseFixture
    {
        private ISubscriptionRepository repository;

        public override void FixtureSetUp()
        {
            base.FixtureSetUp();

            repository = new MongoDbSubscriptionRepository(connectionString);
        }

        [Test]
        public void Create_targeting_a_topic()
        {
            var source = new Subscription
            {
                Callback = new Callback
                                {
                                    Kind = CallbackKind.Key,
                                    Url = new Uri(Utils.RandomUri())
                                },
                Filter = Utils.RandomQuery(),
                TargetId = Identity.Random(Utils.MongoObjectId),
                TargetKind = TargetKind.Topic
            };

            var created = repository.Create(source);
            Validate(source, created);
        }

        [Test]
        public void Create_targeting_a_group()
        {
            var source = new Subscription
            {
                Callback = new Callback
                {
                    Kind = CallbackKind.Key,
                    Url = new Uri(Utils.RandomUri())
                },
                Filter = Utils.RandomQuery(),
                TargetId = Identity.Random(Utils.MongoObjectId),
                TargetKind = TargetKind.Group
            };

            var created = repository.Create(source);
            Validate(source, created);
        }

        [Test]
        public void Create_with_null_callback()
        {
            var source = new Subscription
            {
                Filter = Utils.RandomQuery(),
                TargetId = Identity.Random(Utils.MongoObjectId),
                TargetKind = TargetKind.Topic
            };

            var created = repository.Create(source);
            Validate(source, created);
        }


        [Test]
        public void Create_with_null_filter()
        {
            var source = new Subscription
            {
                Callback = new Callback
                {
                    Kind = CallbackKind.Key,
                    Url = new Uri(Utils.RandomUri())
                },
                TargetId = Identity.Random(Utils.MongoObjectId),
                TargetKind = TargetKind.Topic
            };

            var created = repository.Create(source);
            Validate(source, created);
        }

        [Test]
        public void Get_with_topic()
        {
            var source = new Subscription
            {
                Callback = new Callback
                {
                    Kind = CallbackKind.Key,
                    Url = new Uri(Utils.RandomUri())
                },
                Filter = Utils.RandomQuery(),
                TargetId = Identity.Random(Utils.MongoObjectId),
                TargetKind = TargetKind.Topic
            };

            var created = repository.Create(source);
            Assert.IsNotNull(created);
            Assert.IsNotNull(created.Id);
            var result = repository.Get(created.Id.Value);
            Validate(created, result, true);
        }

        [Test]
        public void Get_with_group()
        {
            var source = new Subscription
            {
                Callback = new Callback
                {
                    Kind = CallbackKind.Key,
                    Url = new Uri(Utils.RandomUri())
                },
                Filter = Utils.RandomQuery(),
                TargetId = Identity.Random(Utils.MongoObjectId),
                TargetKind = TargetKind.Group
            };

            var created = repository.Create(source);
            Assert.IsNotNull(created);
            Assert.IsNotNull(created.Id);
            var result = repository.Get(created.Id.Value);
            Validate(created, result, true);
        }

        [Test]
        public void Get_with_null_callback()
        {
            var source = new Subscription
            {
                Filter = Utils.RandomQuery(),
                TargetId = Identity.Random(Utils.MongoObjectId),
                TargetKind = TargetKind.Topic
            };

            var created = repository.Create(source);
            Assert.IsNotNull(created);
            Assert.IsNotNull(created.Id);
            var result = repository.Get(created.Id.Value);
            Validate(created, result, true);
        }


        [Test]
        public void Get_with_null_filter()
        {
            var source = new Subscription
            {
                Callback = new Callback
                {
                    Kind = CallbackKind.Key,
                    Url = new Uri(Utils.RandomUri())
                },
                TargetId = Identity.Random(Utils.MongoObjectId),
                TargetKind = TargetKind.Topic
            };

            var created = repository.Create(source);
            Assert.IsNotNull(created);
            Assert.IsNotNull(created.Id);
            var result = repository.Get(created.Id.Value);
            Validate(created, result, true);
        }

        [Test]
        public void Update_all()
        {
            var source = new Subscription
            {
                Callback = new Callback
                {
                    Kind = CallbackKind.Key,
                    Url = new Uri(Utils.RandomUri())
                },
                Filter = Utils.RandomQuery(),
                TargetId = Identity.Random(Utils.MongoObjectId),
                TargetKind = TargetKind.Topic
            };

            var created = repository.Create(source);
            Validate(created, source);

            // Update all properties
            var toUpdate = new Subscription
            {
                Id = created.Id,
                Callback = new Callback
                {
                    Kind = CallbackKind.Message,
                    Url = new Uri(Utils.RandomUri())
                },
                Filter = Utils.RandomQuery(),
            };

            var updated = repository.Update(toUpdate);
            Validate(toUpdate, updated, true, false);

            var result = repository.Get(created.Id.Value);
            Validate(toUpdate, result, true, false);
        }

        [Test]
        public void Update_min()
        {
            var source = new Subscription
            {
                Callback = new Callback
                {
                    Kind = CallbackKind.Key,
                    Url = new Uri(Utils.RandomUri())
                },
                Filter = Utils.RandomQuery(),
                TargetId = Identity.Random(Utils.MongoObjectId),
                TargetKind = TargetKind.Topic
            };

            var created = repository.Create(source);
            Validate(created, source);

            // Remove Propertis
            var toUpdate = new Subscription
            {
                Id = created.Id,
                Callback = null,
                Filter = null,
                TargetId = created.TargetId,
                TargetKind = created.TargetKind
            };

            var updated = repository.Update(toUpdate);
            Validate(toUpdate, updated, true);

            var result = repository.Get(created.Id.Value);
            Validate(toUpdate, result, true);
        }

        [Test]
        public void Update_max()
        {
            var source = new Subscription
            {
                Callback = null,
                Filter = null,
                TargetId = Identity.Random(Utils.MongoObjectId),
                TargetKind = TargetKind.Topic
            };

            var created = repository.Create(source);
            Validate(created, source);

            // Update all properties
            var toUpdate = new Subscription
            {
                Id = created.Id,
                Callback = new Callback
                {
                    Kind = CallbackKind.Message,
                    Url = new Uri(Utils.RandomUri())
                },
                Filter = Utils.RandomQuery(),
                TargetId = created.TargetId,
                TargetKind = created.TargetKind
            };

            var updated = repository.Update(toUpdate);
            Validate(toUpdate, updated, true);

            var result = repository.Get(created.Id.Value);
            Validate(toUpdate, result, true);
        }

        [Test]
        public void delete()
        {
            var source = new Subscription
            {
                Callback = new Callback
                {
                    Kind = CallbackKind.Key,
                    Url = new Uri(Utils.RandomUri())
                },
                Filter = Utils.RandomQuery(),
                TargetId = Identity.Random(Utils.MongoObjectId),
                TargetKind = TargetKind.Topic
            };

            var created = repository.Create(source);
            Assert.IsNotNull(created);
            Assert.IsNotNull(created.Id);

            var result = repository.Get(created.Id.Value);
            Assert.IsNotNull(result);

            // Remove subscription
            repository.Delete(created.Id.Value);

            result = repository.Get(created.Id.Value);
            Assert.IsNull(result);
        }

        [Test]
        public void delete_unexisting()
        {
            var unexistingId = Identity.Random(Utils.MongoObjectId);
            repository.Delete(unexistingId);
        }

        [Test]
        public void ExistsById_Valid()
        {
            var source = new Subscription
            {
                Callback = new Callback
                {
                    Kind = CallbackKind.Key,
                    Url = new Uri(Utils.RandomUri())
                },
                Filter = Utils.RandomQuery(),
                TargetId = Identity.Random(Utils.MongoObjectId),
                TargetKind = TargetKind.Topic
            };

            var created = repository.Create(source);
            Assert.IsNotNull(created);
            Assert.IsNotNull(created.Id);

            var result = repository.ExistsById(created.Id.Value);
            Assert.IsTrue(result);
        }

        [Test]
        public void ExistsById_invalid()
        {
            var unexistingId = Identity.Random(Utils.MongoObjectId);
            var result = repository.ExistsById(unexistingId);
            Assert.IsFalse(result);
        }

        [Test]
        public void IsQueryValid_True()
        {
            var query = Utils.RandomQuery();
            var result = repository.IsQueryValid(query);
            Assert.IsTrue(result);
        }

        [Test]
        public void IsQueryValid_False()
        {
            var query = "Invalid query sintax!";
            var result = repository.IsQueryValid(query);
            Assert.IsFalse(result);
        }


        #region Private members

        private void Validate(Subscription one, Subscription two, bool includeId = false, bool includeTarget = true)
        {
            Assert.IsNotNull(one);
            Assert.IsNotNull(two);

            if (includeId)
            {
                Assert.AreEqual(one.Id, two.Id);
            }

            if (one.Callback != null || two.Callback != null) 
            {
                Assert.IsNotNull(one.Callback);
                Assert.IsNotNull(two.Callback);
                Assert.AreEqual(one.Callback.Url, two.Callback.Url);
                Assert.AreEqual(one.Callback.Kind, two.Callback.Kind);
            }

            if (one.Filter!= null || two.Filter!= null)
            {
                Assert.IsNotNull(one.Filter);
                Assert.IsNotNull(two.Filter);
                Assert.AreEqual(one.Filter, two.Filter);
            }

            if (includeTarget)
            {
                Assert.AreEqual(one.TargetId, two.TargetId);
                Assert.AreEqual(one.TargetKind, two.TargetKind);
            }
        }

        #endregion
    }
}
