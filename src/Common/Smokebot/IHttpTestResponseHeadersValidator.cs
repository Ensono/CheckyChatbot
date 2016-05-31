using System;
using System.Net.Http;
using Datastore.Test;

namespace Smokebot {
    public interface IHttpTestResponseHeadersValidator {
        bool ValidateHttpResponseHeaders(HttpTestDocument test, HttpResponseMessage response, Action<string> callback);
    }
}