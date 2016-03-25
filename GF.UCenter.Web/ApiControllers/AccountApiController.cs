using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net.Http;
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
                var account = await db.Bucket.FirstOrDefaultAsync<AccountEntity>(a => a.AccountName == info.AccountName, throwIfFailed: false);
                if (account != null)
                {
                    return CreateErrorResult(UCenterErrorCode.AccountRegisterFailedAlreadyExist, "The account already exists.");
                }

                account = new AccountEntity()
                {
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

                await this.db.Bucket.InsertSlimAsync(account);
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

            var accountResourceByName = await this.db.Bucket.GetByEntityIdSlimAsync<AccountResourceEntity>(AccountResourceEntity.GenerateResourceId(AccountResourceType.AccountName, info.AccountName), false);
            AccountEntity account = null;
            if (accountResourceByName != null)
            {
                // this means the temp value still exists, we can directly get the account by account id.
                account = await this.db.Bucket.GetByEntityIdSlimAsync<AccountEntity>(accountResourceByName.AccountId);
            }
            else
            {
                // this means the temp value not exists any more, meanwhile, it have passed a period after the account created
                // so the index should be already created and we can query the entity by query string
                account = await this.db.Bucket.FirstOrDefaultAsync<AccountEntity>(a => a.AccountName == info.AccountName);
            }

            if (account == null)
            {
                return CreateErrorResult(UCenterErrorCode.AccountNotExist, "Account does not exist");
            }
            else if (!EncryptHashManager.VerifyHash(info.Password, account.Password))
            {
                await this.RecordLogin(account, UCenterErrorCode.AccountLoginFailedPasswordNotMatch, "The account name and password do not match");
                return CreateErrorResult(UCenterErrorCode.AccountLoginFailedPasswordNotMatch, "The account name and password do not match");
            }
            else
            {
                account.LastLoginDateTime = DateTime.UtcNow;
                account.Token = EncryptHashManager.GenerateToken();
                await this.db.Bucket.UpsertSlimAsync(account);
                await this.RecordLogin(account, UCenterErrorCode.Success);

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
            string accountNamePostfix = r.Next(0, 1000000).ToString("D6");
            string accountName = $"uc_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}_{accountNamePostfix}";
            string token = EncryptHashManager.GenerateToken();
            string password = Guid.NewGuid().ToString();

            var account = new AccountEntity()
            {
                AccountName = accountName,
                IsGuest = true,
                Password = EncryptHashManager.ComputeHash(password),
                Token = EncryptHashManager.GenerateToken(),
                CreatedDateTime = DateTime.UtcNow
            };

            await this.db.Bucket.InsertSlimAsync(account);

            var response = new AccountGuestLoginResponse()
            {
                AccountId = account.Id,
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

            var account = await this.db.Bucket.GetByEntityIdSlimAsync<AccountEntity>(info.AccountId);
            if (account == null)
            {
                return CreateErrorResult(UCenterErrorCode.AccountNotExist, "Account does not exist");
            }
            if (!EncryptHashManager.VerifyHash(info.OldPassword, account.Password))
            {
                await this.RecordLogin(account, UCenterErrorCode.AccountLoginFailedPasswordNotMatch, "The account name and password do not match");
                return CreateErrorResult(UCenterErrorCode.AccountLoginFailedPasswordNotMatch, "The account name and password do not match");
            }

            account.AccountName = info.AccountName;
            account.IsGuest = false;
            account.Name = info.Name;
            account.IdentityNum = info.IdentityNum;
            account.Password = EncryptHashManager.ComputeHash(info.Password);
            account.SuperPassword = EncryptHashManager.ComputeHash(info.SuperPassword);
            account.PhoneNum = info.PhoneNum;
            account.Email = info.Email;
            account.Sex = info.Sex;
            await this.db.Bucket.UpsertSlimAsync<AccountEntity>(account);
            await this.RecordLogin(account, UCenterErrorCode.Success, "Account converted successfully.");
            return CreateSuccessResult(ToResponse<AccountRegisterResponse>(account));
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("resetpassword")]
        public async Task<IHttpActionResult> ResetPassword([FromBody]AccountResetPasswordInfo info)
        {
            logger.Info($"AppClient请求重置密码AccountId={info.AccountId}");

            var account = await this.db.Bucket.GetByEntityIdSlimAsync<AccountEntity>(info.AccountId);
            if (account == null)
            {
                return CreateErrorResult(UCenterErrorCode.AccountNotExist, "Account does not exist");
            }
            else if (!EncryptHashManager.VerifyHash(info.SuperPassword, account.SuperPassword))
            {
                await this.RecordLogin(account, UCenterErrorCode.AccountLoginFailedPasswordNotMatch, "The super password provided is incorrect");
                return CreateErrorResult(UCenterErrorCode.AccountLoginFailedPasswordNotMatch, "The super password provided is incorrect");
            }
            else
            {
                account.Password = EncryptHashManager.ComputeHash(info.Password);
                await this.db.Bucket.UpsertSlimAsync<AccountEntity>(account);
                await this.RecordLogin(account, UCenterErrorCode.Success, "Reset password successfully.");
                return CreateSuccessResult(ToResponse<AccountResetPasswordResponse>(account));
            }
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("upload")]
        public async Task<IHttpActionResult> UploadProfileImage()
        //public async Task<IHttpActionResult> UploadProfileImage([FromBody]AccountUploadProfileImageInfo info)
        {
            logger.Info($"AppClient请求上传图片AccountId=475201a3-e9c7-4659-9cec-a3e31396ce83");

            var account = await this.db.Bucket.GetByEntityIdSlimAsync<AccountEntity>("475201a3-e9c7-4659-9cec-a3e31396ce83");
            if (account == null)
            {
                return CreateErrorResult(UCenterErrorCode.AccountNotExist, "Account does not exist");
            }

            string fileName = $"profile_l_475201a3-e9c7-4659-9cec-a3e31396ce83.jpg";
            var provider = new BlobStorageMultipartStreamProvider(fileName);
            logger.Info("Uploading raw profile image to azure storage");
            await Request.Content.ReadAsMultipartAsync(provider);
            logger.Info("Uploading completed");

            account.ProfileImage = provider.BlobUrl;
            await this.db.Bucket.UpsertSlimAsync<AccountEntity>(account);
            await this.RecordLogin(account, UCenterErrorCode.Success, "Profile image uploaded successfully.");
            return CreateSuccessResult(ToResponse<AccountUploadProfileImageResponse>(account));
        }

        [HttpPost]
        [Route("upload1")]
        public async Task<IHttpActionResult> UploadTest()
        {
            logger.Info($"AppClient请求上传图片");

            string fileName = $"profile_l.jpg";
            try
            {
                var provider = new BlobStorageMultipartStreamProvider(fileName);
                logger.Info("Uploading raw profile image to azure storage");
                await Request.Content.ReadAsMultipartAsync(provider);
                logger.Info($"Uploading completed, {provider.BlobUrl}");
            }
            catch (Exception ex)
            {
                CreateErrorResult(UCenterErrorCode.Failed, ex.Message);
            }

            return CreateSuccessResult("O");
        }

        //---------------------------------------------------------------------
        [HttpGet]
        [Route("test")]
        public async Task<IHttpActionResult> Test(AccountLoginInfo info)
        {
            logger.Info("in account controller, test method");
            var accounts = await this.db.Bucket.QueryAsync<AccountEntity>(a => a.AccountName == "Ny7IBHtK");
            //// var accounts = bucket.Query<AccountEntity>("select id, accountName,phoneNum from ucenter as c where c.accountName='Ny7IBHtK'");
            //var context = new BucketContext(bucket);
            //var accounts = from a in context.Query<TestAccountEntity>() select a;

            return await Task.FromResult<IHttpActionResult>(CreateSuccessResult(accounts));
        }

        //---------------------------------------------------------------------
        private async Task RecordLogin(AccountEntity account, UCenterErrorCode code, string comments = null)
        {
            LoginRecordEntity record = new LoginRecordEntity()
            {
                AccountName = account.AccountName,
                AccountId = account.Id,
                Code = code,
                LoginTime = DateTime.UtcNow,
                UserAgent = Request.Headers.UserAgent.ToString(),
                ClientIp = this.GetClientIp(Request),
                Comments = comments
            };

            await this.db.Bucket.InsertSlimAsync(record, throwIfFailed: false);
        }

        //---------------------------------------------------------------------
        // todo: clean up this later
        public TResponse ToResponse<TResponse>(AccountEntity entity) where TResponse : AccountRequestResponse
        {
            var res = new AccountResponse()
            {
                AccountId = entity.Id,
                AccountName = entity.AccountName,
                Password = entity.Password,
                SuperPassword = entity.Password,
                Token = entity.Token,
                LastLoginDateTime = entity.LastLoginDateTime,
                Name = entity.Name,
                ProfileImage = entity.ProfileImage,
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
