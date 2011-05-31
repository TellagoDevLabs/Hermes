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

        public void ValidateBeforeCreate(Group group)
        {
            Validate(
                ValidateName(group),
                ValidateParent(group)
                );
        }

        public void ValidateBeforeDelete(Identity groupId)
        {
            Validate(
                ValidateId(groupId),
                ValidateGroupIsEmpty(groupId)
                );
        }

        public void ValidateBeforeUpdate(Group group)
        {
            Validate(
                ValidateId(group.Id),
                ValidateName(group),
                ValidateNoCircularReferences(group)
                );
        }

        private Identity? VerifyLoopsInTree(Group group, List<Identity> list)
        {
            if (group.ParentId == null) return null;

            if (group.Id.HasValue)
            {
                if (list.Contains(group.Id.Value))
                {
                    return group.Id;
                }
                if (list.Contains(group.ParentId.Value))
                {
                    return group.ParentId;
                }
                list.Add(group.Id.Value);
            }
            var parent = Repository.Get(group.ParentId.Value);
            return VerifyLoopsInTree(parent, list);
        }

        private static void Validate(params IEnumerable<string>[] validationResults)
        {
            var errors = validationResults.SelectMany(v => v);

            if (errors.Count() > 0) throw new ValidationException(errors);
        }

        private IEnumerable<string> ValidateName(Group group)
        {
            var errors = new List<string>();

            var query = Repository.QueryDuplicatedName(group);

            // Name is not null nor unique
            if (string.IsNullOrWhiteSpace(group.Name))
            {
                errors.Add(string.Format(Messages.NameMustBeNotNull));
            }
            else if (Repository.ExistsByQuery(query))
            {
                errors.Add(string.Format(Messages.GroupNameMustBeUnique, group.Name));
            }

            return errors;
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

        private IEnumerable<string> ValidateParent(Group group)
        {
            // If parent is not null, then parent Id is valid);
            if (!group.ParentId.HasValue) return new List<string>();

            var errors = new List<string>();
            if (!Repository.ExistsById(group.ParentId.Value))
            {
                errors.Add(string.Format(Messages.EntityNotFound, typeof(Group).Name, group.ParentId));
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

        private IEnumerable<string> ValidateNoCircularReferences(Group group)
        {
            var errors = new List<string>();

            // Parent will not generate a circular reference
            var circularReferenceGroupId = VerifyLoopsInTree(group, new List<Identity>());
            if (circularReferenceGroupId != null)
            {
                errors.Add(string.Format(Messages.GroupCircleReference, circularReferenceGroupId));
            }

            return errors;
        }
    }
}