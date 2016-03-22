using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Web.Http;
using Couchbase;
using GF.UCenter.Common;
using GF.UCenter.Common.Portable;
using GF.UCenter.CouchBase;
using NLog;

namespace GF.UCenter.Web.ApiControllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/account")]
    [ValidateModel]
    [TraceExceptionFilter("AccountApiController")]
    public class AccountApiController : ApiControllerBase
    {
        //---------------------------------------------------------------------
        private Logger logger = LogManager.GetCurrentClassLogger();

        //---------------------------------------------------------------------
        [ImportingConstructor]
        public AccountApiController(CouchBaseContext db)
            : base(db)
        {
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> Register([FromBody]AccountRegisterInfo info)
        {
            logger.Info($"AppClient请求登录请求注册\nAccoundName={info.AccountName}");

            var removeTempsIfError = new List<AccountResourceEntity>();
            var error = false;
            try
            {
                var account = await db.Accounts.FirstOrDefaultAsync<AccountEntity>(a => a.AccountName == info.AccountName);
                if (account != null)
                {
                    return CreateErrorResult(UCenterErrorCode.AccountRegisterFailedAlreadyExist, "The account already exists.");
                }

                account = new AccountEntity()
                {
                    AccountId = Guid.NewGuid().ToString(),
                    AccountName = info.AccountName,
                    IsGuest = false,
                    Name = info.Name,
                    IdentityNum = info.IdentityNum,
                    Password = EncryptHashManager.ComputeHash(info.Password),
                    SuperPassword = EncryptHashManager.ComputeHash(info.SuperPassword),
                    PhoneNum = info.PhoneNum,
                    Sex = info.Sex,
                    CreatedDateTime = DateTime.UtcNow
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
                return CreateSuccessResult(ToResponse<AccountRegisterResponse>(account));
            }
            catch (Exception ex)
            {
                error = true;
                if (ex is CouchBaseException)
                {
                    var status = (ex as CouchBaseException).Result as IDocumentResult<AccountResourceEntity>;
                    if (status != null)
                    {
                        return CreateErrorResult(UCenterErrorCode.AccountRegisterFailedAlreadyExist, "The account already exists.");
                    }
                }

                return CreateErrorResult(UCenterErrorCode.Failed, ex.Message);
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

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login([FromBody]AccountLoginInfo info)
        {
            logger.Info($"AppClient请求登录\nAccountName={info.AccountName}");

            var account = await this.db.Accounts.FirstOrDefaultAsync<AccountEntity>(a => a.AccountName == info.AccountName);
            if (account == null)
            {
                return CreateErrorResult(UCenterErrorCode.AccountLoginFailedNotExist, "Account does not exist");
            }
            else if (!EncryptHashManager.VerifyHash(info.Password, account.Password))
            {
                await this.RecordLogin(info.AccountName, UCenterErrorCode.AccountLoginFailedNotMatch, "The account name and password do not match");
                return CreateErrorResult(UCenterErrorCode.AccountLoginFailedNotMatch, "The account name and password do not match");
            }
            else
            {
                account.LastLoginDateTime = DateTime.UtcNow;
                account.Token = EncryptHashManager.GenerateToken();
                await this.db.Accounts.UpsertSlimAsync(account);
                await this.RecordLogin(info.AccountName, UCenterErrorCode.Success);
                // todo: update token and only return necesary properties.
                return CreateSuccessResult(ToResponse<AccountLoginResponse>(account));
            }
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("guest")]
        public async Task<IHttpActionResult> GuestLogin([FromBody]AccountLoginInfo info)
        {
            logger.Info("AppClient请求访客登录");

            var r = new Random();
            string accountId = Guid.NewGuid().ToString();
            string accountNamePostfix = r.Next(0, 1000000).ToString("D6");
            string accountName = $"uc_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}_{accountNamePostfix}";
            string token = EncryptHashManager.GenerateToken();
            string password = Guid.NewGuid().ToString();

            var account = new AccountEntity()
            {
                AccountId = accountId,
                AccountName = accountName,
                IsGuest = true,
                Password = EncryptHashManager.ComputeHash(password),
                Token = EncryptHashManager.GenerateToken(),
                CreatedDateTime = DateTime.UtcNow
            };

            await this.db.Accounts.InsertSlimAsync(account);

            var response = new AccountGuestLoginResponse()
            {
                AccountId = accountId,
                AccountName = accountName,
                Token = token,
                Password = password
            };
            return CreateSuccessResult(response);
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("convert")]
        public async Task<IHttpActionResult> Convert([FromBody]AccountConvertInfo info)
        {
            logger.Info($"AppClient请求访客账号转正式账号AccountName={info.AccountName}");

            var account = await this.db.Accounts.FirstOrDefaultAsync<AccountEntity>(a => a.AccountId == info.AccountId);
            if (account == null)
            {
                return CreateErrorResult(UCenterErrorCode.AccountLoginFailedNotExist, "Account does not exist");
            }
            if (!EncryptHashManager.VerifyHash(info.OldPassword, account.Password))
            {
                await this.RecordLogin(info.AccountId, UCenterErrorCode.AccountLoginFailedNotMatch, "The account name and password do not match");
                return CreateErrorResult(UCenterErrorCode.AccountLoginFailedNotMatch, "The account name and password do not match");
            }

            account.AccountName = info.AccountName;
            account.IsGuest = false;
            account.Name = info.Name;
            account.IdentityNum = info.IdentityNum;
            account.Password = EncryptHashManager.ComputeHash(info.Password);
            account.SuperPassword = EncryptHashManager.ComputeHash(info.SuperPassword);
            account.PhoneNum = info.PhoneNum;
            account.Sex = info.Sex;
            await this.db.Accounts.UpsertSlimAsync<AccountEntity>(account);
            await this.RecordLogin(info.AccountId, UCenterErrorCode.Success, "Account converted successfully.");
            return CreateSuccessResult(ToResponse<AccountRegisterResponse>(account));
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("resetpassword")]
        public async Task<IHttpActionResult> ResetPassword([FromBody]AccountResetPasswordInfo info)
        {
            var account = await this.db.Accounts.FirstOrDefaultAsync<AccountEntity>(a => a.AccountId == info.AccountId);
            if (account == null)
            {
                return CreateErrorResult(UCenterErrorCode.AccountLoginFailedNotExist, "Account does not exist");
            }
            else if (!EncryptHashManager.VerifyHash(info.SuperPassword, account.SuperPassword))
            {
                await this.RecordLogin(info.AccountId, UCenterErrorCode.AccountLoginFailedNotMatch, "The super password provided is incorrect");
                return CreateErrorResult(UCenterErrorCode.AccountLoginFailedNotMatch, "The super password provided is incorrect");
            }
            else
            {
                account.Password = EncryptHashManager.ComputeHash(info.Password);
                await this.db.Accounts.UpsertSlimAsync<AccountEntity>(account);
                await this.RecordLogin(info.AccountId, UCenterErrorCode.Success, "Reset password successfully.");
                return CreateSuccessResult(ToResponse<AccountResetPasswordResponse>(account));
            }
        }

        //---------------------------------------------------------------------
        [HttpGet]
        [Route("test")]
        public async Task<IHttpActionResult> Test(AccountLoginInfo info)
        {
            logger.Info("in account controller, test method");
            var accounts = await this.db.Accounts.QueryAsync<AccountEntity>(a => a.AccountName == "Ny7IBHtK");
            //// var accounts = bucket.Query<AccountEntity>("select id, accountName,phoneNum from ucenter as c where c.accountName='Ny7IBHtK'");
            //var context = new BucketContext(bucket);
            //var accounts = from a in context.Query<TestAccountEntity>() select a;

            return await Task.FromResult<IHttpActionResult>(CreateSuccessResult(accounts));
        }

        //---------------------------------------------------------------------
        private async Task RecordLogin(string accountName, UCenterErrorCode code, string comments = null)
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

        //---------------------------------------------------------------------
        // todo: clean up this later
        public TResponse ToResponse<TResponse>(AccountEntity entity) where TResponse : AccountRequestResponse
        {
            var res = new AccountResponse()
            {
                AccountId = entity.AccountId,
                AccountName = entity.AccountName,
                Password = entity.Password,
                SuperPassword = entity.Password,
                Token = entity.Token,
                LastLoginDateTime = entity.LastLoginDateTime,
                Name = entity.Name,
                Sex = entity.Sex,
                IdentityNum = entity.IdentityNum,
                PhoneNum = entity.PhoneNum,
                Email = entity.Email
            };

            var response = Activator.CreateInstance<TResponse>();
            response.ApplyEntity(res);

            return response;
        }
    }
}
