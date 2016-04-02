namespace GF.UCenter.Web
{
    using System.ComponentModel.Composition.Hosting;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using UCenter.Common;
    using UCenter.Common.Settings;

    /// <summary>
    /// MVC Application
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// The application start event
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            ExportProvider exportProvider = CompositionContainerFactory.Create();

            ApplicationManager.InitializeApplication(GlobalConfiguration.Configuration, exportProvider);
            SettingsInitializer.Initialize<Settings>(
                exportProvider,
                SettingsDefaultValueProvider<Settings>.Default,
                AppConfigurationValueProvider.Default);
        }
    }
}