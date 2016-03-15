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
using UCenter.SDK;

namespace UCenter.Test.SDK
{
    [TestClass]
    public class UCenterClientTest : UCenterTestBase
    {
        private readonly string host;
        private readonly UCenterClient client;

        public UCenterClientTest()
        {
            this.host = "http://localhost:8888/";
            this.client = new UCenterClient(host);
        }

        [TestMethod]
        public async Task SDK_Account_Register_And_Login_Test()
        {
            var info = new AccountRegisterInfo()
            {
                Name = GenerateRandomString(),
                AccountName = GenerateRandomString(),
                Password = ValidPassword,
                SuperPassword = ValidPassword,
                IdentityNum = GenerateRandomString(),
                PhoneNum = GenerateRandomString(),
                Sex = Sex.Female
            };

            var registerResponse = await client.AccountRegisterAsync(info);
            Assert.AreEqual(registerResponse.AccountName, info.AccountName);
            Assert.AreEqual(registerResponse.IdentityNum, info.IdentityNum);
            Assert.AreEqual(registerResponse.Name, info.Name);
            Assert.AreEqual(registerResponse.PhoneNum, info.PhoneNum);
            Assert.AreEqual(registerResponse.Sex, info.Sex);

            await Task.Delay(1000);

            var loginResponse = await client.AccountLoginAsync(new AccountLoginInfo()
            {
                AccountName = info.AccountName,
                Password = info.Password
            });

            Assert.AreEqual(loginResponse.AccountName, info.AccountName);
            Assert.AreEqual(loginResponse.IdentityNum, info.IdentityNum);
            Assert.AreEqual(loginResponse.Name, info.Name);
            Assert.AreEqual(loginResponse.PhoneNum, info.PhoneNum);
            Assert.AreEqual(loginResponse.Sex, info.Sex);
            Assert.IsNotNull(loginResponse.LastLoginDateTime);
            Assert.IsNotNull(loginResponse.Token);
        }

        [TestMethod]
        public async Task SDK_App_Login_Test()
        {
            var info = new AppLoginInfo()
            {
                AppId = "texaspoker",
                AppSecret = "767c71c5-1bc5-4323-9e46-03a6a55c6ab1",
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
                AppId = "texaspoker",
                AppSecret = "767c71c5-1bc5-4323-9e46-03a6a55c6ab1",
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
                AppId = "texaspoker",
                AppSecret = "767c71c5-1bc5-4323-9e46-03a6a55c6ab1",
                AccountName = loginResponse.AccountName,
                Data = data
            };

            await client.AppWriteDataAsync(appData);
            var result = await client.AppReadDataAsync(appData);

            Assert.AreEqual(appData.AppId, result.AppId);
            Assert.AreEqual(appData.AccountName, result.AccountName);
            Assert.AreEqual(appData.Data, result.Data);

        }
    }
}
