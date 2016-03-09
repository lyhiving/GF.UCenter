using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCenter.Common.Database.Couch;
using UCenter.Common.Database.Entities;

namespace UCenter.Test
{
    [TestClass]
    public class CoushBaseContextTest : UCenterTestBase
    {
        [TestMethod]
        public async Task TestInsertData()
        {
            AccountEntity account = new AccountEntity()
            {
                AccountName = GenerateRandomString(),
                Name = GenerateRandomString()
            };

            CouchBaseTable<AccountEntity> table = ExportProvider.GetExportedValue<CouchBaseTable<AccountEntity>>();
            var result = await table.InsertAsync(account);

            Assert.AreEqual(result.Id, account.Id);
        }
    }
}
