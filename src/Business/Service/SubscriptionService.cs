using System;
using System.Collections.Generic;
using System.Linq;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Validator;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.Business.Service;

namespace TellagoStudios.Hermes.Business.Service
{
    public class SubscriptionService : ISubscriptionService
    {
        public ISubscriptionRepository Repository { get; set; }
        public SubscriptionValidator Validator { get; set; }
        public ITopicService TopicService { get; set; }
        public IGroupService GroupService { get; set; }

        public Subscription Create(Subscription subscription)
        {
            Guard.Instance.ArgumentNotNull(()=>subscription, subscription);            

            Validator.ValidateBeforeCreate(subscription);
            return Repository.Create(subscription);
        }

        public Subscription Get(Identity id)
        {
            return Repository.Get(id);
        }

        public Subscription Update(Subscription subscription)
        {
            Guard.Instance.ArgumentNotNull(()=>subscription, subscription);            

            Validator.ValidateBeforeUpdate(subscription);
            return Repository.Update(subscription);
        }

        public void Delete(Identity id)
        {
            Repository.Delete(id);
        }

        public bool ExistsById(Identity id)
        {
            return Repository.ExistsById(id);
        }

        public IEnumerable<Subscription> Find(string query, int? skip = null, int? limit = null)
        {
            Guard.Instance
                .ArgumentValid(()=>skip, () => (skip.HasValue && skip.Value < 0))
                .ArgumentValid(()=>limit, () => (limit.HasValue && limit.Value <= 0));

            return Repository.Find(query, skip, limit);
        }

        public IEnumerable<Subscription> GetByGroup(Identity groupId)
        {
            return Repository.ForGroup(groupId);
        }

        public IEnumerable<Subscription> GetByTopic(Identity topicId)
        {
            return Repository.ForTopic(topicId);
        }

        public IEnumerable<Subscription> GetByTopicAndTopicsGroups(Identity topicId)
        {
            var topic = TopicService.Get(topicId);
            var subscriptions = Repository.ForTopic(topicId).ToList();

            var groups = GroupService.GetAncestry(topic.GroupId);
            subscriptions.AddRange(groups.SelectMany(g => Repository.ForGroup(g.Id.Value)));

            return subscriptions.Distinct(new SubscriptionComparer());
        }

        class SubscriptionComparer : IEqualityComparer<Subscription>
        {
            public bool Equals(Subscription x, Subscription y)
            {
                return x.Id == y.Id;
            }

            public int GetHashCode(Subscription obj)
            {
                return obj.Id.GetHashCode();
            }
        }
    }
}