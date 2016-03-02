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
    public class AppControllerTest : UCenterTestBase
    {
        private readonly AppControllerClient client = ExportProvider.GetExportedValue<AppControllerClient>();

        [TestMethod]
        public async Task AppControllerTest_Login_Test()
        {
            var request = new AppLoginRequest();
            request.app_id = "testapp";
            request.data = "data";

            var response = await this.client.LoginAsync(request);

            Assert.IsNotNull(response);
        }
    }
}
