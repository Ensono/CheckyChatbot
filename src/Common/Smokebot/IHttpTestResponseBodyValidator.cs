using System;
using Checky.Common.Datastore.Test;
using Newtonsoft.Json.Linq;

namespace Checky.Common.Smokebot {
    public interface IHttpTestResponseBodyValidator {
        bool ValidateHttpResponseBody(HttpTestDocument test, JObject body, Action<string> callback);
    }
}