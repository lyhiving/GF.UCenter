using System.ComponentModel.Composition;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace GF.UCenter.Test
{
    [Export]
    public class TenantEnvironment
    {
        protected readonly WebContext WebContext;
        protected readonly HttpClient HttpClient;

        [ImportingConstructor]
        public TenantEnvironment(WebContext webContext)
        {
            this.WebContext = webContext;
            this.HttpClient = new HttpClient();
        }
    }
}
