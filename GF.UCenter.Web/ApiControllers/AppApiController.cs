using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Web.Http;
using GF.UCenter.Common;
using GF.UCenter.Common.Portable;
using GF.UCenter.CouchBase;
using NLog;

namespace GF.UCenter.Web.ApiControllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/app")]
    [TraceExceptionFilter("AppApiController")]
    public class AppApiController : ApiControllerBase
    {
        //---------------------------------------------------------------------
        private Logger logger = LogManager.GetCurrentClassLogger();

        //---------------------------------------------------------------------
        [ImportingConstructor]
        public AppApiController(CouchBaseContext db)
            : base(db)
        {
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> Create([FromBody]AppInfo info)
        {
            logger.Info("AppServer请求创建App\nAppId={0}", info.AppId);

            var app = await this.db.Bucket.FirstOrDefaultAsync<AppEntity>(a => a.AppId == info.AppId);

            if (app == null)
            {
                var appEntity = new AppEntity()
                {
                    AppId = info.AppId,
                    AppSecret = info.AppSecret
                };

                await this.db.Bucket.InsertSlimAsync<AppEntity>(appEntity);
            }

            var response = new AppResponse()
            {
                AppId = info.AppId,
                AppSecret = info.AppSecret
            };
            return CreateSuccessResult(response);
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(AppLoginInfo info)
        {
            logger.Info("AppServer请求登录\nAppId={0}", info.AppId);

            var app = await this.db.Bucket.FirstOrDefaultAsync<AppEntity>(a => a.AppId == info.AppId && a.AppSecret == info.AppSecret);

            if (app == null)
            {
                return CreateErrorResult(UCenterErrorCode.AppLoginFailedNotExit, "App does not exist.");
            }

            app.Token = EncryptHashManager.GenerateToken();
            await this.db.Bucket.UpsertSlimAsync(app);

            //todo: need login record?
            return CreateSuccessResult(app);
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("verifyaccount")]
        public async Task<IHttpActionResult> AppVerifyAccount(AppVerifyAccountInfo info)
        {
            logger.Info($"AppServer请求验证Account\nAppId={info.AppId}\nAccountId={info.AccountId}");

            var result = new AppVerifyAccountResponse();

            var appAuthResult = await AuthApp(info.AppId, info.AppSecret);
            if (appAuthResult == UCenterErrorCode.AppLoginFailedNotExit)
            {
                return CreateErrorResult(UCenterErrorCode.AppLoginFailedNotExit, "App does not exist");

            }
            else if (appAuthResult == UCenterErrorCode.AppLoginFailedSecretError)
            {
                return CreateErrorResult(UCenterErrorCode.AppLoginFailedSecretError, "App secret incorrect");
            }

            var account = await db.Bucket.FirstOrDefaultAsync<AccountEntity>(a => a.AccountId == info.AccountId);
            if (account == null)
            {
                return CreateErrorResult(UCenterErrorCode.AccountLoginFailedNotExist, "Account does not exist");
            }

            result.AccountId = account.Id;
            result.AccountName = account.AccountName;
            result.AccountToken = account.Token;
            result.LastLoginDateTime = account.LastLoginDateTime;
            result.LastVerifyDateTime = DateTime.UtcNow;

            return CreateSuccessResult(result);
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("readdata")]
        public async Task<IHttpActionResult> AppReadData(AppDataInfo info)
        {
            logger.Info($"AppServer请求读取AccountData\nAppId={info.AppId}\nAccountId={info.AccountId}");

            var appAuthResult = await AuthApp(info.AppId, info.AppSecret);
            if (appAuthResult == UCenterErrorCode.AppLoginFailedNotExit)
            {
                return CreateErrorResult(UCenterErrorCode.AppLoginFailedNotExit, "App does not exist");
            }
            if (appAuthResult == UCenterErrorCode.AppLoginFailedSecretError)
            {
                return CreateErrorResult(UCenterErrorCode.AppLoginFailedSecretError, "App secret incorrect");
            }

            var result = await db.Bucket.FirstOrDefaultAsync<AppDataEntity>(d => d.AppId == info.AppId && d.AccountId == info.AccountId);

            var response = new AppDataResponse()
            {
                AppId = info.AppId,
                AccountId = info.AccountId,
                Data = result.Data
            };

            return CreateSuccessResult(result);
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("writedata")]
        public async Task<IHttpActionResult> AppWriteData(AppDataInfo info)
        {
            logger.Info($"AppServer请求读取AccountData\nAppId={info.AppId}\nAccountId={info.AccountId}");

            var appAuthResult = await AuthApp(info.AppId, info.AppSecret);
            if (appAuthResult == UCenterErrorCode.AppLoginFailedNotExit)
            {
                return CreateErrorResult(UCenterErrorCode.AppLoginFailedNotExit, "App does not exist");
            }
            if (appAuthResult == UCenterErrorCode.AppLoginFailedSecretError)
            {
                return CreateErrorResult(UCenterErrorCode.AppLoginFailedSecretError, "App secret incorrect");
            }

            var appData = await db.Bucket.FirstOrDefaultAsync<AppDataEntity>(d => d.AppId == info.AppId && d.AccountId == info.AccountId);
            if (appData != null)
            {
                appData.Data = info.Data;
            }
            else
            {
                appData = new AppDataEntity
                {
                    AppId = info.AppId,
                    AccountId = info.AccountId,
                    Data = info.Data
                };
            }

            await db.Bucket.UpsertSlimAsync<AppDataEntity>(appData);

            var response = new AppDataResponse()
            {
                AppId = info.AppId,
                AccountId = info.AccountId,
                Data = appData.Data
            };

            return CreateSuccessResult(appData);
        }

        //---------------------------------------------------------------------
        private async Task<UCenterErrorCode> AuthApp(string appId, string appSecret)
        {
            var app = await this.db.Bucket.FirstOrDefaultAsync<AppEntity>(a => a.AppId == appId);
            if (app == null)
            {
                return UCenterErrorCode.AppLoginFailedNotExit;
            }
            if (appSecret != app.AppSecret)
            {
                return UCenterErrorCode.AppLoginFailedSecretError;
            }

            return UCenterErrorCode.Success;
        }
    }
}
