namespace GF.UCenter.Test
{
    using System;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Web.Http;
    using System.Web.Http.SelfHost;
    using UCenter.Common;
    using Web;

    [Export]
    public class WebContext : DisposableObjectSlim
    {
        private readonly HttpSelfHostConfiguration configuration;
        private readonly HttpSelfHostServer server;
        private readonly Settings settings;

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

        public string BaseAddress { get; }

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