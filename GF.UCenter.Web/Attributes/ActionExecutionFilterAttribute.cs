using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using GF.UCenter.Common.Portable;
using GF.UCenter.CouchBase;
using Newtonsoft.Json.Linq;
using NLog;
using GF.UCenter.Common;

namespace GF.UCenter.Web
{
    public sealed class ActionExecutionFilterAttribute : ActionFilterAttribute
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public override async Task OnActionExecutingAsync(HttpActionContext context, CancellationToken token)
        {
            await this.LogIncomingRequest(context, token);

            if (!context.ModelState.IsValid)
            {
                string errorMessage = context.ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .JoinToString("\n", e => e);

                context.Response = this.CreateErrorResponseMessage(UCenterErrorCode.HttpClientError, errorMessage);
            }

            await base.OnActionExecutingAsync(context, token);
        }

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                logger.Error(context.Exception, $"Execute request exception: url: {context.Request.RequestUri}", context.ActionContext.ActionArguments);

                UCenterErrorCode errorCode = UCenterErrorCode.Failed;
                string errorMessage = null;

                if (context.Exception is UCenterException)
                {
                    errorCode = (context.Exception as UCenterException).ErrorCode;
                }
                else if (context.Exception is CouchBaseException)
                {
                    errorCode = UCenterErrorCode.CouchBaseError;
                }
                else
                {
                    errorCode = UCenterErrorCode.Failed;
                }

                errorMessage = context.Exception.Message;
                context.Response = this.CreateErrorResponseMessage(errorCode, errorMessage);
            }

            base.OnActionExecuted(context);
        }

        private async Task LogIncomingRequest(HttpActionContext context, CancellationToken token)
        {
            try
            {
                var body = await context.Request.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(body))
                {
                    JToken jtoken = JToken.Parse(body);
                    jtoken.Where(j => j is JProperty)
                        .Select(j => j as JProperty)
                        .Where(j => j.Name.Contains("Password"))
                        .ToList()
                        .ForEach(j => j.Value = $"<--{j.Name}-->");

                    body = jtoken.ToString();
                }

                string message = $"Incoming Request\n\t{context.Request.ToString()}\n\t BODY:{body}";
                logger.Info(message);
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Log Incoming Request Error: \n\t{context.Request.ToString()}");
            }
        }

        private HttpResponseMessage CreateErrorResponseMessage(UCenterErrorCode errorCode, string errorMessage)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var content = new UCenterResponse<UCenterError>()
            {
                Status = UCenterResponseStatus.Error,
                Error = new UCenterError() { ErrorCode = errorCode, Message = errorMessage }
            };

            response.Content = new ObjectContent<UCenterResponse<UCenterError>>(content, new JsonMediaTypeFormatter());

            return response;
        }
    }
}