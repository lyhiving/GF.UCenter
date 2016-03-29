using System.ComponentModel.Composition.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using GF.UCenter.Common;

namespace GF.UCenter.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            ExportProvider exportProvider = CompositionContainerFactory.Create();

            ApplicationManager.InitializeApplication(GlobalConfiguration.Configuration, exportProvider);
            SettingsInitializer.Initialize<Settings>(exportProvider, SettingsDefaultValueProvider<Settings>.Default, AppConfigurationValueProvider.Default);
        }
    }
}
