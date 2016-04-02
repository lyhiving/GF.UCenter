/// <summary>
/// UCenter account api controller
/// </summary>

namespace GF.UCenter.Web
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Drawing;
    using System.IO;
    using System.Net.Http;
    using System.ServiceModel.Channels;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using Common;
    using Common.Portable;
    using Common.Settings;
    using Comomn;
    using Couchbase;
    using CouchBase;

    /// <summary>
    ///     UCenter account api controller
    /// </summary>
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [RoutePrefix("api/account")]
    public class AccountApiController : ApiControllerBase
    {
        private readonly Settings settings;
        private readonly StorageAccountContext storageContext;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AccountApiController" /> class.
        /// </summary>
        /// <param name="db">The couch base context.</param>
        /// <param name="settings">The UCenter settings.</param>
        /// <param name="storageContext">The storage account context.</param>
        [ImportingConstructor]
        public AccountApiController(CouchBaseContext db, Settings settings, StorageAccountContext storageContext)
            : base(db)
        {
            this.settings = settings;
            this.storageContext = storageContext;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IHttpActionResult> Register([FromBody] AccountRegisterRequestInfo info,
            CancellationToken token)
        {
            this.Logger.Info($"Account.Register AccountName={info.AccountName}");

            var removeTempsIfError = new List<AccountResourceEntity>();
            var error = false;
            try
            {
                var account =
                    await
                        DatabaseContext.Bucket.FirstOrDefaultAsync<AccountEntity>(
                            a => a.AccountName == info.AccountName, false);
                if (account != null)
                {
                    throw new UCenterException(UCenterErrorCode.AccountRegisterFailedAlreadyExist);
                }

                account = new AccountEntity
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
                    await this.DatabaseContext.Bucket.InsertSlimAsync(namePointer);
                    removeTempsIfError.Add(namePointer);
                }
                if (!string.IsNullOrEmpty(account.PhoneNum))
                {
                    var phonePointer = new AccountResourceEntity(account, AccountResourceType.Phone);
                    await this.DatabaseContext.Bucket.InsertSlimAsync(phonePointer);
                    removeTempsIfError.Add(phonePointer);
                }
                else if (!string.IsNullOrEmpty(account.Email))
                {
                    var emailPointer = new AccountResourceEntity(account, AccountResourceType.Email);
                    await this.DatabaseContext.Bucket.InsertSlimAsync(emailPointer);
                    removeTempsIfError.Add(emailPointer);
                }

                // set the default profiles
                account.ProfileImage = await this.storageContext.CopyBlobAsync(
                    account.Sex == Sex.Female
                        ? this.settings.DefaultProfileImageForFemaleBlobName
                        : this.settings.DefaultProfileImageForMaleBlobName,
                    this.settings.ProfileImageForBlobNameTemplate.FormatInvariant(account.Id),
                    token);

                account.ProfileThumbnail = await this.storageContext.CopyBlobAsync(
                    account.Sex == Sex.Female
                        ? this.settings.DefaultProfileThumbnailForFemaleBlobName
                        : this.settings.DefaultProfileThumbnailForMaleBlobName,
                    this.settings.ProfileThumbnailForBlobNameTemplate.FormatInvariant(account.Id),
                    token);

                await this.DatabaseContext.Bucket.InsertSlimAsync(account);

                return CreateSuccessResult(ToResponse<AccountRegisterResponse>(account));
            }
            catch (Exception ex)
            {
                this.Logger.Info($"Account.Register Exception：AccoundName={info.AccountName}");
                this.Logger.Info(ex.ToString());

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
                        this.DatabaseContext.Bucket.Remove(item.ToDocument());
                    }
                }
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IHttpActionResult> Login([FromBody] AccountLoginInfo info)
        {
            Logger.Info($"Account.Login AccountName={info.AccountName}");

            var accountResourceByName =
                await
                    this.DatabaseContext.Bucket.GetByEntityIdSlimAsync<AccountResourceEntity>(
                        AccountResourceEntity.GenerateResourceId(AccountResourceType.AccountName, info.AccountName),
                        false);
            AccountEntity account = null;
            if (accountResourceByName != null)
            {
                // this means the temp value still exists, we can directly get the account by account id.
                account =
                    await
                        this.DatabaseContext.Bucket.GetByEntityIdSlimAsync<AccountEntity>(
                            accountResourceByName.AccountId);
            }
            else
            {
                // this means the temp value not exists any more, meanwhile, it have passed a period after the account created
                // so the index should be already created and we can query the entity by query string
                account =
                    await
                        this.DatabaseContext.Bucket.FirstOrDefaultAsync<AccountEntity>(
                            a => a.AccountName == info.AccountName);
            }

            if (account == null)
            {
                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }
            if (!EncryptHashManager.VerifyHash(info.Password, account.Password))
            {
                await
                    this.RecordLogin(account, UCenterErrorCode.AccountLoginFailedPasswordNotMatch,
                        "The account name and password do not match");

                throw new UCenterException(UCenterErrorCode.AccountLoginFailedPasswordNotMatch);
            }
            account.LastLoginDateTime = DateTime.UtcNow;
            account.Token = EncryptHashManager.GenerateToken();
            await this.DatabaseContext.Bucket.UpsertSlimAsync(account);
            await this.RecordLogin(account, UCenterErrorCode.Success);

            return CreateSuccessResult(ToResponse<AccountLoginResponse>(account));
        }

        [HttpPost]
        [Route("guest")]
        public async Task<IHttpActionResult> GuestLogin([FromBody] AccountLoginInfo info)
        {
            Logger.Info("Account.GuestLogin");

            var r = new Random();
            string accountNamePostfix = r.Next(0, 1000000).ToString("D6");
            string accountName = $"uc_{DateTime.Now.ToString("yyyyMMddHHmmssffff")}_{accountNamePostfix}";
            string token = EncryptHashManager.GenerateToken();
            string password = Guid.NewGuid().ToString();

            var account = new AccountEntity
            {
                AccountName = accountName,
                IsGuest = true,
                Password = EncryptHashManager.ComputeHash(password),
                Token = EncryptHashManager.GenerateToken(),
                CreatedDateTime = DateTime.UtcNow
            };

            await this.DatabaseContext.Bucket.InsertSlimAsync(account);

            var response = new AccountGuestLoginResponse
            {
                AccountId = account.Id,
                AccountName = accountName,
                Token = token,
                Password = password
            };
            return CreateSuccessResult(response);
        }

        [HttpPost]
        [Route("convert")]
        public async Task<IHttpActionResult> Convert([FromBody] AccountConvertInfo info)
        {
            Logger.Info($"Account.Convert AccountName={info.AccountName}");

            var account = await GetAndVerifyAccount(info.AccountId);

            if (!EncryptHashManager.VerifyHash(info.OldPassword, account.Password))
            {
                await
                    this.RecordLogin(account, UCenterErrorCode.AccountLoginFailedPasswordNotMatch,
                        "The account name and password do not match");
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
            await this.DatabaseContext.Bucket.UpsertSlimAsync(account);
            await this.RecordLogin(account, UCenterErrorCode.Success, "Account converted successfully.");
            return CreateSuccessResult(ToResponse<AccountRegisterResponse>(account));
        }

        [HttpPost]
        [Route("resetpassword")]
        public async Task<IHttpActionResult> ResetPassword([FromBody] AccountResetPasswordInfo info)
        {
            Logger.Info($"Account.ResetPassword AccountName={info.AccountId}");

            var account = await GetAndVerifyAccount(info.AccountId);

            if (!EncryptHashManager.VerifyHash(info.SuperPassword, account.SuperPassword))
            {
                await
                    this.RecordLogin(account, UCenterErrorCode.AccountLoginFailedPasswordNotMatch,
                        "The super password provided is incorrect");
                throw new UCenterException(UCenterErrorCode.AccountLoginFailedPasswordNotMatch);
            }
            account.Password = EncryptHashManager.ComputeHash(info.Password);
            await this.DatabaseContext.Bucket.UpsertSlimAsync(account);
            await this.RecordLogin(account, UCenterErrorCode.Success, "Reset password successfully.");
            return CreateSuccessResult(ToResponse<AccountResetPasswordResponse>(account));
        }

        [HttpPost]
        [Route("upload/{accountId}")]
        public async Task<IHttpActionResult> UploadProfileImage([FromUri] string accountId, CancellationToken token)
        {
            this.Logger.Info($"Account.UploadProfileImage AccountId={accountId}");

            var account = await GetAndVerifyAccount(accountId);

            using (var stream = await this.Request.Content.ReadAsStreamAsync())
            {
                var image = Image.FromStream(stream);
                using (var thumbnailStream = this.GetThumbprintStream(image))
                {
                    var smallProfileName = this.settings.ProfileThumbnailForBlobNameTemplate.FormatInvariant(accountId);
                    account.ProfileThumbnail =
                        await this.storageContext.UploadBlobAsync(smallProfileName, thumbnailStream, token);
                }

                string profileName = this.settings.ProfileImageForBlobNameTemplate.FormatInvariant(accountId);
                stream.Seek(0, SeekOrigin.Begin);
                account.ProfileImage = await this.storageContext.UploadBlobAsync(profileName, stream, token);

                await this.DatabaseContext.Bucket.UpsertSlimAsync(account);
                await this.RecordLogin(account, UCenterErrorCode.Success, "Profile image uploaded successfully.");
                return CreateSuccessResult(ToResponse<AccountUploadProfileImageResponse>(account));
            }
        }

        [HttpGet]
        [Route("test")]
        public async Task<IHttpActionResult> Test(AccountLoginInfo info)
        {
            Logger.Info($"Account.Test");

            var accounts = await this.DatabaseContext.Bucket.QueryAsync<AccountEntity>(a => a.AccountName == "Ny7IBHtK");

            return await Task.FromResult(CreateSuccessResult(accounts));
        }

        private async Task RecordLogin(AccountEntity account, UCenterErrorCode code, string comments = null)
        {
            var record = new LoginRecordEntity
            {
                AccountName = account.AccountName,
                AccountId = account.Id,
                Code = code,
                LoginTime = DateTime.UtcNow,
                UserAgent = Request.Headers.UserAgent.ToString(),
                ClientIp = this.GetClientIp(Request),
                Comments = comments
            };

            await this.DatabaseContext.Bucket.InsertSlimAsync(record, false);
        }

        private async Task<AccountEntity> GetAndVerifyAccount(string accountId)
        {
            var account = await DatabaseContext.Bucket.GetByEntityIdSlimAsync<AccountEntity>(accountId, false);
            if (account == null)
            {
                throw new UCenterException(UCenterErrorCode.AccountNotExist);
            }

            return account;
        }

        private TResponse ToResponse<TResponse>(AccountEntity entity) where TResponse : AccountRequestResponse
        {
            var res = new AccountResponse
            {
                AccountId = entity.Id,
                AccountName = entity.AccountName,
                Password = entity.Password,
                SuperPassword = entity.Password,
                Token = entity.Token,
                LastLoginDateTime = entity.LastLoginDateTime,
                Name = entity.Name,
                ProfileImage = entity.ProfileImage,
                ProfileThumbnail = entity.ProfileThumbnail,
                Sex = entity.Sex,
                IdentityNum = entity.IdentityNum,
                PhoneNum = entity.PhoneNum,
                Email = entity.Email
            };

            var response = Activator.CreateInstance<TResponse>();
            response.ApplyEntity(res);

            return response;
        }

        private Stream GetThumbprintStream(Image sourceImage)
        {
            var stream = new MemoryStream();
            if (sourceImage.Width > this.settings.MaxThumbnailWidth ||
                sourceImage.Height > this.settings.MaxThumbnailHeight)
            {
                var radio = Math.Min((double)this.settings.MaxThumbnailWidth / sourceImage.Width,
                    (double)this.settings.MaxThumbnailHeight / sourceImage.Height);

                var twidth = (int)(sourceImage.Width * radio);
                var theigth = (int)(sourceImage.Height * radio);
                var thumbnail = sourceImage.GetThumbnailImage(twidth, theigth, null, IntPtr.Zero);

                thumbnail.Save(stream, sourceImage.RawFormat);
            }
            else
            {
                sourceImage.Save(stream, sourceImage.RawFormat);
            }

            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }

        private string GetClientIp(HttpRequestMessage request)
        {
            request = request ?? this.Request;

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }
            if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            {
                var prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Request.UserHostAddress;
            }
            return null;
        }
    }
}