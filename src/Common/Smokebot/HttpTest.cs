using System.Collections.Generic;

namespace Smokebot {
    public class HttpTest {
        public string Id { get; set; }
        public string[] ServiceFilter { get; set; }
        public string[] EnvironmentFilter { get; set; }
        public string HttpRequestMethod { get; set; }
        public string HttpRequestResource { get; set; }
        public string HttpRequestBody { get; set; }
        public string HttpRequestEncoding { get; set; }
        public string HttpRequestContentType { get; set; }
        public Dictionary<string, string> HttpRequestHeaders { get; set; }
        public int ExpectHttpResponseCode { get; set; }
        public Dictionary<string, string> ExpectHttpResponseHeaders { get; set; }
        public IEnumerable<ExpectToken> ExpectHttpResponseBodyTokens { get; set; }
    }
}