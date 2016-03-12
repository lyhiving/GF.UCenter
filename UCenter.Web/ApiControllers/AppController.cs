using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using UCenter.Common.Models;
using System.ComponentModel.Composition;
using System.Threading;
using UCenter.Common.Database.Entities;
using UCenter.Common.Database.Couch;
using UCenter.Common;

namespace UCenter.Web.ApiControllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/app")]
    public class AppController : ApiControllerBase
    {
        private DbClientMySQL ClientMySQL = new DbClientMySQL();

        [ImportingConstructor]
        public AppController(CouchBaseContext db)
            : base(db)
        {
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(AppLoginInfo info, CancellationToken token)
        {
            string message = string.Format("App请求登录\nAppId={0}", info.AppId);

            var app = await this.db.Bucket.FirstOrDefaultAsync<AppEntity>( a => a.AppId == info.AppId && a.AppSecret == info.AppSecret);

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
        public async Task<IHttpActionResult> AppVerifyAccount(AccountAppVerificationInfo info, CancellationToken token)
        {
            var result = new AccountAppInfo();
            result.AppId = info.AppId;
            result.AccountId = info.AccountId;
            result.AccountName = info.AccountName;

            // 验证App合法性
            //bool is_valid_app = await ClientMySQL.isValidApp(login_verify_request.app_id, login_verify_request.app_secret);
            //if (!is_valid_app)
            //{
            //    result.result = UCenterResult.LoginVerifyInvalidApp;
            //    return result;
            //}

            var app = await this.db.Bucket.FirstOrDefaultAsync<AppEntity>(a => a.AppId == info.AppId);
            if (app == null)
            {
                return CreateErrorResult(UCenterResult.AppLoginFailedNotExit, "App does not exist");
            }
            else
            {
                var account = await db.Bucket.GetSlimAsync<AccountEntity>(info.AccountId);
                if (account == null)
                {
                    return CreateErrorResult(UCenterResult.AccountLoginFailedNotExist, "Account does not exist");
                }
                else
                {
                    result.Token = account.Token;
                    result.LastLoginDateTime = account.LastLoginDateTime;

                    return CreateSuccessResult(result);
                }
            }
        }

        [HttpPost]
        [Route("writedata")]
        public async Task<IHttpActionResult> AppWriteData(AppWriteDataRequest write_appdata_request)
        {
            string info = string.Format("App请求写入AppData\nAppId={0}",
                write_appdata_request.app_id);
            //Logger.Info(info);

            var result = await ClientMySQL.appWriteData(write_appdata_request);

            return Ok(result);
        }

        [HttpPost]
        [Route("readdata")]
        public async Task<IHttpActionResult> AppReadData(AppReadDataRequest read_appdata_request)
        {
            string info = string.Format("App请求读取AppData\nAppId={0}",
                read_appdata_request.app_id);
            //Logger.Info(info);

            var result = await ClientMySQL.appReadData(read_appdata_request);

            return Ok(result);
        }
    }
}
