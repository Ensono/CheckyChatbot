using System;
using System.Net;
using System.Net.Http;
using Checky.Common.Datastore.Test;
using Newtonsoft.Json.Linq;

namespace Checky.Common.Smokebot {
    public class HttpTestResponseValidator : IHttpTestResponseValidator {
        private readonly HttpTestResponseBodyValidator _responseBodyValidator;
        private readonly HttpTestResponseHeadersValidator _responseHeadersValidator;

        public HttpTestResponseValidator(HttpTestResponseHeadersValidator responseHeadersValidator,
                                         HttpTestResponseBodyValidator responseBodyValidator) {
            _responseHeadersValidator = responseHeadersValidator;
            _responseBodyValidator = responseBodyValidator;
        }

        public bool Validate(HttpTestDocument test, HttpResponseMessage response, Action<string> callback) {
            var result = true;
            var content = response.Content.ReadAsStringAsync();
            var body = JObject.Parse(content.Result);

            if (response.StatusCode != (HttpStatusCode) test.ExpectHttpResponseCode) {
                result = false;
                callback(
                    $"Invalid response code, expected: {test.ExpectHttpResponseCode}, actual: {response.StatusCode}");
            }

            result = result & _responseHeadersValidator.ValidateHttpResponseHeaders(test, response, callback);
            result = result & _responseBodyValidator.ValidateHttpResponseBody(test, body, callback);

            return result;
        }
    }
}