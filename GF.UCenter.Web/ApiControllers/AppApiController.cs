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

            var app = await this.db.Bucket.GetByEntityIdSlimAsync<AppEntity>(info.AppId, throwIfFailed: false);

            if (app == null)
            {
                var appEntity = new AppEntity()
                {
                    Id = info.AppId,
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
        [Route("verifyaccount")]
        public async Task<IHttpActionResult> VerifyAccount(AppVerifyAccountInfo info)
        {
            logger.Info($"AppServer请求验证Account\nAppId={info.AppId}\nAccountId={info.AccountId}");

            var result = new AppVerifyAccountResponse();

            var appAuthResult = await VerifyApp(info.AppId, info.AppSecret);
            if (appAuthResult == UCenterErrorCode.AppNotExit)
            {
                return CreateErrorResult(UCenterErrorCode.AppNotExit, "App does not exist");

            }
            else if (appAuthResult == UCenterErrorCode.AppAuthFailedSecretNotMatch)
            {
                return CreateErrorResult(UCenterErrorCode.AppAuthFailedSecretNotMatch, "App and secret do not match");
            }

            var account = await db.Bucket.GetByEntityIdSlimAsync<AccountEntity>(info.AccountId, throwIfFailed: false);
            if (account == null || account.Token != info.AccountToken)
            {
                return CreateErrorResult(UCenterErrorCode.AccountLoginFailedTokenNotMatch, "Account and token do not match");
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
        public async Task<IHttpActionResult> ReadAppAccountData(AppAccountDataInfo info)
        {
            logger.Info($"AppServer请求读取AccountData\nAppId={info.AppId}\nAccountId={info.AccountId}");

            var appAuthResult = await VerifyApp(info.AppId, info.AppSecret);
            if (appAuthResult == UCenterErrorCode.AppNotExit)
            {
                return CreateErrorResult(UCenterErrorCode.AppNotExit, "App does not exist");
            }
            if (appAuthResult == UCenterErrorCode.AppAuthFailedSecretNotMatch)
            {
                return CreateErrorResult(UCenterErrorCode.AppAuthFailedSecretNotMatch, "App and secret do not match");
            }

            var account = await db.Bucket.GetByEntityIdSlimAsync<AccountEntity>(info.AccountId, throwIfFailed: false);
            if (account == null)
            {
                return CreateErrorResult(UCenterErrorCode.AccountNotExist, "Account does not exist");
            }

            string dataId = this.CreateAppAccountDataId(info.AppId, info.AccountId);
            var accountData = await db.Bucket.GetByEntityIdSlimAsync<AppAccountDataEntity>(dataId);

            var response = new AppAccountDataResponse()
            {
                AppId = info.AppId,
                AccountId = info.AccountId,
                Data = accountData?.Data
            };

            return CreateSuccessResult(response);
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("writedata")]
        public async Task<IHttpActionResult> WriteAppAccountData(AppAccountDataInfo info)
        {
            logger.Info($"AppServer请求读取AccountData\nAppId={info.AppId}\nAccountId={info.AccountId}");

            var appAuthResult = await VerifyApp(info.AppId, info.AppSecret);
            if (appAuthResult == UCenterErrorCode.AppNotExit)
            {
                return CreateErrorResult(UCenterErrorCode.AppNotExit, "App does not exist");
            }
            if (appAuthResult == UCenterErrorCode.AppAuthFailedSecretNotMatch)
            {
                return CreateErrorResult(UCenterErrorCode.AppAuthFailedSecretNotMatch, "App and secret do not match");
            }

            var account = await db.Bucket.GetByEntityIdSlimAsync<AccountEntity>(info.AccountId);
            if (account == null)
            {
                return CreateErrorResult(UCenterErrorCode.AccountNotExist, "Account does not exist");
            }

            string dataId = this.CreateAppAccountDataId(info.AppId, info.AccountId);
            var accountData = await db.Bucket.GetByEntityIdSlimAsync<AppAccountDataEntity>(dataId);
            if (accountData != null)
            {
                accountData.Data = info.Data;
            }
            else
            {
                accountData = new AppAccountDataEntity()
                {
                    Id = dataId,
                    AppId = info.AppId,
                    AccountId = info.AccountId,
                    Data = info.Data
                };
            }

            await db.Bucket.UpsertSlimAsync<AppAccountDataEntity>(accountData);

            var response = new AppAccountDataResponse()
            {
                AppId = info.AppId,
                AccountId = info.AccountId,
                Data = accountData.Data
            };

            return CreateSuccessResult(response);
        }

        //---------------------------------------------------------------------
        private async Task<UCenterErrorCode> VerifyApp(string appId, string appSecret)
        {
            var app = await this.db.Bucket.GetByEntityIdSlimAsync<AppEntity>(appId);
            if (app == null)
            {
                return UCenterErrorCode.AppNotExit;
            }
            if (appSecret != app.AppSecret)
            {
                return UCenterErrorCode.AppAuthFailedSecretNotMatch;
            }

            return UCenterErrorCode.Success;
        }

        private string CreateAppAccountDataId(string appId, string accountId)
        {
            return $"{appId}##{accountId}";
        }
    }
}
