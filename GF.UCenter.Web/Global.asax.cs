using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UCenter.Common;

namespace UCenter.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            ExportProvider exportProvider = CompositionContainerFactory.Create();

            ApplicationManager.InitializeApplication(GlobalConfiguration.Configuration, exportProvider);

            // The following controller factory is used for MVC controllers.
            SettingsInitializer.Initialize<Settings>(exportProvider, SettingsDefaultValueProvider<Settings>.Default, AppConfigurationValueProvider.Default);
            var mefControllerFactory = exportProvider.GetExportedValue<MEFControllerFactory>();
            ControllerBuilder.Current.SetControllerFactory(mefControllerFactory);
        }
    }
}
