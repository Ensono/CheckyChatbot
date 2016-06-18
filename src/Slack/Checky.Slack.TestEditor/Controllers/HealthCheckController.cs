using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Checky.Slack.TestEditor.Controllers {
    public class HealthCheckController : ApiController {
        public HttpResponseMessage Get(HttpRequestMessage request) {
            return request.CreateResponse(HttpStatusCode.OK);
        }
    }
}