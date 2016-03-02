using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using UCenter.Common.Models;
using System.ComponentModel.Composition;

namespace UCenter.Web.ApiControllers
{
    [Export]
    [RoutePrefix("api/app")]
    public class AppController : ApiController
    {
        private DbClientMySQL ClientMySQL = new DbClientMySQL();

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> AppLogin(AppLoginRequest app_login_request)
        {
            string info = string.Format("App请求登录\nAppId={0}", app_login_request.app_id);
            //Logger.Info(info);

            var result = new AppLoginResponse();
            return await Task.FromResult<IHttpActionResult>(Ok(result));
        }

        [HttpPost]
        [Route("verifyaccount")]
        public async Task<IHttpActionResult> AppVerifyAccount(AppVerifyAccountRequest login_verify_request)
        {
            var result = new AppVerifyAccountResponse();
            result.result = UCenterResult.Failed;
            result.app_id = login_verify_request.app_id;
            result.acc_id = login_verify_request.acc_id;
            result.acc_name = login_verify_request.acc_name;

            // 验证App合法性
            //bool is_valid_app = await ClientMySQL.isValidApp(login_verify_request.app_id, login_verify_request.app_secret);
            //if (!is_valid_app)
            //{
            //    result.result = UCenterResult.LoginVerifyInvalidApp;
            //    return result;
            //}

            string info = string.Format("App请求验证帐号\nAppId={0} AccId={1} AccName={2}",
                login_verify_request.app_id, login_verify_request.acc_id, login_verify_request.acc_name);
            //Logger.Info(info);

            // 获取用户Token信息
            var acc_verify_data = await ClientMySQL.getAccountVerifyData(login_verify_request.acc_id);
            if (acc_verify_data.result != UCenterResult.Success)
            {
                result.result = acc_verify_data.result;
                return Ok(result);
            }

            result.token = acc_verify_data.token;
            result.last_login_dt = acc_verify_data.last_login_dt;
            result.now_dt = DateTime.Now;

            // 获取AppData
            if (login_verify_request.get_appdata)
            {
                var read_appdata_request = new AppReadDataRequest();
                read_appdata_request.app_id = login_verify_request.app_id;
                read_appdata_request.acc_id = login_verify_request.acc_id;

                var read_appdata_response = await ClientMySQL.appReadData(read_appdata_request);
                if (read_appdata_response.result != UCenterResult.Success)
                {
                    result.result = acc_verify_data.result;
                    return Ok(result);
                }

                result.app_data = read_appdata_response.app_data;
            }

            result.result = UCenterResult.Success;
            return Ok(result);
        }

        [HttpPost]
        [Route("writedata")]
        //---------------------------------------------------------------------
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
