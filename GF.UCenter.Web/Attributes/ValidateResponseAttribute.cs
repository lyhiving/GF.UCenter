using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using GF.UCenter.Common.Portable;
using GF.UCenter.CouchBase;
using NLog;

namespace GF.UCenter.Web
{
    public class ValidateResponseAttribute : ActionFilterAttribute
    {
        protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                logger.Error(context.Exception, $"Execute request exception: url: {context.Request.RequestUri}", context.ActionContext.ActionArguments);
                UCenterError error = new UCenterError();
                if (context.Exception is UCenterException)
                {
                    error.ErrorCode = (context.Exception as UCenterException).ErrorCode;
                }
                else if (context.Exception is CouchBaseException)
                {
                    error.ErrorCode = UCenterErrorCode.CouchBaseError;
                    error.Message = context.Exception.Message;
                }
                else
                {
                    error.ErrorCode = UCenterErrorCode.Failed;
                }

                error.Message = context.Exception.Message;

                var content = new UCenterResponse<UCenterError>()
                {
                    Status = UCenterResponseStatus.Error,
                    Error = error
                };

                var response = new HttpResponseMessage(HttpStatusCode.OK);

                response.Content = new ObjectContent<UCenterResponse<UCenterError>>(content, new JsonMediaTypeFormatter());
                context.Response = response;
            }

            base.OnActionExecuted(context);
        }
    }
}