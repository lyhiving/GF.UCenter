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
    public class UCenterSDKAppClientTest : UCenterSDKTestBase
    {
        protected UCenterClient client;
        private readonly string host;

        public UCenterSDKAppClientTest()
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
    }
}
