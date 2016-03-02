using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace UCenter.Web
{
    internal static class ApplicationManager
    {
        public static void InitializeApplication(HttpConfiguration configuration, ExportProvider exportProvider)
        {
            // UnityConfig.RegisterComponents();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RegisterMefDepencency(configuration, exportProvider);
        }

        private static void RegisterMefDepencency(HttpConfiguration configuration, ExportProvider exportProvider)
        {
            MefDependencyResolver dependency = new MefDependencyResolver(exportProvider);
            configuration.DependencyResolver = dependency;
        }
    }
}