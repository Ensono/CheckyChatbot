using Checky.Slack.TestEditor;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Checky.Slack.TestEditor {
    public class Startup {
        public void Configuration(IAppBuilder app) {}
    }
}