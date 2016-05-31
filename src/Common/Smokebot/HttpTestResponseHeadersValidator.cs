using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using Datastore.Test;

namespace Smokebot {
    public class HttpTestResponseHeadersValidator : IHttpTestResponseHeadersValidator {
        public bool ValidateHttpResponseHeaders(HttpTestDocument test, HttpResponseMessage response,
                                                Action<string> callback) {
            var result = true;
            foreach (var header in test.ExpectHttpResponseHeaders) {
                if (!response.Headers.Contains(header.Key)) {
                    result = false;
                    callback($"Expected {header.Key} to be returned, but it was missing");
                    continue;
                }

                var responseHeader = response.Headers.Single(x => x.Key == header.Key);
                var headerMatch = new Regex(header.Value, RegexOptions.Compiled);

                if (!responseHeader.Value.Any(x => headerMatch.IsMatch(x))) {
                    continue;
                }

                result = false;
                callback(
                    $"Expected `{responseHeader.Key}` to match `{header.Value}` but it was actually `{string.Join(", ", responseHeader.Value)}`");
            }
            return result;
        }
    }
}