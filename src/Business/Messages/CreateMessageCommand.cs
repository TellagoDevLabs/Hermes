using System;
using TellagoStudios.Hermes.Business.Events;
using TellagoStudios.Hermes.Business.Exceptions;
using TellagoStudios.Hermes.Business.Model;
using TellagoStudios.Hermes.Business.Data.Commads;
using TellagoStudios.Hermes.Business.Data.Queries;

namespace TellagoStudios.Hermes.Business.Messages    
{
    public class CreateMessageCommand : ICreateMessageCommand
    {
        private readonly IEntityById entityById;
        private readonly IMessageRepository repository;
        private readonly IEventAggregator eventAggregator;

        public CreateMessageCommand(
            IEntityById entityById,
            IMessageRepository repository,
            IEventAggregator eventAggregator)
        {
            this.entityById = entityById;
            this.repository = repository;
            this.eventAggregator = eventAggregator;
        }

        public virtual void Execute(Message instance)
        {

            if (instance.UtcReceivedOn == default(DateTime)) throw new ValidationException(Texts.ReceivedOnMustBeSetted);
            if (!entityById.Exist<Topic>(instance.TopicId)) throw new EntityNotFoundException(typeof(Topic), instance.TopicId);

            repository.MakePersistent(instance);
            
            eventAggregator.Raise(new NewMessageEvent {Message = instance});
        }
    }
}