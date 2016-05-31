using System.Net.Http;

namespace Network {
    public class HttpClientFactory : IHttpClientFactory {
        public HttpClient GetClient() {
            return new HttpClient();
        }
    }
}