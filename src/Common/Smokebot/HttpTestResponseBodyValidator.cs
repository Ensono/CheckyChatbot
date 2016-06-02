using System;
using System.Text.RegularExpressions;
using Checky.Common.Datastore.Test;
using Newtonsoft.Json.Linq;

namespace Checky.Common.Smokebot {
    public class HttpTestResponseBodyValidator : IHttpTestResponseBodyValidator {
        public bool ValidateHttpResponseBody(HttpTestDocument test, JObject body, Action<string> callback) {
            var result = true;
            foreach (var token in test.ExpectHttpResponseBodyTokens) {
                var actualValue = body.SelectToken(token.Path);
                if (actualValue == null) {
                    result = false;
                    callback($"Unable to find `{token.Path}` in the document.");
                }

                var tokenMatch = new Regex(token.ExpectedValue, RegexOptions.Compiled);
                if (actualValue != null && tokenMatch.IsMatch(actualValue.ToString())) {
                    continue;
                }

                result = false;
                callback(
                    $"Looked for value matching `{token.ExpectedValue}` at `{token.Path}`, however encoutered `{actualValue}`");
            }
            return result;
        }
    }
}