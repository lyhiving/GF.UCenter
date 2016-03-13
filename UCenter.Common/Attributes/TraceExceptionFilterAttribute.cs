using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using UCenter.Common.Utils;

namespace UCenter.Common.Attributes
{
    public class TraceExceptionFilterAttribute : ExceptionFilterAttribute, ITraceIdentifier
    {
        public TraceExceptionFilterAttribute(string traceIdentifier)
        {
            TraceIdentifier = traceIdentifier;
        }

        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;
            var request = actionExecutedContext.Request;
            string id = Guid.NewGuid().ToString();
            Logger.TraceError(this,
                "Error Occurred,ErrorId:{0}\n,Method:{1},\nUri:{2},\nContent:{3},\nException:{4}",
                id,
                request.Method,
                request.RequestUri,
                request.Content.ReadAsStringAsync().Result,
                exception);

            var response = new
            {
                status = "error",
                error = new
                {
                    code = 9999,
                    message = exception.Message
                }
            };

            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, response, "application/json");
        }

        public string TraceIdentifier { get; private set; }
    }
}