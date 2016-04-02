namespace GF.UCenter.Web.ApiControllers
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using System.Web.Http;
    using CouchBase.Database;
    using CouchBase.Entities;
    using UCenter.Common.Models.AppServer;
    using UCenter.Common.Portable.Contracts;
    using UCenter.Common.Portable.Exceptions;

    /// <summary>
    ///     UCenter app api controller
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/app")]
    public class AppApiController : ApiControllerBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ImportingConstructor" /> class.
        /// </summary>
        /// <param name="db">The couch base context.</param>
        [ImportingConstructor]
        public AppApiController(CouchBaseContext db)
            : base(db)
        {
        }

        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> Create([FromBody] AppInfo info)
        {
            Logger.Info("App.Create AppId={0}", info.AppId);

            var app = await this.DatabaseContext.Bucket.GetByEntityIdSlimAsync<AppEntity>(info.AppId, false);

            if (app == null)
            {
                var appEntity = new AppEntity
                {
                    Id = info.AppId,
                    AppSecret = info.AppSecret
                };

                await this.DatabaseContext.Bucket.InsertSlimAsync(appEntity);
            }

            var response = new AppResponse
            {
                AppId = info.AppId,
                AppSecret = info.AppSecret
            };
            return CreateSuccessResult(response);
        }

        [HttpPost]
        [Route("verifyaccount")]
        public async Task<IHttpActionResult> VerifyAccount(AppVerifyAccountInfo info)
        {
            Logger.Info($"App.VerifyAccount AppId={info.AppId} AccountId={info.AccountId}");

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

        [HttpPost]
        [Route("readdata")]
        public async Task<IHttpActionResult> ReadAppAccountData(AppAccountDataInfo info)
        {
            Logger.Info($"App.ReadAppAccountData AppId={info.AppId} AccountId={info.AccountId}");

            await VerifyApp(info.AppId, info.AppSecret);

            var account = await this.GetAndVerifyAccount(info.AccountId);
            var dataId = this.CreateAppAccountDataId(info.AppId, info.AccountId);
            var accountData = await DatabaseContext.Bucket.GetByEntityIdSlimAsync<AppAccountDataEntity>(dataId);

            var response = new AppAccountDataResponse
            {
                AppId = info.AppId,
                AccountId = info.AccountId,
                Data = accountData?.Data
            };

            return CreateSuccessResult(response);
        }

        [HttpPost]
        [Route("writedata")]
        public async Task<IHttpActionResult> WriteAppAccountData(AppAccountDataInfo info)
        {
            Logger.Info($"App.WriteAppAccountData AppId={info.AppId} AccountId={info.AccountId}");

            await VerifyApp(info.AppId, info.AppSecret);

            var account = await this.GetAndVerifyAccount(info.AccountId);

            var dataId = this.CreateAppAccountDataId(info.AppId, info.AccountId);
            var accountData = await DatabaseContext.Bucket.GetByEntityIdSlimAsync<AppAccountDataEntity>(dataId);
            if (accountData != null)
            {
                accountData.Data = info.Data;
            }
            else
            {
                accountData = new AppAccountDataEntity
                {
                    Id = dataId,
                    AppId = info.AppId,
                    AccountId = info.AccountId,
                    Data = info.Data
                };
            }

            await DatabaseContext.Bucket.UpsertSlimAsync(accountData);

            var response = new AppAccountDataResponse
            {
                AppId = info.AppId,
                AccountId = info.AccountId,
                Data = accountData.Data
            };

            return CreateSuccessResult(response);
        }

        private async Task VerifyApp(string appId, string appSecret)
        {
            var app = await this.DatabaseContext.Bucket.GetByEntityIdSlimAsync<AppEntity>(appId);
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
            var account = await DatabaseContext.Bucket.GetByEntityIdSlimAsync<AccountEntity>(accountId, false);
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