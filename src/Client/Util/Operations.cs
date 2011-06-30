using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.Client.Util
{
    public static class Operations
    {
        public const string Groups = "groups";
        public const string Messages = "messages";
        public const string Subscriptions = "subscriptions";
        
        static public string GetGroup(Identity id)
        {
            return Groups + "/" + id;
        }
    }
}
