using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Handler;
using UCenter.Common.Models;
using System.Threading;
using UCenter.Common.Database.Entities;

namespace UCenter.Test
{
    [TestClass]
    public class AccountHandlerTest : UCenterTestBase
    {
        private readonly AccountHandler handler;

        public AccountHandlerTest()
        {
            this.handler = ExportProvider.GetExportedValue<AccountHandler>();
        }

        [TestMethod]
        public async Task AccountHandlerTest_RegisterTest()
        {
            var requestData = new AccountEntity()
            {
                Name = GenerateRandomString(),
                AccountName = GenerateRandomString(),
                Password = GenerateRandomString(),
                IdentityNum = GenerateRandomString(),
                PhoneNum = GenerateRandomString(),
                Sex = Sex.Female
            };

            var response = await handler.RegisterAsync(requestData, CancellationToken.None);
            Assert.IsNotNull(response);
        }
    }
}
