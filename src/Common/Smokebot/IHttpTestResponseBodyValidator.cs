using System;
using Datastore.Test;
using Newtonsoft.Json.Linq;

namespace Smokebot {
    public interface IHttpTestResponseBodyValidator {
        bool ValidateHttpResponseBody(HttpTestDocument test, JObject body, Action<string> callback);
    }
}