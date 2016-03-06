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
        public async Task SDK_Account_Register_Test()
        {
            var info = new AccountRegisterInfo()
            {
                Name = GenerateRandomString(),
                AccountName = GenerateRandomString(),
                Password = GenerateRandomString(),
                IdentityNum = GenerateRandomString(),
                PhoneNum = GenerateRandomString(),
                Sex = Sex.Female
            };
            var accountEntity = await client.AccountRegisterAsync(info);
            Assert.IsNotNull(accountEntity.AccountName);
            Assert.IsNotNull(accountEntity.Name);
            Assert.IsNotNull(accountEntity.Name);
        }

        [TestMethod]
        public async Task SDK_Account_Login_Test()
        {
            string accountName = "TESTUSER0001";
            string password = "123456";

            var registerInfo = new AccountRegisterInfo()
            {
                AccountName = accountName,
                Password = password,
                Name = GenerateRandomString(),
                IdentityNum = GenerateRandomString(),
                PhoneNum = GenerateRandomString(),
                Sex = Sex.Female
            };
            // TODO: Fix error apicontroller can not be created twice
            //var accountEntity = await client.AccountRegisterAsync(registerInfo);

            var loginInfo = new AccountLoginInfo()
            {
                AccountName = accountName,
                Password = password
            };
            var result = await client.AccountLoginAsync(loginInfo);
            Assert.IsNotNull(result.Token);
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
                AccountId = 24,
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
