using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.DataAccess.MongoDB
{
    public class IdentityGenerator : IIdGenerator
    {
        private static readonly IdentityGenerator instance = new IdentityGenerator();

        public static IdentityGenerator Instance
        {
            get
            {
                return instance;
            }
        }

        public object GenerateId()
        {
            return new Identity(ObjectId.GenerateNewId().ToByteArray());
        }

        public bool IsEmpty(object id)
        {
            return id == null || (Identity)id == Identity.Empty;
        }
    }
}
