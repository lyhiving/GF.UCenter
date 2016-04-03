namespace GF.UCenter.Web
{
    using System.ComponentModel.Composition.Hosting;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Attributes;
    using Common.Settings;

    /// <summary>
    ///     The UCenter web application manager
    /// </summary>
    public static class ApplicationManager
    {
        /// <summary>
        ///     Initialize the web application
        /// </summary>
        /// <param name="configuration">The http configuration</param>
        /// <param name="exportProvider">The export provider</param>
        public static void InitializeApplication(HttpConfiguration configuration, ExportProvider exportProvider)
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            configuration.Filters.Add(new ActionExecutionFilterAttribute());
            RegisterMefDepencency(configuration, exportProvider);
            InitializeSettings(exportProvider);
        }

        private static void InitializeSettings(ExportProvider exportProvider)
        {
            SettingsInitializer.Initialize<Settings>(
                exportProvider,
                SettingsDefaultValueProvider<Settings>.Default,
                AppConfigurationValueProvider.Default);
        }

        private static void RegisterMefDepencency(HttpConfiguration configuration, ExportProvider exportProvider)
        {
            var dependency = new MefDependencyResolver(exportProvider);
            configuration.DependencyResolver = dependency;
        }
    }
}