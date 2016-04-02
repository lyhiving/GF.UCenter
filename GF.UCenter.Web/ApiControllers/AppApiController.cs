using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Web.Http;
using GF.UCenter.Common.Models;
using GF.UCenter.Common.Portable;
using GF.UCenter.CouchBase;

namespace GF.UCenter.Web.ApiControllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/app")]
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
            logger.Info("App.Create AppId={0}", info.AppId);

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
            logger.Info($"App.VerifyAccount AppId={info.AppId} AccountId={info.AccountId}");

            await VerifyApp(info.AppId, info.AppSecret);
            var account = await this.GetAndVerifyAccount(info.AccountId, info.AccountToken);

            var result = new AppVerifyAccountResponse();
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
            logger.Info($"App.ReadAppAccountData AppId={info.AppId} AccountId={info.AccountId}");

            await VerifyApp(info.AppId, info.AppSecret);

            var account = await this.GetAndVerifyAccount(info.AccountId);
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
            logger.Info($"App.WriteAppAccountData AppId={info.AppId} AccountId={info.AccountId}");

            await VerifyApp(info.AppId, info.AppSecret);

            var account = await this.GetAndVerifyAccount(info.AccountId);

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
        private async Task VerifyApp(string appId, string appSecret)
        {
            var app = await this.db.Bucket.GetByEntityIdSlimAsync<AppEntity>(appId);
            if (app == null)
            {
                throw new UCenterException(UCenterErrorCode.AppNotExit);
            }
            if (appSecret != app.AppSecret)
            {
                throw new UCenterException(UCenterErrorCode.AppAuthFailedSecretNotMatch);
            }
        }

        private async Task<AccountEntity> GetAndVerifyAccount(string accountId)
        {
            var account = await db.Bucket.GetByEntityIdSlimAsync<AccountEntity>(accountId, throwIfFailed: false);
            if (account == null)
            {
                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }

            return account;
        }

        private async Task<AccountEntity> GetAndVerifyAccount(string accountId, string accountToken)
        {
            var account = await this.GetAndVerifyAccount(accountId);
            if (account.Token != accountToken)
            {
                throw new UCenterException(UCenterErrorCode.AccountLoginFailedTokenNotMatch);
            }

            return account;
        }

        private string CreateAppAccountDataId(string appId, string accountId)
        {
            return $"{appId}##{accountId}";
        }
    }
}
