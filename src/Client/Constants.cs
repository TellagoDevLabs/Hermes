namespace TellagoStudios.Hermes.Client
{
    public static class Constants
    {
        #region PrivateHeaders
        public class PrivateHeaders
        {
            public const string Prefix = "HUB_";
            public const string PromotedProperty = Prefix + "PromotedProperty_";
            public const string PromotedProperties = Prefix + "PromotedProperties";
        }
        #endregion

        #region HttpContentHeaders
        public static readonly string[] HttpContentHeaders = new []
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
        #endregion

        #region HttpRequestHeaders
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
        #endregion

        #region HttpResponseHeaders
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
        #endregion

    }
}
