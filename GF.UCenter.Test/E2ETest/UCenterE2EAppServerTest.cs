namespace GF.UCenter.Test.E2ETest
{
    using System.Threading.Tasks;
    using Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using UCenter.Common.Models.AppServer;
    using UCenter.Common.Models.PingPlusPlus;
    using UCenter.Common.Portable.Contracts;
    using UCenter.Common.Portable.Models.AppClient;

    [TestClass]
    public class UCenterE2EAppServerTest : UCenterE2ETestBase
    {
        [TestMethod]
        public async Task E2E_AppServer_VerifyAccount_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await cClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var appVerifyAccountInfo = new AppVerifyAccountInfo
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                AccountId = loginResponse.AccountId,
                AccountToken = loginResponse.Token
            };
            var result = await sClient.AppVerifyAccountAsync(appVerifyAccountInfo);
            Assert.IsNotNull(result.AccountId);
            Assert.IsNotNull(result.AccountName);
            Assert.IsNotNull(result.AccountToken);
            Assert.IsNotNull(result.LastLoginDateTime);
            Assert.IsNotNull(result.LastVerifyDateTime);
        }

        [TestMethod]
        public void E2E_AppServer_VerifyAccount_AccountNotExist_Test()
        {
            var appVerifyAccountInfo = new AppVerifyAccountInfo
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                AccountId = "___Test_Account_No_Exist___",
                AccountToken = ValidAccountPassword
            };

            TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AccountNotExist,
                async () => { await sClient.AppVerifyAccountAsync(appVerifyAccountInfo); });
        }

        [TestMethod]
        public async Task E2E_AppServer_VerifyAccount_AppNotExist_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await cClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var appVerifyAccountInfo = new AppVerifyAccountInfo
            {
                AppId = "___Test_App_No_Exist___",
                AppSecret = TestAppSecret,
                AccountId = registerResponse.AccountId,
                AccountToken = loginResponse.Token
            };

            TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AppNotExit,
                async () => { await sClient.AppVerifyAccountAsync(appVerifyAccountInfo); });
        }

        [TestMethod]
        public async Task E2E_AppServer_VerifyAccount_IncorrectAppSecret_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await cClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var appVerifyAccountInfo = new AppVerifyAccountInfo
            {
                AppId = TestAppId,
                AppSecret = InvalidAppSecret,
                AccountId = loginResponse.AccountId,
                AccountToken = ValidAccountPassword
            };

            TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AppAuthFailedSecretNotMatch,
                async () => { await sClient.AppVerifyAccountAsync(appVerifyAccountInfo); });
        }

        [TestMethod]
        public async Task E2E_AppServer_VerifyAccount_IncorrectAccountToken_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await cClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var appVerifyAccountInfo = new AppVerifyAccountInfo
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                AccountId = loginResponse.AccountId,
                AccountToken = InValidAccountToken
            };

            TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AccountLoginFailedTokenNotMatch,
                async () => { await sClient.AppVerifyAccountAsync(appVerifyAccountInfo); });
        }

        [TestMethod]
        public async Task E2E_AppServer_ReadAccountData_And_WriteAccountData_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await cClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var data = @"{ 'id': 1, 'name': 'abc'}";
            var accountData = new AppAccountDataInfo
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                AccountId = loginResponse.AccountId,
                Data = data
            };

            await sClient.AppWriteAccountDataAsync(accountData);

            var result = await sClient.AppReadAccountDataAsync(accountData);

            Assert.AreEqual(accountData.AppId, result.AppId);
            Assert.AreEqual(accountData.AccountId, result.AccountId);
            Assert.AreEqual(accountData.Data, result.Data);
        }

        [TestMethod]
        public async Task E2E_AppServer_ReadAccountData_IncorrectAppSecret_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await cClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var accountData = new AppAccountDataInfo
            {
                AppId = TestAppId,
                AppSecret = InvalidAppSecret,
                AccountId = loginResponse.AccountId
            };

            TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AppAuthFailedSecretNotMatch,
                async () => { await sClient.AppReadAccountDataAsync(accountData); });
        }

        [TestMethod]
        public async Task E2E_AppServer_WriteAccountData_InvalidAppSecret_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await cClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var data = @"{ 'id': 1, 'name': 'abc'}";
            var accountData = new AppAccountDataInfo
            {
                AppId = TestAppId,
                AppSecret = InvalidAppSecret,
                AccountId = loginResponse.AccountId,
                Data = data
            };

            TestExpector.ExpectUCenterErrorAsync(UCenterErrorCode.AppAuthFailedSecretNotMatch,
                async () => { await sClient.AppWriteAccountDataAsync(accountData); });
        }

        [TestMethod]
        public async Task E2E_AppServer_Create_Charge_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await cClient.AccountLoginAsync(new AccountLoginInfo
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            var chargeInfo = new ChargeInfo
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                AccountId = loginResponse.AccountId,
                Amount = 100.5,
                Subject = "Super Axe",
                Body = "Test body",
                ClientIp = "1.2.3.4",
                Description = "This is a test order created by unit test"
            };

            var result = await sClient.CreateChargeAsync(chargeInfo);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Amount, chargeInfo.Amount);
            Assert.AreEqual(result.Subject, chargeInfo.Subject);
            Assert.AreEqual(result.Body, chargeInfo.Body);
            Assert.AreEqual(result.Description, chargeInfo.Description);
            Assert.IsNotNull(result.OrderNo);
            Assert.IsNotNull(result.TransactionNo);
        }

        [TestInitialize]
        public void Initialize()
        {
            // use public async void Initialize() will never triggered
            this.InitializeAsync().Wait();
        }

        private async Task InitializeAsync()
        {
            var appInfo = new AppInfo
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret
            };

            await sClient.AppCreateAsync(appInfo);
        }
    }
}