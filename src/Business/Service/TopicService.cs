using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Validator;
using TellagoStudios.Hermes.Business.Service;

namespace TellagoStudios.Hermes.Business.Service
{
    public class TopicService : ITopicService
    {
        public ITopicRepository Repository { get; set; }
        public TopicValidator Validator { get; set; }

        public Topic Create(Topic topic)
        {
            Guard.Instance.ArgumentNotNull(()=>topic, topic);

            Validator.ValidateBeforeCreate(topic);
            return Repository.Create(topic);
        }

        public Topic Get(Identity id)
        {
            return Repository.Get(id);
        }

        public Topic Update(Topic topic)
        {
            Validator.ValidateBeforeUpdate(topic);
            return Repository.Update(topic);
        }

        public void Delete(Identity id)
        {
            Validator.ValidateBeforeDelete(id);
            Repository.Delete(id);
        }

        public IEnumerable<Topic> Find(string query, int? skip = null, int? limit = null)
        {
            Guard.Instance
                .ArgumentValid(()=>skip, () => (skip.HasValue && skip.Value < 0))
                .ArgumentValid(()=>limit, () => (limit.HasValue && limit.Value <= 0));

            return Repository.Find(query, skip, limit);
        }

        public IEnumerable<Identity> GetTopicIdsInGroup(Identity groupId)
        {
            return Repository.GetTopicIdsInGroup(groupId);
        }

        public bool Exists(Identity id)
        {
            return Repository.ExistsById(id);
        }

        public bool ExistsByGroup(Identity groupId)
        {
            var query = Repository.QueryGetByGroup(groupId);
            return Repository.ExistsByQuery(query);
        }


        public IEnumerable<Topic> GetByGroup(Identity groupId, int? skip = null, int? limit = null)
        {
            var query = Repository.QueryGetByGroup(groupId);
            return Repository.Find(query, skip, limit);
        }
    }
}