using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Portable;

namespace UCenter.Test
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

        protected ObjectContent<T> BuildContent<T>(T data)
        {
            return new ObjectContent<T>(data, new JsonMediaTypeFormatter());
        }
    }
}
