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
using UCenter.SDK.AppClient;

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
                AccountName = GenerateRandomString(),
                Password = ValidPassword,
                SuperPassword = ValidPassword,
                Name = GenerateRandomString(),
                IdentityNum = GenerateRandomString(),
                PhoneNum = GenerateRandomString(),
                Sex = Sex.Female
            };

            var registerResponse = await client.AccountRegisterAsync(info);
            Assert.IsNotNull(registerResponse.AccountId);
            Assert.AreEqual(registerResponse.AccountName, info.AccountName);
            Assert.AreEqual(registerResponse.IdentityNum, info.IdentityNum);
            Assert.AreEqual(registerResponse.Name, info.Name);
            Assert.AreEqual(registerResponse.PhoneNum, info.PhoneNum);
            Assert.AreEqual(registerResponse.Sex, info.Sex);

            await Task.Delay(1000);

            var loginResponse = await client.AccountLoginAsync(new AccountLoginInfo()
            {
                AccountName = registerResponse.AccountName,
                Password = info.Password
            });

            Assert.IsNotNull(loginResponse.AccountId);
            Assert.AreEqual(loginResponse.AccountName, info.AccountName);
            Assert.AreEqual(loginResponse.IdentityNum, info.IdentityNum);
            Assert.AreEqual(loginResponse.Name, info.Name);
            Assert.AreEqual(loginResponse.PhoneNum, info.PhoneNum);
            Assert.AreEqual(loginResponse.Sex, info.Sex);
            Assert.IsNotNull(loginResponse.LastLoginDateTime);
            Assert.IsNotNull(loginResponse.Token);
        }

        [TestMethod]
        public async Task SDK_Account_Login_Incorrect_Password_Test()
        {
            try
            {
                var info = new AccountRegisterInfo()
                {
                    AccountName = GenerateRandomString(),
                    Password = ValidPassword,
                    SuperPassword = ValidPassword,
                    Name = GenerateRandomString(),
                    IdentityNum = GenerateRandomString(),
                    PhoneNum = GenerateRandomString(),
                    Sex = Sex.Female
                };

                var registerResponse = await client.AccountRegisterAsync(info);

                await Task.Delay(1000);

                await client.AccountLoginAsync(new AccountLoginInfo()
                {
                    AccountName = registerResponse.AccountName,
                    Password = ""
                });
            }
            catch (UCenterException ex)
            {
                Assert.AreEqual(ex.ErrorCode, UCenterErrorCode.AccountLoginFailedNotMatch);
            }
        }

        [TestMethod]
        public async Task SDK_Account_Register_Twice_Test()
        {
            try
            {
                var info = new AccountRegisterInfo()
                {
                    AccountName = GenerateRandomString(),
                    Password = ValidPassword,
                    SuperPassword = ValidPassword,
                    Name = GenerateRandomString(),
                    IdentityNum = GenerateRandomString(),
                    PhoneNum = GenerateRandomString(),
                    Sex = Sex.Female
                };

                await client.AccountRegisterAsync(info);

                await Task.Delay(1000);

                await client.AccountRegisterAsync(info);
                Assert.Fail();
            }
            catch (UCenterException ex)
            {
                Assert.AreEqual(ex.ErrorCode, UCenterErrorCode.AccountRegisterFailedAlreadyExist);
            }
        }

        [TestMethod]
        public async Task SDK_Account_Guest_Login_And_Convert_Test()
        {
            var loginResponse = await client.AccountGuestLoginAsync();

            Assert.IsNotNull(loginResponse.AccountId);
            Assert.IsNotNull(loginResponse.AccountName);
            Assert.IsNotNull(loginResponse.Password);
            Assert.IsNotNull(loginResponse.Token);

            var convertInfo = new AccountConvertInfo()
            {
                AccountId = loginResponse.AccountId,
                AccountName = GenerateRandomString(),
                OldPassword = loginResponse.Password,
                Password = ValidPassword,
                SuperPassword = ValidPassword,
                Name = GenerateRandomString(),
                PhoneNum = GenerateRandomString(),
                IdentityNum = GenerateRandomString(),
                Sex = Sex.Female
            };

            await Task.Delay(1000);

            var convertResponse = await client.AccountConvertAsync(convertInfo);

            Assert.IsNotNull(convertResponse.AccountId);
            Assert.IsNotNull(convertResponse.AccountId, convertInfo.AccountId);
            Assert.AreEqual(convertResponse.AccountName, convertInfo.AccountName);
            Assert.AreEqual(convertResponse.IdentityNum, convertInfo.IdentityNum);
            Assert.AreEqual(convertResponse.Name, convertInfo.Name);
            Assert.AreEqual(convertResponse.PhoneNum, convertInfo.PhoneNum);
            Assert.AreEqual(convertResponse.Sex, convertInfo.Sex);
        }

        [TestMethod]
        public async Task SDK_Account_Reset_Password_Test()
        {
            var registerInfo = new AccountRegisterInfo()
            {
                AccountName = GenerateRandomString(),
                Password = ValidPassword,
                SuperPassword = ValidPassword,
                Name = GenerateRandomString(),
                IdentityNum = GenerateRandomString(),
                PhoneNum = GenerateRandomString(),
                Sex = Sex.Female
            };

            var registerResponse = await client.AccountRegisterAsync(registerInfo);

            await Task.Delay(1000);

            var resetInfo = new AccountResetPasswordInfo()
            {
                AccountId = registerResponse.AccountId,
                Password = "123456",
                SuperPassword = ValidPassword
            };

            var resetPasswordResponse = await client.AccountResetPasswordAsync(resetInfo);

            var loginInfo = new AccountLoginInfo()
            {
                AccountName = registerResponse.AccountName,
                Password = ValidPassword
            };

            await Task.Delay(1000);

            try
            {
                var response = await client.AccountLoginAsync(loginInfo);
            }
            // todo: 
            catch (Exception ex)
            {
                //ex.ErrorCode = UCenterError.
            }
        }
    }
}
