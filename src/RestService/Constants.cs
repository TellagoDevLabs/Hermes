namespace TellagoStudios.Hermes.RestService
{
    public static class Constants
    {
        public const string HermesHostConfigurationObjectName = "HermesHostConfiguration";

        public static class RetryValues
        {
            public const int RetryMax = 3;
            public const int DelaySeconds = 15;
        }

        public class PrivateHeaders
        {
            public const string Prefix = "HUB_";
            public const string PromotedProperty = Prefix + "PromotedProperty_";
            public const string PromotedProperties = Prefix + "PromotedProperties";
        }

        public static class Routes
        {
            public const string Messages = "messages";
            public const string Topics = "topics";
            public const string Groups = "groups";
            public const string Subscriptions = "subscriptions";
            public const string Retries = "retries";
            public const string Log = "log";
        }

        public static class Relationships
        {
            public const string Message = "Message";
            public const string Parent = "Parent";
            public const string Topic = "Topic";
            public const string Group = "Group";
        }


        public static readonly string[] HttpContentHeaders = new[]
                                                                 {
                                                                    "Allow",
                                                                    "Content-Encoding",
                                                                    "Content-Language",
                                                                    "Content-Length",
                                                                    "Content-Location",
                                                                    "Content-MD5",
                                                                    "Content-Range",
                                                                    "Content-Type",
                                                                    "Expires",
                                                                    "Last-Modified"
                                                                 };

        public static readonly string[] HttpRequestHeaders = new[]
                                                                {
                                                                    "Accept",
                                                                    "Accept-Charset",
                                                                    "Accept-Encoding",
                                                                    "Accept-Language",
                                                                    "Authorization",
                                                                    "Expect",
                                                                    "From",
                                                                    "Host",
                                                                    "If-Match",
                                                                    "If-Modified-Since",
                                                                    "If-None-Match",
                                                                    "If-Range",
                                                                    "If-Unmodified-Since",
                                                                    "Max-Forwards",
                                                                    "Proxy-Authorization",
                                                                    "Range",
                                                                    "Referer",
                                                                    "TE",
                                                                    "User-Agent"
                                                                };

        public static readonly string[] HttpResponseHeaders = new[]
                                                                {
                                                                    "Accept-Ranges",
                                                                    "Age", 
                                                                    "ETag", 
                                                                    "Location", 
                                                                    "Proxy-Authenticate", 
                                                                    "Retry-After", 
                                                                    "Server", 
                                                                    "Vary",
                                                                    "WWW-Authenticate"
                                                                };

        public static readonly string[] HttpHeadersToIgnoreOnPush = new[]
                                                                {
                                                                    "Host",
                                                                    "Expect"
                                                                };
    }
}