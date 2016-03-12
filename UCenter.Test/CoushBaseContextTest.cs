using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCenter.Common.Database.Couch;
using UCenter.Common.Database.Entities;

namespace UCenter.Test
{
    [TestClass]
    public class CoushBaseContextTest
    {
        //[TestMethod]
        //public async Task TestInsertData()
        //{
        //    //AccountEntity account = new AccountEntity()
        //    //{
        //    //    AccountName = GenerateRandomString(),
        //    //    Name = GenerateRandomString()
        //    //};

        //    //CouchBaseBucketContext<AccountEntity> table = ExportProvider.GetExportedValue<CouchBaseBucketContext<AccountEntity>>();
        //    //var result = await table.InsertAsync(account);

        //    //Assert.AreEqual(result.Id, account.Id);
        //}

        //[TestMethod]
        //public void CouchTranslaterTest()
        //{
        //    var translator = new CouchQueryTranslator();

        //    Expression<Func<AccountEntity, bool>> exp = col => col.Id == "abc" && col.AccountName == "name";

        //    var command = translator.Translate(exp);

        //    Assert.IsNotNull(command);
        //}
    }
}
