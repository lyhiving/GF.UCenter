using System.ComponentModel.Composition;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace GF.UCenter.Web
{
    [Export]
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Response = context.Request.CreateErrorResponse(
                    HttpStatusCode.BadRequest, context.ModelState);
            }
        }
    }
}
