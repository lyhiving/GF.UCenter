/// <summary>
/// UCenter base api controller
/// </summary>
namespace GF.UCenter.Web
{
    using System.ComponentModel.Composition;
    using System.Web.Http;
    using Attributes;
    using CouchBase;
    using NLog;
    using UCenter.Common.Portable;

    /// <summary>
    ///     API controller base class
    /// </summary>
    [Export]
    [ActionExecutionFilter]
    public class ApiControllerBase : ApiController
    {
        /// <summary>
        ///     Couch database context
        /// </summary>
        protected readonly CouchBaseContext DatabaseContext;

        /// <summary>
        ///     The logger
        /// </summary>
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ApiControllerBase" /> class.
        /// </summary>
        /// <param name="db">The couch base context</param>
        [ImportingConstructor]
        public ApiControllerBase(CouchBaseContext db)
        {
            this.DatabaseContext = db;
        }

        /// <summary>
        ///     Create success result
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="result">The content of the result</param>
        /// <returns>Http Action result</returns>
        protected IHttpActionResult CreateSuccessResult<TResult>(TResult result)
        {
            return this.Ok(new UCenterResponse<TResult> { Status = UCenterResponseStatus.Success, Result = result });
        }
    }
}