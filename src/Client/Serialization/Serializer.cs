using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TellagoStudios.Hermes.Client.Util;

namespace TellagoStudios.Hermes.Client.Serialization
{
    class Serializer
    {
        static public readonly Serializer Instance = new Serializer();

        private readonly Dictionary<string, ISerializer> byName = new Dictionary<string, ISerializer>(StringComparer.CurrentCultureIgnoreCase);
        private readonly List<Tuple<Regex, ISerializer>> byRegex = new List<Tuple<Regex, ISerializer>>();

        private Serializer()
        {
            Add(new XmlSerializer(), new Regex(@"^\w+/xml.*"));
            Add(new JsonSerializer(), new Regex(@"^\w+/json.*"));
            Add(new TextSerializer(), new Regex(@"^\w+/(text|plain).*"));
        }

        public void Clear()
        {
            byName.Clear();
            byRegex.Clear();
        }

        public IEnumerable<Tuple<string, ISerializer>> SerializersByContentType
        {
            get { return byName.Select(kv => new Tuple<string, ISerializer>(kv.Key, kv.Value)); }
        }

        public IEnumerable<Tuple<Regex, ISerializer>> SerializersByRegEx
        {
            get { return byRegex; }
        }

        public void Add(ISerializer serializer, params string[] contentTypes)
        {
            Guard.Instance
                .ArgumentNotNullOrEmpty(() => contentTypes, contentTypes)
                .ArgumentNotNull(() => serializer, serializer);

            foreach (var contentType in contentTypes)
            {
                if (byName.ContainsKey(contentType))
                {
                    throw new ApplicationException("Already exists a Serializer for Content-Type: " + contentType);
                }
                byName.Add(contentType, serializer);
            }
        }

        public void Add(ISerializer serializer, params Regex[] contentTypes)
        {
            Guard.Instance
                .ArgumentNotNullOrEmpty(() => contentTypes, contentTypes)
                .ArgumentNotNull(() => serializer, serializer);

            foreach (var contentType in contentTypes)
            {
                byRegex.Add(new Tuple<Regex, ISerializer>(contentType, serializer));
            }
        }

        public void Serialize<T>(string contentType, Stream stream, T data)
        {
            Guard.Instance
                .ArgumentNotNullOrWhiteSpace(() => contentType, contentType)
                .ArgumentNotNull(() => stream, stream);

            var serializer = GetSerializer(contentType);
            if (serializer == null) throw new ApplicationException("Can't find a serializer for Content-Type: " + contentType);

            serializer.Serialize(stream, data);
        }

        public T Deserialize<T>(string contentType, Stream stream)
        {
            Guard.Instance
                .ArgumentNotNullOrWhiteSpace(() => contentType, contentType)
                .ArgumentNotNull(() => stream, stream);

            var serializer = GetSerializer(contentType);
            if (serializer == null) throw new ApplicationException("Can't find a serializer for Content-Type: " + contentType);

            return serializer.Deserialize<T>(stream);
        }

        public ISerializer GetSerializer(string contentType)
        {
            ISerializer serializer;
            if (!byName.TryGetValue(contentType, out serializer))
            {
                serializer = byRegex
                    .Where(t => t.Item1.IsMatch(contentType))
                    .Select(t => t.Item2)
                    .FirstOrDefault();
            }
            return serializer;
        }
    }
}