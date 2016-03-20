using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCenter.Common.Portable;
using UCenter.Common;

namespace UCenter.Test.SDK
{
    [TestClass]
    public class UCenterSDKAppServerTest : UCenterSDKTestBase
    {
        protected UCenter.SDK.AppClient.UCenterClient cClient;
        protected UCenter.SDK.AppServer.UCenterClient sClient;

        private readonly string host;

        public UCenterSDKAppServerTest()
        {
            this.host = "http://localhost:8888/";
            this.cClient = new UCenter.SDK.AppClient.UCenterClient(host);
            this.sClient = new UCenter.SDK.AppServer.UCenterClient(host);
        }

        [TestMethod]
        public async Task SDK_App_Login_Test()
        {
            var info = new AppLoginInfo()
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
            };
            var result = await sClient.AppLoginAsync(info);
            Assert.AreEqual(info.AppId, result.AppId);
            Assert.AreEqual(info.AppSecret, result.AppSecret);
            Assert.IsNotNull(result.Token);
        }

        [TestMethod]
        public async Task SDK_App_VerifyAccount_Test()
        {
            var registerInfo = new AccountRegisterInfo()
            {
                Name = GenerateRandomString(),
                AccountName = GenerateRandomString(),
                Password = ValidPassword,
                SuperPassword = ValidPassword,
                IdentityNum = GenerateRandomString(),
                PhoneNum = GenerateRandomString(),
                Sex = Sex.Female
            };

            await cClient.AccountRegisterAsync(registerInfo);

            await Task.Delay(1000);

            var loginResponse = await cClient.AccountLoginAsync(new AccountLoginInfo()
            {
                AccountName = registerInfo.AccountName,
                Password = registerInfo.Password
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
        public async Task SDK_App_ReadData_And_WriteData_Test()
        {
            var registerInfo = new AccountRegisterInfo()
            {
                Name = GenerateRandomString(),
                AccountName = GenerateRandomString(),
                Password = ValidPassword,
                SuperPassword = ValidPassword,
                IdentityNum = GenerateRandomString(),
                PhoneNum = GenerateRandomString(),
                Sex = Sex.Female
            };

            await cClient.AccountRegisterAsync(registerInfo);

            await Task.Delay(1000);

            var loginResponse = await cClient.AccountLoginAsync(new AccountLoginInfo()
            {
                AccountName = registerInfo.AccountName,
                Password = registerInfo.Password
            });

            string data = @"{ 'id': 1, 'name': 'abc'}";
            var appData = new AppDataInfo()
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                AccountId = loginResponse.AccountId,
                Data = data
            };

            await sClient.AppWriteDataAsync(appData);

            await Task.Delay(1000);

            var result = await sClient.AppReadDataAsync(appData);

            Assert.AreEqual(appData.AppId, result.AppId);
            Assert.AreEqual(appData.AccountId, result.AccountId);
            Assert.AreEqual(appData.Data, result.Data);

        }

        [TestMethod]
        public async Task SDK_Create_Charge_Test()
        {
            var chargeInfo = new ChargeInfo()
            {
                OrderNo = DateTime.Now.ToString("yyyyMMddHHmmssffff"),
                Channel = "alipay",
                Amount = 100,
                Subject = "fake item"
            };

            var result = await sClient.CreateChargeAsync(chargeInfo);
            Assert.AreEqual(chargeInfo.Amount, result.Amount);
        }

        [TestInitialize]
        public void Initialize()
        {
            // use public async void Initialize() will never triggered
            InitlizazeAsync();
        }

        private async void InitlizazeAsync()
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
