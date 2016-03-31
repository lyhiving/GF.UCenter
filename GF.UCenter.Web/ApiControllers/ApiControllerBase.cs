using System.ComponentModel.Composition;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;
using Couchbase;
using GF.UCenter.Common;
using GF.UCenter.Common.Portable;
using GF.UCenter.CouchBase;
using NLog;

namespace GF.UCenter.Web.ApiControllers
{
    [Export]
    [ActionExecutionFilter]
    public class ApiControllerBase : ApiController
    {
        //---------------------------------------------------------------------
        protected readonly CouchBaseContext db;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        //---------------------------------------------------------------------
        [ImportingConstructor]
        public ApiControllerBase(CouchBaseContext db)
        {
            this.db = db;
        }

        //---------------------------------------------------------------------
        protected string GetClientIp(HttpRequestMessage request)
        {
            request = request ?? Request;

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            else if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            else
            {
                return null;
            }
        }

        protected IHttpActionResult CreateSuccessResult<TResult>(TResult result)
        {
            return Ok(new UCenterResponse<TResult>() { Status = UCenterResponseStatus.Success, Result = result });
        }
    }
}
