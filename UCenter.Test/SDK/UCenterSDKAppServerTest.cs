using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCenter.Common.Models;
using UCenter.Common.SDK;

namespace UCenter.Test.SDK
{
    [TestClass]
    public class UCenterSDKAppServerTest : UCenterSDKTestBase
    {
        protected UCenterClient client;
        private readonly string host;

        public UCenterSDKAppServerTest()
        {
            this.host = "http://localhost:8888/";
            this.client = new UCenterClient(host);
        }

        [TestMethod]
        public async Task SDK_App_Login_Test()
        {
            var info = new AppLoginInfo()
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
            };
            var result = await client.AppLoginAsync(info);
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

            await client.AccountRegisterAsync(registerInfo);

            await Task.Delay(1000);

            var loginResponse = await client.AccountLoginAsync(new AccountLoginInfo()
            {
                AccountName = registerInfo.AccountName,
                Password = registerInfo.Password
            });

            var accountVerificationInfo = new AccountVerificationInfo()
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                AccountName = loginResponse.AccountName,
                AccountToken = loginResponse.Token
            };
            var result = await client.AppVerifyAccountAsync(accountVerificationInfo);

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

            await client.AccountRegisterAsync(registerInfo);

            await Task.Delay(1000);

            var loginResponse = await client.AccountLoginAsync(new AccountLoginInfo()
            {
                AccountName = registerInfo.AccountName,
                Password = registerInfo.Password
            });

            string data = @"{ 'id': 1, 'name': 'abc'}";
            var appData = new AppDataInfo()
            {
                AppId = TestAppId,
                AppSecret = TestAppSecret,
                AccountName = loginResponse.AccountName,
                Data = data
            };

            await client.AppWriteDataAsync(appData);

            await Task.Delay(1000);

            var result = await client.AppReadDataAsync(appData);

            Assert.AreEqual(appData.AppId, result.AppId);
            Assert.AreEqual(appData.AccountName, result.AccountName);
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

            var result = await client.CreateChargeAsync(chargeInfo);
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

            await client.AppCreateAsync(appInfo);
        }
    }
}
