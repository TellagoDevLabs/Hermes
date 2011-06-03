using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using TellagoStudios.Hermes.Business.Model;

namespace TellagoStudios.Hermes.Logging
{
    public class IdentitySerializer : BsonBaseSerializer
    {
        private static IdentitySerializer instance = new IdentitySerializer();

        public static IdentitySerializer Instance
        {
            get
            {
                return IdentitySerializer.instance;
            }
        }

        public override object Deserialize(BsonReader bsonReader, Type nominalType, IBsonSerializationOptions options)
        {
            switch (bsonReader.CurrentBsonType)
            {
                case BsonType.ObjectId:
                    int timestamp;
                    int machine;
                    short pid;
                    int increment;
                    bsonReader.ReadObjectId(out timestamp, out machine, out pid, out increment);
                    var id = new ObjectId(timestamp, machine, pid, increment);
                    return new Identity(id.ToByteArray());

                case BsonType.String:
                    return new Identity(bsonReader.ReadString());

                default:
                    throw new FormatException(string.Format("Cannot deserialize Identity from BsonType: {0}", bsonReader.CurrentBsonType));
            }
        }

        public override void Serialize(BsonWriter bsonWriter, Type nominalType, object value, IBsonSerializationOptions options)
        {
            var identity = (Identity)value;
            var objectId = new ObjectId(identity.ToArray());
            BsonType bsonType = options == null ? BsonType.ObjectId : ((RepresentationSerializationOptions)options).Representation;
            switch (bsonType)
            {
                case BsonType.String:
                    bsonWriter.WriteString(objectId.ToString());
                    break;
                case BsonType.ObjectId:
                    bsonWriter.WriteObjectId(objectId.Timestamp, objectId.Machine, objectId.Pid, objectId.Increment);
                    break;
                default:
                    throw new BsonSerializationException(string.Format("'{0}' is not a valid representation for type 'Identity'", bsonType));
            }
        }
    }
}
