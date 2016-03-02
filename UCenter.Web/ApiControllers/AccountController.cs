using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using UCenter.Common.Models;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using UCenter.Common.Handler;
using System.Threading;
using UCenter.Common.Database.Entities;

namespace UCenter.Web.ApiControllers
{
    [Export]
    [RoutePrefix("api/account")]
    public class AccountController : ApiController
    {
        private DbClientMySQL ClientMySQL = new DbClientMySQL();
        private readonly AccountHandler handler;

        [ImportingConstructor]
        public AccountController(AccountHandler handler)
        {
            this.handler = handler;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> AccountRegister(AccountRegisterInfo info, CancellationToken token)
        {
            string message = string.Format("客户端请求注册\nAcc={0}  Pwd={1}", info.AccountName, info.Password);
            //Logger.Info(info);

            var dbEntity = new AccountEntity()
            {
                AccountName = info.Name,
                Password = info.Password,
                Name = info.Name,
                IdentityNum = info.IdentityNum,
                PhoneNum = info.PhoneNum,
                Sex = Sex.Female
            };

            var result = await this.handler.RegisterAsync(dbEntity, token);
            return await Task.FromResult<IHttpActionResult>(null);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> AccountLogin(AccountLoginRequest login_request)
        {
            string info = string.Format("客户端请求登录\nAcc={0}  Pwd={1}", login_request.acc, login_request.pwd);
            //Logger.Info(info);

            var result = await ClientMySQL.login(login_request.acc, login_request.pwd);

            return Ok(result);
        }

        [HttpGet]
        [Route("test")]
        public async Task<IHttpActionResult> Test()
        {
            return Ok(handler.GetTestData());
        }
    }
}
