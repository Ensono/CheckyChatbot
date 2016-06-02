using System;
using System.Net.Http;
using Checky.Common.Datastore.Test;

namespace Checky.Common.Smokebot {
    public interface IHttpTestResponseValidator {
        bool Validate(HttpTestDocument test, HttpResponseMessage response, Action<string> callback);
    }
}