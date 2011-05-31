using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Validator;
using TellagoStudios.Hermes.Business.Repository;

namespace TellagoStudios.Hermes.Business.Service
{
    public class GroupService : IGroupService
    {
        
        public IGroupRepository Repository { get; set; }
        public GroupValidator Validator { get; set; }

        public Group Create(Group group)
        {
            Guard.Instance.ArgumentNotNull(()=>group, group);            

            Validator.ValidateBeforeCreate(group);
            return Repository.Create(group);
        }

        public Group Get(Identity id)
        {
            return Repository.Get(id);
        }

        public Group Update(Group group)
        {
            Guard.Instance.ArgumentNotNull(()=>group, group);            

            Validator.ValidateBeforeUpdate(group);
            return Repository.Update(group);
        }

        public void Delete(Identity id)
        {
            Validator.ValidateBeforeDelete(id);
            Repository.Delete(id);
        }

        public IEnumerable<Group> Find(string query, int? skip, int? limit)
        {
            Guard.Instance
                .ArgumentValid(()=>skip, () =>(skip.HasValue && skip.Value < 0))
                .ArgumentValid(()=>limit, () => (limit.HasValue && limit.Value <= 0));

            return Repository.Find(query, skip, limit);
        }

        public bool Exists(Identity id)
        {
            return Repository.ExistsById(id);
        }

        public IEnumerable<Group> GetAncestry(Identity groupId)
        {
            var list = new List<Group>();
            var group = Get(groupId);
            GetAncestry(group, list);
            return list.ToArray();
        }

        private void GetAncestry(Group group, IList<Group> list)
        {
            if (group == null)
            {
                return;
            }

            if (group.ParentId.HasValue)
            {
                var parent = Get(group.ParentId.Value);
                GetAncestry(parent, list);
            }
            
            list.Add(group);
        }
    }
}