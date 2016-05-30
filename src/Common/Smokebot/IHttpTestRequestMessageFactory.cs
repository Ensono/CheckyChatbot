using System;
using System.Net.Http;
using Datastore.Test;

namespace Smokebot {
    public interface IHttpTestRequestMessageFactory {
        HttpRequestMessage CreateMessage(Uri baseUrl, HttpTestDocument test);
    }
}