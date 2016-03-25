using System.Threading.Tasks;
using System.Threading;
using GF.UCenter.Common.Portable;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GF.UCenter.Test
{
    [TestClass]
    public class UCenterSDKAppServerTest : UCenterSDKTestBase
    {
        [TestMethod]
        public async Task SDK_AppServer_VerifyAccount_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await cClient.AccountLoginAsync(new AccountLoginInfo()
            {
                AccountName = registerResponse.AccountName,
                Password = ValidPassword
            });

            var appVerifyAccountInfo = new AppVerifyAccountInfo()
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
        public async Task SDK_AppServer_ReadAccountData_And_WriteAccountData_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await cClient.AccountLoginAsync(new AccountLoginInfo()
            {
                AccountName = registerResponse.AccountName,
                Password = ValidPassword
            });

            string data = @"{ 'id': 1, 'name': 'abc'}";
            var accountData = new AppAccountDataInfo()
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                AccountId = loginResponse.AccountId,
                Data = null
            };

            await sClient.AppWriteAccountDataAsync(accountData);

            var result = await sClient.AppReadAccountDataAsync(accountData);

            Assert.AreEqual(accountData.AppId, result.AppId);
            Assert.AreEqual(accountData.AccountId, result.AccountId);
            Assert.AreEqual(accountData.Data, result.Data);
        }

        [TestMethod]
        public async Task SDK_AppServer_Create_Charge_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await cClient.AccountLoginAsync(new AccountLoginInfo()
            {
                AccountName = registerResponse.AccountName,
                Password = ValidPassword
            });

            var chargeInfo = new ChargeInfo()
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
            this.InitlizazeAsync().Wait();
        }

        private async Task InitlizazeAsync()
        {
            var appInfo = new AppInfo()
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret
            };

            await sClient.AppCreateAsync(appInfo);
        }
    }
}
