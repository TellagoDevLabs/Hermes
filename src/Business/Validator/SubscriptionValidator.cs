using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Repository;
using TellagoStudios.Hermes.Business.Service;

namespace TellagoStudios.Hermes.Business.Validator
{
    public class SubscriptionValidator
    {
        public ISubscriptionRepository Repository { get; set; }
        public ITopicService TopicService { get; set; }

        public void ValidateBeforeGetByTopic(Identity topicId)
        {
            if (!TopicService.Exists(topicId))
            {
                throw new ValidationException(string.Format(Texts.EntityNotFound, typeof(Topic).Name, topicId));
            }
        }


        public void ValidateBeforeCreate(Subscription instance)
        {
            var errors = new List<string>();

            // Validates filter
            if (instance.Filter != null && !Repository.IsQueryValid(instance.Filter))
            {
                errors.Add(string.Format(Texts.InvalidFilter, instance.Filter));
            }

            // TopicId is valid
            if (!instance.TargetId.HasValue)
            {
                errors.Add(string.Format(Texts.TargetIdMustNotBeNull));
            }
            else
            {
                switch (instance.TargetKind)
                {
                    case TargetKind.Topic:
                        if (!TopicService.Exists(instance.TargetId.Value))
                        {
                            errors.Add(string.Format(Texts.EntityNotFound, typeof (Topic).Name, instance.TargetId));
                        }
                        break;
                    case TargetKind.Group:
                        //TODO
                        //if (!GroupService.Exists(instance.TargetId.Value))
                        //{
                        //    errors.Add(string.Format(Messages.EntityNotFound, typeof (Group).Name,
                        //                             instance.TargetId));
                        //}
                        break;
                    default:
                        errors.Add(string.Format(Texts.TargetKindUnknown, instance.TargetKind));
                        break;
                }
            }

            // Any error?
            if (errors.Count > 0) throw new ValidationException(errors);
        }

        public void ValidateBeforeUpdate(Subscription instance)
        {
            var errors = new List<string>();

            // Id is not null and is valid
            if (!instance.Id.HasValue)
            {
                errors.Add(Texts.IdMustNotBeNull);
            }
            else if (!Repository.ExistsById(instance.Id.Value))
            {
                throw new EntityNotFoundException(typeof(Subscription), instance.Id.Value);
            }

            // Validates filter
            if (instance.Filter!=null && !Repository.IsQueryValid(instance.Filter))
            {
                throw new ValidationException(string.Format(Texts.InvalidFilter, instance.Filter));
            }

            // Any error?
            if (errors.Count > 0) throw new ValidationException(errors);
        }

        public void ValidateBeforeDelete(Identity id)
        {
            // Id is valid
            if (!Repository.ExistsById(id))
            {
                throw new EntityNotFoundException(typeof(Subscription), id);
            }
        }
    }
}