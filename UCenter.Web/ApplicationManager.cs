using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UCenter.Common;
using UCenter.Common.Database.Entities;
using UCenter.Common.Models;

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
            configuration.Filters.Add(new ValidateModelAttribute());
            RegisterMefDepencency(configuration, exportProvider);
            InitializeSettings(exportProvider);
        }

        private static void InitializeSettings(ExportProvider exportProvider)
        {
            SettingsInitializer.Initialize<Settings>(exportProvider, SettingsDefaultValueProvider<Settings>.Default, AppConfigurationValueProvider.Default);
        }

        private static void RegisterMefDepencency(HttpConfiguration configuration, ExportProvider exportProvider)
        {
            MefDependencyResolver dependency = new MefDependencyResolver(exportProvider);
            configuration.DependencyResolver = dependency;
        }
    }
}