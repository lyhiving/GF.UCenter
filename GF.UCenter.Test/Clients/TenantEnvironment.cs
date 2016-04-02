namespace GF.UCenter.Test.Clients
{
    using System.ComponentModel.Composition;
    using System.Net.Http;

    [Export]
    public class TenantEnvironment
    {
        protected readonly HttpClient HttpClient;
        protected readonly WebContext WebContext;

        [ImportingConstructor]
        public TenantEnvironment(WebContext webContext)
        {
            this.WebContext = webContext;
            this.HttpClient = new HttpClient();
        }
    }
}