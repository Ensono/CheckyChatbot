using System;
using System.Net.Http;
using System.Text;
using Checky.Common.Datastore.Test;

namespace Checky.Common.Smokebot {
    public class HttpTestRequestMessageFactory : IHttpTestRequestMessageFactory {
        public HttpRequestMessage CreateMessage(Uri baseUrl, HttpTestDocument test) {
            var message = new HttpRequestMessage {
                Method = new HttpMethod(test.HttpRequestMethod)
            };
            var testUri = new UriBuilder(baseUrl) {
                Path = test.HttpRequestResource
            };
            var data = Convert.FromBase64String(test.HttpRequestBody);
            var requestBody = Encoding.UTF8.GetString(data);
            message.RequestUri = testUri.Uri;
            message.Content = new StringContent(requestBody, Encoding.GetEncoding(test.HttpRequestEncoding),
                test.HttpRequestContentType);
            foreach (var header in test.HttpRequestHeaders) {
                message.Headers.Add(header.Key, header.Value);
            }

            return message;
        }
    }
}