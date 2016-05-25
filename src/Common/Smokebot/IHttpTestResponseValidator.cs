using System;
using System.Net.Http;

namespace Smokebot {
    public interface IHttpTestResponseValidator {
        bool Validate(HttpTest test, HttpResponseMessage response, Action<string> callback);
    }
}