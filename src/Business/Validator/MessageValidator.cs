using System;
using System.Collections.Generic;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Service;

namespace TellagoStudios.Hermes.Business.Validator
{
    public class MessageValidator
    {
        public ITopicService TopicService { get; set; }
        //public ISubscriptionService SubscriptionService { get; set; }

        public void ValidateSubsriptionExists(Identity id)
        {
            var errors = new List<string>();

            //if (!SubscriptionService.ExistsById(id))
            //{
            //    errors.Add(string.Format(Texts.EntityNotFound, typeof(Subscription).Name, id));
            //}

            // Any error?
            if (errors.Count > 0) throw new ValidationException(errors);
        }

        public void ValidateBeforeCreate(Message instance)
        {
            var errors = new List<string> ();

            // ReceivedOn is not null and it is valid
            if (instance.UtcReceivedOn == default(DateTime))
            {
                errors.Add(Texts.ReceivedOnMustBeSetted);
            }
            
            if (!TopicService.Exists(instance.TopicId))
            {
                errors.Add(string.Format(Texts.EntityNotFound, typeof(Topic).Name, instance.TopicId));
            }

            // Any error?
            if (errors.Count > 0) throw new ValidationException(errors);
        }

        public void ValidateBeforeGet(MessageKey key)
        {
            var errors = new List<string>();

            // TopicId is valid
            if (!TopicService.Exists(key.TopicId))
            {
                errors.Add(string.Format(Texts.EntityNotFound, typeof(Topic).Name, key.TopicId));
            }

            // Any error?
            if (errors.Count > 0) throw new ValidationException(errors);
        }

        public void ValidateGroup(Identity groupId)
        {
            var errors = new List<string>();

            // groupId is valid
            //if (!GroupService.Exists(groupId))
            //{
            //    errors.Add(string.Format(Messages.EntityNotFound, typeof(Group).Name, groupId));
            //}

            // Any error?
            if (errors.Count > 0) throw new ValidationException(errors);
        }
    }
}
