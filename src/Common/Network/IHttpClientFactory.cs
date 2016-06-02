using System.Net.Http;

namespace Checky.Common.Network {
    public interface IHttpClientFactory {
        HttpClient GetClient();
    }
}