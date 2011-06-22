using System;
using MongoDB.Bson.IO;
using TellagoStudios.Hermes.DataAccess;

namespace MongoDB.Bson
{
    public static class MongoDBBsonExtensions
    {
        public static BsonValue ToBsonValue(this string valueInJsonFormat)
        {
            if (string.IsNullOrWhiteSpace(valueInJsonFormat))
            {
                return BsonNull.Value;
            }

            try
            {
                using (var reader = BsonReader.Create(valueInJsonFormat))
                {
                    return BsonValue.ReadFrom(reader);
                }
            }
            catch (Exception e)
            {
                string text = String.Format(Messages.InvalidJsonValue, valueInJsonFormat ?? "is null");
                throw new ArgumentException(text, "valueInJsonFormat", e);
            }
        }
    }
}