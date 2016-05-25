using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace Smokebot {
    public class HttpTestResponseValidator : IHttpTestResponseValidator {
        public bool Validate(HttpTest test, HttpResponseMessage response, Action<string> callback) {
            var result = true;

            if (response.StatusCode != (HttpStatusCode) test.ExpectHttpResponseCode) {
                result = false;
                callback(
                    $"Invalid response code, expected: {test.ExpectHttpResponseCode}, actual: {response.StatusCode}");
            }

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

            var content = response.Content.ReadAsStringAsync();
            var body = JObject.Parse(content.Result);

            foreach (var token in test.ExpectHttpResponseBodyTokens) {
                var actualValue = body.SelectToken(token.Path);
                if (actualValue == null) {
                    result = false;
                    callback($"Unable to find `{token.Path}` in the document.");
                }

                var tokenMatch = new Regex(token.ExpectedValue, RegexOptions.Compiled);
                if (actualValue == null || !tokenMatch.IsMatch(actualValue.ToString())) {
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