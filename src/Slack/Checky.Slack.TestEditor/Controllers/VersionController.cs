using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace Checky.Slack.TestEditor.Controllers {
    public class VersionController : ApiController {
        public HttpResponseMessage Get(HttpRequestMessage request) {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(executingAssembly.Location);
            return request.CreateResponse(HttpStatusCode.OK, new {
                executingAssembly.GetName().Name,
                Version = executingAssembly.GetName().Version.ToString(),
                fileVersionInfo.FileVersion,
                fileVersionInfo.ProductVersion
        });
        }
    }
}