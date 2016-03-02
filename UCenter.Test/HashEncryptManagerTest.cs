using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCenter.Common;

namespace UCenter.Test
{
    [TestClass]
    public class HashEncryptManagerTest
    {
        [TestMethod]
        public void TestEncryptAndCompare()
        {
            string password = Guid.NewGuid().ToString();
            var hash = EncryptHashManager.ComputeHash(password);
            Assert.IsFalse(EncryptHashManager.VerifyHash(Guid.NewGuid().ToString(), hash));
            Assert.IsTrue(EncryptHashManager.VerifyHash(password, hash));
        }
    }
}
