using System.Net.Http;

namespace Network {
    public interface IHttpClientFactory {
        HttpClient GetClient();
    }
}