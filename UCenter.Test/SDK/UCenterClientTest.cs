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
        public async Task SDK_AccountRegisterAsyncTest()
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
            var result = await client.AccountRegisterAsync(info);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task SDK_AccountLoginAsyncTest()
        {
            var info = new AccountLoginInfo()
            {
                AccountName = "test1000",
                Password = "123456"
            };
            var result = await client.AccountLoginAsync(info);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task SDK_AppLoginAsyncTest()
        {
            var info = new AppLoginInfo()
            {
                AppId = "texaspoker",
                AppSecret = "767c71c5-1bc5-4323-9e46-03a6a55c6ab1"
            };
            var result = await client.AppLoginAsync(info);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task SDK_AppVerifyAsyncAccountAsyncTest()
        {
            var info = new AppAccountVerificationInfo()
            {
                AppId = "",
                AppKey = "",
                AccountId = 1,
                AccountName = "",
                AccountToken = "",
                GetAppData = false,
            };
            var result = await client.AppVerifyAccountAsync(info);
            Assert.IsNotNull(result);
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
