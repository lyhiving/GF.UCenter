using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.SelfHost;
using UCenter.Common;
using UCenter.Web;
using UCenter.Web.ApiControllers;

namespace UCenter.Test
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
            this.BaseAddress = $"http://{this.settings.ServerHost}:{this.settings.ServerPort }";

            this.configuration = new HttpSelfHostConfiguration(this.BaseAddress);
            this.configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            this.configuration.Services.Replace(typeof(IAssembliesResolver), new TestAssemblyResolver(typeof(AppController)));
            this.configuration.MapHttpAttributeRoutes();
            ApplicationManager.InitializeApplication(configuration, exportProvider);

            this.server = new HttpSelfHostServer(configuration);
            this.server.OpenAsync().Wait();
        }

        public string BaseAddress { get; private set; }

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

        public class TestAssemblyResolver : IAssembliesResolver
        {
            private readonly Type controllerType;

            public TestAssemblyResolver(Type controllerType)
            {
                this.controllerType = controllerType;
            }

            public ICollection<Assembly> GetAssemblies()
            {
                List<Assembly> baseAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

                if (!baseAssemblies.Contains(controllerType.Assembly))
                    baseAssemblies.Add(controllerType.Assembly);

                return baseAssemblies;
            }
        }
    }
}
