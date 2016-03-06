using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCenter.Common.Database;
using UCenter.Common.Database.Entities;
using UCenter.Common.Database.MySQL;
using UCenter.Common.Expressions;
using UCenter.Common.Models;

namespace UCenter.Test
{
    [TestClass]
    public class DatabaseModelTest : UCenterTestBase
    {
        [TestMethod]
        public async Task DatabaseModelTest_InsertOrUpdateTest()
        {
            var tableModel = ExportProvider.GetExportedValue<DatabaseTableModel<TestDatabaseEntity>>();
            var entity = new TestDatabaseEntity()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "name1",
                Value = 111
            };

            await tableModel.DeleteAsync(CancellationToken.None);
            await tableModel.CreateIfNotExists(CancellationToken.None);

            await tableModel.InsertOrUpdateAsync(entity, CancellationToken.None);

            var entity2 = await tableModel.RetrieveEntityAsync(entity, CancellationToken.None);
            Assert.IsNotNull(entity2);
            Assert.AreEqual(entity.Id, entity2.Id);
            Assert.AreEqual(entity.Name, entity2.Name);
            Assert.AreEqual(entity.Value, entity2.Value);
        }

        [TestMethod]
        public async Task DatabaseModelTest_RetreveEntities()
        {
            var tableModel = ExportProvider.GetExportedValue<DatabaseTableModel<TestDatabaseEntity>>();
            var entity = new TestDatabaseEntity()
            {
                Id = Guid.NewGuid().ToString(),
                Name = GenerateRandomString(),
                Value = 111
            };

            await tableModel.InsertOrUpdateAsync(entity, CancellationToken.None);
            var entities = await tableModel.RetrieveEntitiesAsync(e => e.Name == entity.Name, CancellationToken.None);

            Assert.AreEqual(entities.Count, 1);
            Assert.AreEqual(entities.First().Name, entity.Name);
            Assert.AreEqual(entities.First().Id, entity.Id);
        }

        [TestMethod]
        public void DatabaseModelTest_ExpressionTranslatorTest()
        {
            IEnumerable<TestDatabaseEntity> collection = new List<TestDatabaseEntity>();
            var id = Guid.NewGuid().ToString();
            Expression<Func<TestDatabaseEntity, bool>> exp = col => col.Id == id && col.Name == "name";
            QueryTranslator translator = new MySQLQueryTranslator();
            // translator.Visit(exp);
            var queryCommand = translator.Translate(exp);

            Assert.IsNotNull(queryCommand);
            Assert.AreEqual(queryCommand.Command, "Id = @p0  AND  Name = @p1");
            Assert.AreEqual(queryCommand.Parameters.Count, 2);
            Assert.AreEqual(queryCommand.Parameters.First(p => p.Name == "@p0").Value, id);
            Assert.AreEqual(queryCommand.Parameters.First(p => p.Name == "@p1").Value, "name");
        }

        [TestMethod]
        public async Task DatabaseModelTest_CreateTableTest()
        {
            var tableModel = ExportProvider.GetExportedValue<DatabaseTableModel<AccountEntity>>();
            await tableModel.DeleteAsync(CancellationToken.None);
            await tableModel.CreateIfNotExists(CancellationToken.None);
        }

        [TestMethod]
        public void ValidationInfoTest()
        {
            AccountRegisterInfo info = new AccountRegisterInfo();
            info.AccountName = "testaccount111";
            info.Password = "123456";

            Assert.IsFalse(info.Validate());
            Assert.IsTrue(info.Errors.Any(e => e.MemberNames.Contains("Password")));

            info.Password = "ABC**(!@2Saaa";
            Assert.IsFalse(info.Validate());
            Assert.IsFalse(info.Errors.Any(e => e.MemberNames.Contains("Password")));
        }
    }
}
