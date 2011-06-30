using TellagoStudios.Hermes.RestService.Resources;
using Model = TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.RestService.Extensions
{
    public static class MessageExtensions
    {
        static public Facade.Link ToLink(this Model.Message from)
        {
            if (from == null) return null;
            return new Facade.Link(ResourceLocation.OfMessageByTopic(from).ToString(), Constants.Relationships.Message);
        }

        static public Facade.Link ToLink(this Model.MessageKey from)
        {
            if (from == null) return null;

            return new Facade.Link(ResourceLocation.OfMessageByTopic(from).ToString(), Constants.Relationships.Message);
        }
    }
}
