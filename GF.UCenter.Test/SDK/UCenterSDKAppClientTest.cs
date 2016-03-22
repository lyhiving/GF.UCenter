using System;
using System.Threading.Tasks;
using GF.UCenter.Common.Portable;
using GF.UCenter.SDK.AppClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GF.UCenter.Test
{
    [TestClass]
    public class UCenterSDKAppClientTest : UCenterSDKTestBase
    {

        [TestMethod]
        public async Task SDK_Account_Register_And_Login_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await cClient.AccountLoginAsync(new AccountLoginInfo()
            {
                AccountName = registerResponse.AccountName,
                Password = ValidPassword
            });

            Assert.IsNotNull(loginResponse.AccountId);
            Assert.AreEqual(loginResponse.AccountName, registerResponse.AccountName);
            Assert.AreEqual(loginResponse.IdentityNum, registerResponse.IdentityNum);
            Assert.AreEqual(loginResponse.Name, registerResponse.Name);
            Assert.AreEqual(loginResponse.PhoneNum, registerResponse.PhoneNum);
            Assert.AreEqual(loginResponse.Sex, registerResponse.Sex);
            Assert.IsNotNull(loginResponse.LastLoginDateTime);
            Assert.IsNotNull(loginResponse.Token);
        }

        [TestMethod]
        public async Task SDK_Account_Login_Incorrect_Password_Test()
        {
            try
            {
                var registerResponse = await CreateTestAccount();

                await Task.Delay(1000);

                await cClient.AccountLoginAsync(new AccountLoginInfo()
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

                await cClient.AccountRegisterAsync(info);

                await Task.Delay(1000);

                await cClient.AccountRegisterAsync(info);
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
            var loginResponse = await cClient.AccountGuestLoginAsync();

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

            var convertResponse = await cClient.AccountConvertAsync(convertInfo);

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
            var registerResponse = await CreateTestAccount();

            await Task.Delay(1000);

            var resetInfo = new AccountResetPasswordInfo()
            {
                AccountId = registerResponse.AccountId,
                Password = "123456",
                SuperPassword = ValidPassword
            };

            var resetPasswordResponse = await cClient.AccountResetPasswordAsync(resetInfo);

            var loginInfo = new AccountLoginInfo()
            {
                AccountName = registerResponse.AccountName,
                Password = ValidPassword
            };

            await Task.Delay(1000);

            try
            {
                await cClient.AccountLoginAsync(loginInfo);
                Assert.Fail();
            }
            catch (UCenterException ex)
            {
                Assert.AreEqual(ex.ErrorCode, UCenterErrorCode.AccountLoginFailedNotMatch);
            }
        }
    }
}
