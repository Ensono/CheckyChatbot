using System.Net.Http;

namespace Checky.Common.Network {
    public class HttpClientFactory : IHttpClientFactory {
        public HttpClient GetClient() {
            return new HttpClient();
        }
    }
}