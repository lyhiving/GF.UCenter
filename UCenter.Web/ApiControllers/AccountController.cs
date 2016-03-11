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
        public async Task<IHttpActionResult> Register(AccountRegisterInfo info, CancellationToken token)
        {
            string message = string.Format("客户端请求注册\nAcc={0}  Pwd={1}", info.AccountName, info.Password);
            //Logger.Info(info);

            var accountEntity = new AccountEntity()
            {
                AccountName = info.AccountName,
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
            accountEntity.Password = EncryptHashManager.ComputeHash(info.Password);
            accountEntity.SuperPassword = EncryptHashManager.ComputeHash(info.SuperPassword);

            var result = await this.db.Accounts.InsertAsync(accountEntity.ToDocument());

            return CreateSuccessResult(result);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login(AccountLoginInfo info, CancellationToken token)
        {
            // string info = string.Format("客户端请求登录\nAcc={0}  Pwd={1}", request.AccountName, request.Password);
            //Logger.Info(info);
            var accounts = this.db.Accounts
                .QueryByType<AccountEntity>(a => a.AccountName == info.AccountName);
            UCenterResult code;

            if (accounts.Count() != 1)
            {
                code = UCenterResult.LoginVerifyAccountNotExit;
            }
            else if (!EncryptHashManager.VerifyHash(info.Password, accounts.First().Password))
            {
                code = UCenterResult.LoginPwdError;
            }
            else
            {
                code = UCenterResult.Success;
                await this.RecordLogin(accounts.First(), code, string.Empty);
                return CreateSuccessResult(accounts.First());
            }

            return CreateErrorResult(code, "Account name or password error.");
        }

        [HttpPost]
        [Route("changepassword")]
        public async Task<IHttpActionResult> ChangePassword(AccountChangePasswordInfo info, CancellationToken token)
        {
            var accounts = this.db.Accounts
                .QueryByType<AccountEntity>(a => a.AccountName == info.AccountName);
            UCenterResult code;
            if (accounts.Count() != 1)
            {
                code = UCenterResult.LoginAccountNotExist;
            }
            else if (!EncryptHashManager.VerifyHash(info.Password, accounts.First().SuperPassword))
            {
                code = UCenterResult.LoginPwdError;
            }
            else
            {
                code = UCenterResult.Success;
                LoginRecordEntity record = new LoginRecordEntity()
                {
                    AccountName = info.AccountName,
                    Code = code,
                    LoginTime = DateTime.UtcNow,
                    UserAgent = Request.Headers.UserAgent.ToString(),
                    ClientIp = this.GetClientIp(Request),
                    Comments = "Change password."
                };

                await this.db.LoginRecords.InsertAsync(record.ToDocument());

                accounts.First().Password = EncryptHashManager.ComputeHash(info.Password);

                var result = await this.db.Accounts.UpsertAsync(accounts.First().ToDocument());
                return this.CreateResponse(result);
            }

            return this.CreateErrorResult(code, "Change password error.");
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

        private async Task RecordLogin(AccountEntity account, UCenterResult code, string comments)
        {
            LoginRecordEntity record = new LoginRecordEntity()
            {
                AccountName = account.AccountName,
                Code = code,
                LoginTime = DateTime.UtcNow,
                UserAgent = Request.Headers.UserAgent.ToString(),
                ClientIp = this.GetClientIp(Request),
                Comments = comments
            };

            await this.db.LoginRecords.InsertAsync(record.ToDocument());
        }
    }
}
