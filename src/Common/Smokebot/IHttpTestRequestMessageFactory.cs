using System;
using System.Net.Http;
using Checky.Common.Datastore.Test;

namespace Checky.Common.Smokebot {
    public interface IHttpTestRequestMessageFactory {
        HttpRequestMessage CreateMessage(Uri baseUrl, HttpTestDocument test);
    }
}