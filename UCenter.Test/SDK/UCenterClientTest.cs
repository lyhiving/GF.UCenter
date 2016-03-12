using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
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
                AppSecret = "767c71c5-1bc5-4323-9e46-03a6a55c6ab1"
            };
            var result = await client.AppLoginAsync(info);
            Assert.IsNotNull(result.Token);
        }

        [TestMethod]
        public async Task SDK_AppVerifyAsyncAccountAsyncTest()
        {
            var info = new AccountAppVerificationInfo()
            {
                AppId = "texaspoker",
                AppSecret = "767c71c5-1bc5-4323-9e46-03a6a55c6ab1",
                AccountId = "10",
                AccountName = "TESTUSER0001",
                AccountToken = "295f9f2d-afc4-475b-b1bb-5c54306f69dd",
                GetAppData = false,
            };
            var result = await client.AppVerifyAccountAsync(info);
            Assert.IsNotNull(result.Token);
            Assert.IsNotNull(result.LastLoginDateTime);
        }

        [TestMethod]
        public async Task SDK_AppWriteDataAsyncTest()
        {

        }

        [TestMethod]
        public async Task SDK_AppReadDataAsyncTest()
        {

        }
    }
}
