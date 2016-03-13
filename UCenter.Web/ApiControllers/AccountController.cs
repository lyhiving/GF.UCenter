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
using UCenter.Common.Database.Couch;
using Couchbase;
using UCenter.Common.Attributes;
using UCenter.Common.Database;

namespace UCenter.Web.ApiControllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/account")]
    [ValidateModel]
    [TraceExceptionFilter("AccountController")]
    public class AccountController : ApiControllerBase
    {
        [ImportingConstructor]
        public AccountController(CouchBaseContext db)
            : base(db)
        {
        }

        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> Register([FromBody]AccountRegisterInfo info, CancellationToken token)
        {
            string message = string.Format("客户端请求注册\nAcc={0}  Pwd={1}", info.AccountName, info.Password);
            //Logger.Info(info);

            var removeTempsIfError = new List<AccountResourceEntity>();
            var error = false;
            try
            {
                var account = await db.Accounts.FirstOrDefaultAsync<AccountEntity>(a => a.AccountName == info.AccountName);
                if (account != null)
                {
                    return CreateErrorResult(UCenterResult.AccountRegisterFailedAlreadyExist, "The account already exists.");
                }

                account = new AccountEntity()
                {
                    AccountName = info.AccountName,
                    Name = info.Name,
                    IdentityNum = info.IdentityNum,
                    Password = EncryptHashManager.ComputeHash(info.Password),
                    SuperPassword = EncryptHashManager.ComputeHash(info.SuperPassword),
                    PhoneNum = info.PhoneNum,
                    Sex = info.Sex
                };

                if (!string.IsNullOrEmpty(account.AccountName))
                {
                    var namePointer = new AccountResourceEntity(account, AccountResourceType.AccountName);
                    await this.db.Bucket.InsertSlimAsync<AccountResourceEntity>(namePointer);
                    removeTempsIfError.Add(namePointer);
                }
                if (!string.IsNullOrEmpty(account.PhoneNum))
                {
                    var phonePointer = new AccountResourceEntity(account, AccountResourceType.Phone);
                    await this.db.Bucket.InsertSlimAsync<AccountResourceEntity>(phonePointer);
                    removeTempsIfError.Add(phonePointer);
                }
                else if (!string.IsNullOrEmpty(account.Email))
                {
                    var emailPointer = new AccountResourceEntity(account, AccountResourceType.Email);
                    await this.db.Bucket.InsertSlimAsync<AccountResourceEntity>(emailPointer);
                    removeTempsIfError.Add(emailPointer);
                }

                await this.db.Accounts.InsertSlimAsync(account);
                return CreateSuccessResult(account.ToResponse<AccountRegisterResponse>());
            }
            catch (Exception ex)
            {
                error = true;
                if (ex is CouchBaseException)
                {
                    var status = (ex as CouchBaseException).Result as IDocumentResult<AccountResourceEntity>;
                    if (status != null)
                    {
                        return CreateErrorResult(UCenterResult.AccountRegisterFailedAlreadyExist, "The account already exists.");
                    }
                }

                return CreateErrorResult(UCenterResult.Failed, ex.Message);
            }
            finally
            {
                if (error)
                {
                    foreach (var item in removeTempsIfError)
                    {
                        this.db.Bucket.Remove<AccountResourceEntity>(item.ToDocument());
                    }
                }
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login([FromBody]AccountLoginInfo info, CancellationToken token)
        {
            // string info = string.Format("客户端请求登录\nAcc={0}  Pwd={1}", request.AccountName, request.Password);
            //Logger.Info(info);

            var account = await this.db.Accounts.FirstOrDefaultAsync<AccountEntity>(a => a.AccountName == info.AccountName);
            if (account == null)
            {
                return CreateErrorResult(UCenterResult.AccountLoginFailedNotExist, "Account does not exist");
            }
            else if (!EncryptHashManager.VerifyHash(info.Password, account.Password))
            {
                await this.RecordLogin(info.AccountName, UCenterResult.AccountLoginFailedPasswordError, "Password incorrect");
                return CreateErrorResult(UCenterResult.AccountLoginFailedPasswordError, "Password incorrect");
            }
            else
            {
                account.LastLoginDateTime = DateTime.UtcNow;
                account.Token = EncryptHashManager.GenerateToken();
                await this.db.Accounts.UpsertSlimAsync(account);
                await this.RecordLogin(info.AccountName, UCenterResult.Success);
                // todo: update token and only return necesary properties.
                return CreateSuccessResult(account.ToResponse<AccountLoginResponse>());
            }
        }

        [HttpPost]
        [Route("changepassword")]
        public async Task<IHttpActionResult> ChangePassword([FromBody]AccountChangePasswordInfo info, CancellationToken token)
        {
            var account = await this.db.Accounts.FirstOrDefaultAsync<AccountEntity>(a => a.AccountName == info.AccountName);
            if (account == null)
            {
                return CreateErrorResult(UCenterResult.AccountLoginFailedNotExist, "Account not exists or password is wrong.");
            }
            else if (!EncryptHashManager.VerifyHash(info.SuperPassword, account.SuperPassword))
            {
                await this.RecordLogin(info.AccountName, UCenterResult.AccountLoginFailedPasswordError, "Change password with wrong super password.");
                return CreateErrorResult(UCenterResult.AccountLoginFailedPasswordError, "Account not exists or password is wrong.");
            }
            else
            {
                account.Password = EncryptHashManager.ComputeHash(info.Password);
                await this.db.Accounts.UpsertSlimAsync<AccountEntity>(account);
                await this.RecordLogin(info.AccountName, UCenterResult.Success, "Change password successfully.");
                return CreateSuccessResult(account.ToResponse<AccountChangePasswordResponse>());
            }
        }

        [HttpGet]
        [Route("test")]
        public async Task<IHttpActionResult> Test(AccountLoginInfo info)
        {
            var accounts = await this.db.Accounts.QueryAsync<AccountEntity>(a => a.AccountName == "Ny7IBHtK");
            //// var accounts = bucket.Query<AccountEntity>("select id, accountName,phoneNum from ucenter as c where c.accountName='Ny7IBHtK'");
            //var context = new BucketContext(bucket);
            //var accounts = from a in context.Query<TestAccountEntity>() select a;

            return await Task.FromResult<IHttpActionResult>(CreateSuccessResult(accounts));
        }

        private async Task RecordLogin(string accountName, UCenterResult code, string comments = null)
        {
            LoginRecordEntity record = new LoginRecordEntity()
            {
                AccountName = accountName,
                Code = code,
                LoginTime = DateTime.UtcNow,
                UserAgent = Request.Headers.UserAgent.ToString(),
                ClientIp = this.GetClientIp(Request),
                Comments = comments
            };

            await this.db.LoginRecords.InsertSlimAsync(record, throwIfFailed: false);
        }
    }
}
