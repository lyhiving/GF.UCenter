using System;
using System.IO;
using System.Threading.Tasks;
using GF.UCenter.Common.Portable;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GF.UCenter.Test
{
    [TestClass]
    public class UCenterSDKAppClientTest : UCenterSDKTestBase
    {
        [TestMethod]
        public async Task SDK_AppClient_Register_And_Login_Test()
        {
            var registerResponse = await CreateTestAccount();

            var loginResponse = await cClient.AccountLoginAsync(new AccountLoginInfo()
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            });

            Assert.IsNotNull(loginResponse.AccountId);
            Assert.AreEqual(loginResponse.AccountName, registerResponse.AccountName);
            Assert.AreEqual(loginResponse.IdentityNum, registerResponse.IdentityNum);
            Assert.AreEqual(loginResponse.Name, registerResponse.Name);
            Assert.AreEqual(loginResponse.PhoneNum, registerResponse.PhoneNum);
            Assert.AreEqual(loginResponse.Email, registerResponse.Email);
            Assert.AreEqual(loginResponse.Sex, registerResponse.Sex);
            Assert.IsNotNull(loginResponse.LastLoginDateTime);
            Assert.IsNotNull(loginResponse.Token);
        }

        [TestMethod]
        [UCExpectedException(UCenterErrorCode.AccountLoginFailedPasswordNotMatch)]
        public async Task SDK_AppClient_Login_Incorrect_Password_Test()
        {
            var registerResponse = await CreateTestAccount();

            await cClient.AccountLoginAsync(new AccountLoginInfo()
            {
                AccountName = registerResponse.AccountName,
                Password = InValidAccountPassword
            });
        }

        [TestMethod]
        [UCExpectedException(UCenterErrorCode.AccountRegisterFailedAlreadyExist)]
        public async Task SDK_AppClient_Register_Twice_Test()
        {
            var info = new AccountRegisterInfo()
            {
                AccountName = GenerateRandomString(),
                Password = ValidAccountPassword,
                SuperPassword = ValidAccountPassword,
                Name = GenerateRandomString(),
                IdentityNum = GenerateRandomString(),
                PhoneNum = GenerateRandomString(),
                Email = GenerateRandomString(),
                Sex = Sex.Female
            };

            await cClient.AccountRegisterAsync(info);

            await cClient.AccountRegisterAsync(info);
        }

        [TestMethod]
        public async Task SDK_AppClient_Guest_Login_And_Convert_Test()
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
                Password = ValidAccountPassword,
                SuperPassword = ValidAccountPassword,
                Name = GenerateRandomString(),
                PhoneNum = GenerateRandomString(),
                Email = GenerateRandomString(),
                IdentityNum = GenerateRandomString(),
                Sex = Sex.Female
            };

            var convertResponse = await cClient.AccountConvertAsync(convertInfo);

            Assert.IsNotNull(convertResponse.AccountId);
            Assert.IsNotNull(convertResponse.Token);
            Assert.IsNotNull(convertResponse.AccountId, convertInfo.AccountId);
            Assert.AreEqual(convertResponse.AccountName, convertInfo.AccountName);
            Assert.AreEqual(convertResponse.IdentityNum, convertInfo.IdentityNum);
            Assert.AreEqual(convertResponse.Name, convertInfo.Name);
            Assert.AreEqual(convertResponse.PhoneNum, convertInfo.PhoneNum);
            Assert.AreEqual(convertResponse.Email, convertInfo.Email);
            Assert.AreEqual(convertResponse.Sex, convertInfo.Sex);
        }

        [TestMethod]
        [UCExpectedException(UCenterErrorCode.AccountLoginFailedPasswordNotMatch)]
        public async Task SDK_AppClient_Reset_Password_Test()
        {
            var registerResponse = await CreateTestAccount();

            var resetInfo = new AccountResetPasswordInfo()
            {
                AccountId = registerResponse.AccountId,
                Password = "123456",
                SuperPassword = ValidAccountPassword
            };

            var resetPasswordResponse = await cClient.AccountResetPasswordAsync(resetInfo);

            var loginInfo = new AccountLoginInfo()
            {
                AccountName = registerResponse.AccountName,
                Password = ValidAccountPassword
            };

            await cClient.AccountLoginAsync(loginInfo);
        }

        [TestMethod]
        public async Task SDK_AppClient_Upload_Profile_Image_By_Path_Test()
        {
            var registerResponse = await CreateTestAccount();

            string testFileForUpload = @"TestData\github.png";
            var uploadProfileResponse = await cClient.AccountUploadProfileImagesync(registerResponse.AccountId, testFileForUpload);
            Assert.AreEqual(registerResponse.AccountId, uploadProfileResponse.AccountId);
            Assert.AreEqual(registerResponse.AccountName, uploadProfileResponse.AccountName);
            Assert.AreEqual(registerResponse.Email, uploadProfileResponse.Email);
            Assert.AreEqual(registerResponse.IdentityNum, uploadProfileResponse.IdentityNum);
            Assert.AreEqual(registerResponse.Name, uploadProfileResponse.Name);
            Assert.AreEqual(registerResponse.PhoneNum, uploadProfileResponse.PhoneNum);
            Assert.AreEqual(registerResponse.Sex, uploadProfileResponse.Sex);
            Assert.IsNotNull(uploadProfileResponse.ProfileImage);
        }

        [TestMethod]
        public async Task SDK_AppClient_Upload_Profile_Image_By_Stream_Test()
        {
            var registerResponse = await CreateTestAccount();

            string testFileForUpload = @"TestData\github.png";
            using (var stream = new FileStream(testFileForUpload, FileMode.Open))
            {
                var uploadProfileResponse = await cClient.AccountUploadProfileImagesync(registerResponse.AccountId, stream);
                Assert.AreEqual(registerResponse.AccountId, uploadProfileResponse.AccountId);
                Assert.AreEqual(registerResponse.AccountName, uploadProfileResponse.AccountName);
                Assert.AreEqual(registerResponse.Email, uploadProfileResponse.Email);
                Assert.AreEqual(registerResponse.IdentityNum, uploadProfileResponse.IdentityNum);
                Assert.AreEqual(registerResponse.Name, uploadProfileResponse.Name);
                Assert.AreEqual(registerResponse.PhoneNum, uploadProfileResponse.PhoneNum);
                Assert.AreEqual(registerResponse.Sex, uploadProfileResponse.Sex);
                Assert.IsNotNull(uploadProfileResponse.ProfileImage);
            }
        }
    }
}
