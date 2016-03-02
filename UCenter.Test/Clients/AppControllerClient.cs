using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using UCenter.Common.Models;

namespace UCenter.Test
{
    [Export]
    public class AppControllerClient : TenantEnvironment
    {
        [ImportingConstructor]
        public AppControllerClient(WebContext context)
            : base(context)
        {

        }

        private string GenerateApiEndpoint(string controller, string route, string queryString = null)
        {
            var url = $"{this.WebContext.BaseAddress}/api/{controller}/{route}";
            if (!string.IsNullOrEmpty(queryString))
            {
                url = $"{url}/{queryString}";
            }

            return url;
        }

        public async Task<AppLoginResponse> LoginAsync(AppLoginRequest request)
        {
            string url = this.GenerateApiEndpoint("app", "login");
            var content = this.BuildContent(request);
            var response = await this.HttpClient.PostAsync(url, content);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<AppLoginResponse>();
        }
    }
}
