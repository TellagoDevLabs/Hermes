using System;
using System.Text;
using TellagoStudios.Hermes.Facade;

namespace TellagoStudios.Hermes.Client.Util
{
    public static class Operations
    {
        public const string Topic = "topic";
        public const string Topics = "topics";
        public const string Group = "group";
        public const string Groups = "groups";
        public const string Messages = "messages";
        public const string Subscriptions = "subscriptions";

        static public string GetGroup(Identity id)
        {
            return Group + "/" + id;
        }

        static public string GetGroup(string name)
        {
            return Group + "/?name=" + name;
        }

        static public string GetTopic(Identity id)
        {
            return Topic + "/" + id;
        }

        static public string GetTopic(string name, Identity? groupId = null)
        {
            return groupId.HasValue ?
                Topic + "/?name=" + name + "&groupId=" + groupId.Value :
                Topic + "/?name=" + name;
        }

        static public string GetMessagesByTopic(Identity id, Uri last, int? skip = null, int? limit = null)
        {
            var lastId = new Identity?();

            if (last != null)
            {
                var length = last.Segments.Length;
                if (length < 5 ||
                    last.Segments[length - 2] != "topic/" ||
                    last.Segments[length - 4] != Operations.Messages + "/")
                {
                    throw new ApplicationException("Invalid URL format");
                }

                var LastStr = last.Segments[length - 3];

                lastId = new Identity(LastStr.Substring(0, LastStr.Length-1));
            }

            return GetMessagesByTopic(id, lastId, skip, limit);
        }

        static public string GetMessagesByTopic(Identity id, Identity? last = null, int? skip = null, int? limit = null)
        {
            var first = true;

            var sb = new StringBuilder(Messages);
            sb.Append("/topic/");
            sb.Append(id);
            sb.AppendParameter(last, "last", ref first);
            sb.AppendParameter(skip, "skip", ref first);
            sb.AppendParameter(limit, "limit", ref first);

            return sb.ToString();
        }

        static private void AppendParameter<T>(this StringBuilder sb, T? instance, string name, ref bool first)
        where T : struct
        {
            if (!instance.HasValue) return;

            sb.Append(first ? '?' : '&');
            first = false;
            sb.Append(name);
            sb.Append('=');
            sb.Append(instance.Value);
        }
    }
}