using TellagoStudios.Hermes.Business;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.RestService.Extensions
{
    static public class TopicExtensions
    {
        static public Topic ToModel(this Facade.TopicPost from)
        {
            if (from == null) return null;

            return new Topic
            {
                Name = from.Name,
                Description = from.Description,
                GroupId = from.GroupId.ToModel(),
            };
        }

        static public Topic ToModel(this Facade.TopicPut from)
        {
            if (from == null) return null;

            return new Topic
            {
                Id = from.Id.ToModel(),
                Name = from.Name,
                Description = from.Description,
                GroupId = from.GroupId.ToModel()
            };
        }

        static public Facade.Topic ToFacade(this Topic from)
        {
            if (from == null) return null;
            Guard.Instance.ArgumentNotNull(() => from.Id, from.Id);

            return new Facade.Topic
            {
                Id = from.Id.Value.ToFacade(),
                Name = from.Name,
                Description = from.Description,
                Group = from.GroupId.ToLink(Business.Constants.Relationships.Group)
            };
        }
    }
}