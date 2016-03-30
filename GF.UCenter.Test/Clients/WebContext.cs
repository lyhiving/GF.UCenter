using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.SelfHost;
using GF.UCenter.Common;
using GF.UCenter.Web;
using GF.UCenter.Web.ApiControllers;

namespace GF.UCenter.Test
{
    [Export]
    public class WebContext : DisposableObjectSlim
    {
        private readonly Settings settings;
        private readonly HttpSelfHostServer server;
        private readonly HttpSelfHostConfiguration configuration;

        [ImportingConstructor]
        public WebContext(ExportProvider exportProvider, Settings settings)
        {
            this.settings = settings;
            this.BaseAddress = $"http://{this.settings.ServerHost}:{this.settings.ServerPort}";

            if (UseSelfHost())
            {
                this.configuration = new HttpSelfHostConfiguration(this.BaseAddress);
                this.configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
                this.configuration.MapHttpAttributeRoutes();
                ApplicationManager.InitializeApplication(configuration, exportProvider);

                this.server = new HttpSelfHostServer(configuration);
                this.server.OpenAsync().Wait();
            }
        }

        public string BaseAddress { get; private set; }

        private bool UseSelfHost()
        {
            return BaseAddress.IndexOf("localhost", StringComparison.OrdinalIgnoreCase) >= 0 ||
                BaseAddress.Contains("127.0.0.1");
        }

        protected override void DisposeInternal()
        {
            if (this.configuration != null)
            {
                this.configuration.Dispose();
            }

            if (this.server != null)
            {
                this.server.Dispose();
            }
        }
    }
}
