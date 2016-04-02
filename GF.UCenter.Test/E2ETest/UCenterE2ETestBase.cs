namespace GF.UCenter.Test
{
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SDK.AppClient;
    using UCenter.Common.Portable;

    [TestClass]
    public class UCenterE2ETestBase : UCenterTestBase
    {
        protected const string TestAppId = "utapp";
        protected const string TestAppSecret = "#pA554&3321#";
        protected const string InvalidAppSecret = "";
        protected const string ValidAccountPassword = "#pA554&3321#";
        protected const string InValidAccountPassword = "";
        protected const string InValidAccountToken = "";

        protected readonly string host;
        protected UCenterClient cClient;
        protected SDK.AppServer.UCenterClient sClient;

        public UCenterE2ETestBase()
        {
            this.host = "http://localhost:8888/";
            this.cClient = new UCenterClient(host);
            this.sClient = new SDK.AppServer.UCenterClient(host);
        }

        protected async Task<AccountRegisterResponse> CreateTestAccount(AccountRegisterInfo info = null)
        {
            if (info == null)
            {
                info = new AccountRegisterInfo
                {
                    AccountName = GenerateRandomString(),
                    Password = ValidAccountPassword,
                    SuperPassword = ValidAccountPassword,
                    Name = GenerateRandomString(),
                    IdentityNum = GenerateRandomString(),
                    PhoneNum = GenerateRandomString(),
                    Sex = Sex.Female
                };
            }
            var registerResponse = await cClient.AccountRegisterAsync(info);
            Assert.IsNotNull(registerResponse.AccountId);
            Assert.AreEqual(registerResponse.AccountName, info.AccountName);
            Assert.AreEqual(registerResponse.IdentityNum, info.IdentityNum);
            Assert.AreEqual(registerResponse.Name, info.Name);
            Assert.AreEqual(registerResponse.PhoneNum, info.PhoneNum);
            Assert.AreEqual(registerResponse.Sex, info.Sex);
            Assert.IsNotNull(registerResponse.ProfileImage);
            Assert.IsNotNull(registerResponse.ProfileThumbnail);

            return registerResponse;
        }
    }
}