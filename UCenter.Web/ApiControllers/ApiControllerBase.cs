using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Http;
using UCenter.Common.Database.Couch;
using UCenter.Common.Models;

namespace UCenter.Web.ApiControllers
{
    [Export]
    public class ApiControllerBase : ApiController
    {
        protected readonly CouchBaseContext db;

        [ImportingConstructor]
        public ApiControllerBase(CouchBaseContext db)
        {
            this.db = db;
        }

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

        protected IHttpActionResult CreateSuccessResult(object result)
        {
            return Ok(new
            {
                status = "success",
                result = result
            });
        }

        protected IHttpActionResult CreateErrorResult(UCenterResult code, string message)
        {
            return Ok(new
            {
                status = "error",
                error = new
                {
                    code = code,
                    message = message
                }
            });
        }
    }
}