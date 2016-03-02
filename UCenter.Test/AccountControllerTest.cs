using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Models;

namespace UCenter.Test
{
    [TestClass]
    public class AccountControllerTest : UCenterTestBase
    {
        private readonly AccountControllerClient client = ExportProvider.GetExportedValue<AccountControllerClient>();

        [TestMethod]
        public async Task AccountControllerTest_Login_Test()
        {
            var request = new AccountLoginRequest();
            request.acc = "test1000";
            request.pwd = "123456";
            var response = await this.client.LoginAsync(request);

            Assert.IsNotNull(response);
        }
    }
}
