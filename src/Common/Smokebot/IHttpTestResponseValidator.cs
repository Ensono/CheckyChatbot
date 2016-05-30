using System;
using System.Net.Http;
using Datastore.Test;

namespace Smokebot {
    public interface IHttpTestResponseValidator {
        bool Validate(HttpTestDocument test, HttpResponseMessage response, Action<string> callback);
    }
}