using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject;
using WebApiContrib.IoC.Ninject;

namespace Checky.Slack.TestEditor {
    public class WebApiApplication : System.Web.HttpApplication {
        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            var config = GlobalConfiguration.Configuration;

            WebApiConfig.Register(config);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            config.EnsureInitialized();
        }

        private static StandardKernel CreateKernel() {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            return kernel;
        }
    }
}
