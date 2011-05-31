// ReSharper disable CheckNamespace
namespace TellagoStudios.Hermes.Business.Model
// ReSharper restore CheckNamespace
{
    public static class ModelExtensions
    {
        public static MessageKey ToMessageKey(this Message from)
        {
            Guard.Instance
                .ArgumentNotNull(() => from, from)
                .ArgumentNotNull(() => from.Id, from.Id);

// ReSharper disable PossibleInvalidOperationException
            return new MessageKey { TopicId = from.TopicId, MessageId = from.Id.Value };
// ReSharper restore PossibleInvalidOperationException
        }
    }
}