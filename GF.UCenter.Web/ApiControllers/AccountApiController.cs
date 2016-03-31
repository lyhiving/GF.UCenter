using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using Couchbase;
using GF.UCenter.Common;
using GF.UCenter.Common.Models;
using GF.UCenter.Common.Portable;
using GF.UCenter.CouchBase;
using Microsoft.WindowsAzure.Storage;

namespace GF.UCenter.Web.ApiControllers
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/account")]
    public class AccountApiController : ApiControllerBase
    {
        //---------------------------------------------------------------------
        private readonly Settings settings;

        //---------------------------------------------------------------------
        [ImportingConstructor]
        public AccountApiController(CouchBaseContext db, Settings settings)
            : base(db)
        {
            this.settings = settings;
        }

        //---------------------------------------------------------------------
        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> Register([FromBody]AccountRegisterRequestInfo info)
        {
            logger.Info($"Account.Register AccountName={info.AccountName}");

            var removeTempsIfError = new List<AccountResourceEntity>();
            var error = false;
            try
            {
                var account = await db.Bucket.FirstOrDefaultAsync<AccountEntity>(a => a.AccountName == info.AccountName, throwIfFailed: false);
                if (account != null)
                {
                    throw new UCenterException(UCenterErrorCode.AccountRegisterFailedAlreadyExist);
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
                logger.Info($"Account.Register Exception：AccoundName={info.AccountName}");
                logger.Info(ex.ToString());

                error = true;
                if (ex is CouchBaseException)
                {
                    var status = (ex as CouchBaseException).Result as IDocumentResult<AccountResourceEntity>;
                    if (status != null)
                    {
                        throw new UCenterException(UCenterErrorCode.AccountRegisterFailedAlreadyExist, ex);
                    }
                }

                throw;
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
            logger.Info($"Account.Login AccountName={info.AccountName}");

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
                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }
            else if (!EncryptHashManager.VerifyHash(info.Password, account.Password))
            {
                await this.RecordLogin(account, UCenterErrorCode.AccountLoginFailedPasswordNotMatch, "The account name and password do not match");

                throw new UCenterException(UCenterErrorCode.AccountLoginFailedPasswordNotMatch);
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
            logger.Info("Account.GuestLogin");

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
            logger.Info($"Account.Convert AccountName={info.AccountName}");

            var account = await GetAndVerifyAccount(info.AccountId);

            if (!EncryptHashManager.VerifyHash(info.OldPassword, account.Password))
            {
                await this.RecordLogin(account, UCenterErrorCode.AccountLoginFailedPasswordNotMatch, "The account name and password do not match");
                throw new UCenterException(UCenterErrorCode.AccountLoginFailedPasswordNotMatch);
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
            logger.Info($"Account.ResetPassword AccountName={info.AccountId}");

            var account = await GetAndVerifyAccount(info.AccountId);

            if (!EncryptHashManager.VerifyHash(info.SuperPassword, account.SuperPassword))
            {
                await this.RecordLogin(account, UCenterErrorCode.AccountLoginFailedPasswordNotMatch, "The super password provided is incorrect");
                throw new UCenterException(UCenterErrorCode.AccountLoginFailedPasswordNotMatch);
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
        [Route("upload/{accountId}")]
        public async Task<IHttpActionResult> UploadProfileImage([FromUri]string accountId)
        {
            logger.Info($"Account.UploadProfileImage AccountId={accountId}");

            var account = await GetAndVerifyAccount(accountId);

            using (Stream stream = await this.Request.Content.ReadAsStreamAsync())
            {
                string blobName = $"profile_l_{accountId}.jpg";
                var storageAccount = CloudStorageAccount.Parse(this.settings.UCStorageConnectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var blobContainer = blobClient.GetContainerReference(this.settings.ImageContainerName);
                var blob = blobContainer.GetBlockBlobReference(blobName);

                // todo: retry if upload failed?
                await blob.UploadFromStreamAsync(stream);
                logger.Info($"Uploading successfully, url = {blob.Uri.AbsoluteUri}");

                account.ProfileImage = blob.Uri.AbsoluteUri;
                await this.db.Bucket.UpsertSlimAsync<AccountEntity>(account);
                await this.RecordLogin(account, UCenterErrorCode.Success, "Profile image uploaded successfully.");
                return CreateSuccessResult(ToResponse<AccountUploadProfileImageResponse>(account));
            }
        }

        //---------------------------------------------------------------------
        [HttpGet]
        [Route("test")]
        public async Task<IHttpActionResult> Test(AccountLoginInfo info)
        {
            logger.Info($"Account.Test");

            var accounts = await this.db.Bucket.QueryAsync<AccountEntity>(a => a.AccountName == "Ny7IBHtK");

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
        private async Task<AccountEntity> GetAndVerifyAccount(string accountId)
        {
            var account = await db.Bucket.GetByEntityIdSlimAsync<AccountEntity>(accountId, throwIfFailed: false);
            if (account == null)
            {
                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }

            return account;
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
