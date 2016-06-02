using System;
using System.Net.Http;
using Checky.Common.Datastore.Test;

namespace Checky.Common.Smokebot {
    public interface IHttpTestResponseHeadersValidator {
        bool ValidateHttpResponseHeaders(HttpTestDocument test, HttpResponseMessage response, Action<string> callback);
    }
}