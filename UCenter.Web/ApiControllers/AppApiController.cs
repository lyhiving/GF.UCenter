using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using UCenter.Common.Models;
using System.ComponentModel.Composition;
using System.ServiceModel.Security;
using System.Threading;
using NLog;
using UCenter.Common;
using UCenter.Common.Attributes;
using UCenter.CouchBase.Database;
using UCenter.CouchBase.Entities;

namespace UCenter.Web.ApiControllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/app")]
    [TraceExceptionFilter("AppController")]
    public class AppApiController : ApiControllerBase
    {
        private Logger logger = LogManager.GetCurrentClassLogger();

        [ImportingConstructor]
        public AppApiController(CouchBaseContext db)
            : base(db)
        {
        }

        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> Create([FromBody]AppInfo info, CancellationToken token)
        {
            logger.Info("创建App\nAppId={0}", info.AppId);

            var appEntity = new AppEntity()
            {
                AppId = info.AppId,
                AppSecret = info.AppSecret
            };

            await this.db.Bucket.InsertSlimAsync<AppEntity>(appEntity);
            var response = new AppResponse()
            {
                AppId = info.AppId,
                AppSecret = info.AppSecret
            };
            return CreateSuccessResult(response);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(AppLoginInfo info, CancellationToken token)
        {
            logger.Info("App请求登录\nAppId={0}", info.AppId);

            var app = await this.db.Bucket.FirstOrDefaultAsync<AppEntity>(a => a.AppId == info.AppId && a.AppSecret == info.AppSecret);

            if (app == null)
            {
                return CreateErrorResult(UCenterResult.AppLoginFailedNotExit, "App does not exist.");
            }

            app.Token = EncryptHashManager.GenerateToken();
            await this.db.Bucket.UpsertSlimAsync(app);

            //todo: need login record?
            return CreateSuccessResult(app);
        }

        [HttpPost]
        [Route("verifyaccount")]
        public async Task<IHttpActionResult> AppVerifyAccount(AccountVerificationInfo info, CancellationToken token)
        {
            var result = new AppVerifyAccountResponse();

            var appAuthResult = await AuthApp(info.AppId, info.AppSecret);
            if (appAuthResult == UCenterResult.AppLoginFailedNotExit)
            {
                return CreateErrorResult(UCenterResult.AppLoginFailedNotExit, "App does not exist");

            }
            else if (appAuthResult == UCenterResult.AppLoginFailedSecretError)
            {
                return CreateErrorResult(UCenterResult.AppLoginFailedSecretError, "App secret incorrect");
            }

            var account = await db.Bucket.FirstOrDefaultAsync<AccountEntity>(a => a.AccountName == info.AccountName);
            if (account == null)
            {
                return CreateErrorResult(UCenterResult.AccountLoginFailedNotExist, "Account does not exist");
            }

            result.AccountId = account.Id;
            result.AccountName = account.AccountName;
            result.AccountToken = account.Token;
            result.LastLoginDateTime = account.LastLoginDateTime;
            result.LastVerifyDateTime = DateTime.UtcNow;

            return CreateSuccessResult(result);
        }

        [HttpPost]
        [Route("readdata")]
        public async Task<IHttpActionResult> AppReadData(AppDataInfo info)
        {
            logger.Info("App请求读取AppData\nAppId={0}", info.AppId);

            var appAuthResult = await AuthApp(info.AppId, info.AppSecret);
            if (appAuthResult == UCenterResult.AppLoginFailedNotExit)
            {
                return CreateErrorResult(UCenterResult.AppLoginFailedNotExit, "App does not exist");
            }
            if (appAuthResult == UCenterResult.AppLoginFailedSecretError)
            {
                return CreateErrorResult(UCenterResult.AppLoginFailedSecretError, "App secret incorrect");
            }

            var result = await db.Bucket.FirstOrDefaultAsync<AppDataEntity>(d => d.AppId == info.AppId && d.AccountName == info.AccountName);

            return CreateSuccessResult(result);
        }

        [HttpPost]
        [Route("writedata")]
        public async Task<IHttpActionResult> AppWriteData(AppDataInfo info)
        {
            logger.Info("App请求写入AppData\nAppId={0}", info.AppId);

            var appAuthResult = await AuthApp(info.AppId, info.AppSecret);
            if (appAuthResult == UCenterResult.AppLoginFailedNotExit)
            {
                return CreateErrorResult(UCenterResult.AppLoginFailedNotExit, "App does not exist");
            }
            if (appAuthResult == UCenterResult.AppLoginFailedSecretError)
            {
                return CreateErrorResult(UCenterResult.AppLoginFailedSecretError, "App secret incorrect");
            }

            var appData = await db.Bucket.FirstOrDefaultAsync<AppDataEntity>(d => d.AppId == info.AppId && d.AccountName == info.AccountName);
            if (appData == null)
            {
                appData = new AppDataEntity()
                {
                    AppId = info.AppId,
                    AccountName = info.AccountName,
                    Data = info.Data
                };
            }

            await db.Bucket.UpsertSlimAsync<AppDataEntity>(appData);

            return CreateSuccessResult(appData);
        }

        private async Task<UCenterResult> AuthApp(string appId, string appSecret)
        {
            var app = await this.db.Bucket.FirstOrDefaultAsync<AppEntity>(a => a.AppId == appId);
            if (app == null)
            {
                return UCenterResult.AppLoginFailedNotExit;
            }
            if (appSecret != app.AppSecret)
            {
                return UCenterResult.AppLoginFailedSecretError;
            }

            return UCenterResult.Success;
        }
    }
}
