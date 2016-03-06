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

namespace UCenter.Web.ApiControllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/app")]
    public class AppController : ApiControllerBase
    {
        private DbClientMySQL ClientMySQL = new DbClientMySQL();
        private readonly DatabaseTableModel<AccountEntity> acTableModel;
        private readonly DatabaseTableModel<AppEntity> appTableModel;

        [ImportingConstructor]
        public AppController(DatabaseTableModel<AccountEntity> acTableModel, DatabaseTableModel<AppEntity> appTableModel)
        {
            this.acTableModel = acTableModel;
            this.appTableModel = appTableModel;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(AppLoginInfo info, CancellationToken token)
        {
            string message = string.Format("App请求登录\nAppId={0}", info.AppId);
            //Logger.Info(info);

            var apps = await appTableModel.RetrieveEntitiesAsync(e => e.AppId == info.AppId && e.AppSecret == info.AppSecret, token);
            if (apps.Count != 1)
            {
                return null;
            }

            var appEntity = apps.First();
            appEntity.Token = Guid.NewGuid().ToString();
            await appTableModel.UpdateEntityAsync(appEntity, token);

            return CreateSuccessResult(appEntity);
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

            var apps = await appTableModel.RetrieveEntitiesAsync(e => e.AppId == info.AppId, token);
            if (apps.Count != 1)
            {
                CreateErrorResult(UCenterResult.Failed, "App auth fail");
            }

            string message = string.Format("App请求验证帐号\nAppId={0} AccId={1} AccName={2}",
                info.AppId, info.AccountId, info.AccountName);
            //Logger.Info(info);

            // 获取用户Token信息
            var accounts = await acTableModel.RetrieveEntitiesAsync(e => e.AccountId == info.AccountId, token);
            if (accounts.Count != 1)
            {
                CreateErrorResult(UCenterResult.Failed, "Can not find user");
            }

            result.Token = accounts.First().Token;
            result.LastLoginDateTime = accounts.First().LastLoginDateTime;


            //// 获取AppData
            //if (login_verify_request.get_appdata)
            //{
            //    var read_appdata_request = new AppReadDataRequest();
            //    read_appdata_request.app_id = login_verify_request.app_id;
            //    read_appdata_request.acc_id = login_verify_request.acc_id;

            //    var read_appdata_response = await ClientMySQL.appReadData(read_appdata_request);
            //    if (read_appdata_response.result != UCenterResult.Success)
            //    {
            //        return Ok(result);
            //    }

            //    result.app_data = read_appdata_response.app_data;
            //}

            return CreateSuccessResult(result);
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
