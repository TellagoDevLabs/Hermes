using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Repository;
using System.Linq;
using TellagoStudios.Hermes.Business.Service;

namespace TellagoStudios.Hermes.Business.Validator
{
    public class GroupValidator 
    {
        public IGroupRepository Repository { get; set; }
        public ITopicService TopicService { get; set; }
        
        public void ValidateBeforeDelete(Identity groupId)
        {
            Validate(
                ValidateId(groupId),
                ValidateGroupIsEmpty(groupId)
                );
        }
        
        private static void Validate(params IEnumerable<string>[] validationResults)
        {
            var errors = validationResults.SelectMany(v => v);

            if (errors.Count() > 0) throw new ValidationException(errors);
        }
        
        private IEnumerable<string> ValidateId(Identity? id)
        {
            var errors = new List<string>();

            if (!id.HasValue)
            {
                errors.Add(Messages.IdMustNotBeNull);
            }
            else if (!Repository.ExistsById(id.Value))
            {
                throw new EntityNotFoundException(typeof(Group), id.Value);
            }

            return errors;
        }

        private IEnumerable<string> ValidateGroupIsEmpty(Identity groupId)
        {
            var errors = new List<string>();

            // Validate that group doesn't have any topic););
            if (TopicService.ExistsByGroup(groupId))
            {
                errors.Add(string.Format(Messages.GroupContainsChildTopics, groupId));
            }

            // Validate that group doesn't have any sub-group
            var query = Repository.QueryGetChildGroups(groupId);
            if (Repository.ExistsByQuery(query))
            {
                errors.Add(string.Format(Messages.GroupContainsChildGroups, groupId));
            }

            return errors;
        }

    }
}