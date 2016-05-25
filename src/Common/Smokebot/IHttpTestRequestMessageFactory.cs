using System;
using System.Net.Http;

namespace Smokebot {
    public interface IHttpTestRequestMessageFactory {
        HttpRequestMessage CreateMessage(Uri baseUrl, HttpTest test);
    }
}