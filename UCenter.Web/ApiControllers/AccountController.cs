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
using System.Threading;
using UCenter.Common;
using UCenter.Common.Database.Entities;
using UCenter.Common.Exceptions;
using UCenter.Common.Filters;
using UCenter.Common.Database.Couch;

namespace UCenter.Web.ApiControllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/account")]
    [ValidateModel]
    public class AccountController : ApiControllerBase
    {
        [ImportingConstructor]
        public AccountController(CouchBaseContext db)
            : base(db)
        {
        }

        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> AccountRegister(AccountRegisterInfo info, CancellationToken token)
        {
            string message = string.Format("客户端请求注册\nAcc={0}  Pwd={1}", info.AccountName, info.Password);
            //Logger.Info(info);

            var accountEntity = new AccountEntity()
            {
                AccountName = info.AccountName,
                Password = info.Password,
                Name = info.Name,
                IdentityNum = info.IdentityNum,
                PhoneNum = info.PhoneNum,
                Sex = info.Sex
            };

            var remoteEntities = this.db.Accounts.QueryByType<AccountEntity>(a => a.AccountName == accountEntity.AccountName);

            // var remoteEntities = await this.tableModel.RetrieveEntitiesAsync(e => e.Name == accountEntity.Name || e.PhoneNum == accountEntity.PhoneNum, token);
            if (remoteEntities.Count() > 0)
            {
                return CreateErrorResult(UCenterResult.RegisterAccountExist, "The account already exists.");
            }

            // encrypted the user password.
            accountEntity.Password = EncryptHashManager.ComputeHash(accountEntity.Password);

            var result = await this.db.Accounts.InsertAsync(accountEntity.ToDocument());

            return CreateSuccessResult(result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> AccountLogin(AccountLoginInfo info, CancellationToken token)
        {
            // string info = string.Format("客户端请求登录\nAcc={0}  Pwd={1}", request.AccountName, request.Password);
            //Logger.Info(info);
            var accounts = this.db.Accounts
                .QueryByType<AccountEntity>(a => a.AccountName == info.AccountName);
            UCenterResult code;
            if (accounts.Count() != 1 || !EncryptHashManager.VerifyHash(info.Password, accounts.First().Password))
            {
                code = UCenterResult.LoginVerifyAccountNotExit;
            }
            else
            {
                code = UCenterResult.Success;
            }

            LoginRecordEntity record = new LoginRecordEntity()
            {
                AccountName = info.AccountName,
                Code = code,
                LoginTime = DateTime.UtcNow,
                UserAgent = Request.Headers.UserAgent.ToString(),
                ClientIp = this.GetClientIp(Request)
            };

            await this.db.LoginRecords.InsertAsync(record.ToDocument());

            if (code == UCenterResult.Success)
            {
                return CreateSuccessResult(accounts.First());
            }
            else
            {
                return CreateErrorResult(code, "Account name or password error.");
            }
        }

        [HttpGet]
        [Route("test")]
        public async Task<IHttpActionResult> Test(AccountLoginInfo info)
        {
            var accounts = new AccountEntity[]
            {
                new AccountEntity { AccountId = 1, AccountName = "Tomato Soup"  },
                new AccountEntity { AccountId = 2, AccountName = "Yo-yo"  },
                new AccountEntity { AccountId = 3, AccountName = "Hammer"}
            };

            LoginRecordEntity record = new LoginRecordEntity()
            {
                AccountName = "test",
                Code = UCenterResult.Success,
                LoginTime = DateTime.UtcNow,
                UserAgent = Request.Headers.UserAgent.ToString(),
                ClientIp = this.GetClientIp(Request)
            };


            return await Task.FromResult<IHttpActionResult>(CreateSuccessResult(accounts));
        }
    }
}
