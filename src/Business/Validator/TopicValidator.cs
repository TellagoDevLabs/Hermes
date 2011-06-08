using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.Business.Service;

namespace TellagoStudios.Hermes.Business.Validator
{
    public class TopicValidator
    {
        public ITopicRepository TopicRepository { get; set; }

        public void ValidateBeforeCreate(Topic instance)
        {
            var errors = new List<string> ();

            // Name is ot null
            if (string.IsNullOrWhiteSpace(instance.Name))
            {
                errors.Add(Texts.NameMustBeNotNull);
            }

            // Name is unique
            var query = TopicRepository.QueryDuplicatedName(instance);
            if (TopicRepository.ExistsByQuery(query)) 
            {
                errors.Add(string.Format(Texts.TopicNameMustBeUnique, instance.Name));
            }

            // Group is valid TODO
            //if (!GroupService.Exists(instance.GroupId))
            //{
            //    errors.Add(string.Format(Messages.EntityNotFound, typeof(Group).Name, instance.GroupId));
            //}

            // Any error?
            if (errors.Count > 0) throw new ValidationException(errors);
        }

        public void ValidateBeforeDelete(Identity id)
        {
            // Id is valid
            if (!TopicRepository.ExistsById(id))
            {
                throw new EntityNotFoundException(typeof(Topic), id);
            }
        }

        public void ValidateBeforeUpdate(Topic instance)
        {
            var errors = new List<string>();

            // Id is not null and is valid
            if (!instance.Id.HasValue)
            {
                errors.Add(Texts.IdMustNotBeNull);
            }
            else if (!TopicRepository.ExistsById(instance.Id.Value))
            {
                throw new EntityNotFoundException(typeof(Topic), instance.Id.Value);
            }

            // Name is not null and unique
            var query = TopicRepository.QueryDuplicatedName(instance);
            if (string.IsNullOrWhiteSpace(instance.Name))
            {
                errors.Add(string.Format(Texts.NameMustBeNotNull));
            }
            else if (TopicRepository.ExistsByQuery(query))
            {
                errors.Add(string.Format(Texts.TopicNameMustBeUnique, instance.Name));
            }

            // Any error?
            if (errors.Count > 0) throw new ValidationException(errors);
        }
    }
}
