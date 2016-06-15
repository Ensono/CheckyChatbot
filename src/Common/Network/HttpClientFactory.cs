using System;
using System.Net.Http;
using System.Net.Security;

namespace Checky.Common.Network {
    public class HttpClientFactory : IHttpClientFactory {
        public HttpClient GetClient(string trustInvalidCertificatesWithSubject = null) {
            if (string.IsNullOrEmpty(trustInvalidCertificatesWithSubject)) {
                return new HttpClient();
            }

            var handler = new WebRequestHandler();
            handler.ServerCertificateValidationCallback += OnValidation(trustInvalidCertificatesWithSubject);

            return new HttpClient(handler);
        }

        private static RemoteCertificateValidationCallback OnValidation(string certificateSubject) {
            return
                (sender, certificate, chain, errors) =>
                    certificate.Subject.Equals(certificateSubject, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}