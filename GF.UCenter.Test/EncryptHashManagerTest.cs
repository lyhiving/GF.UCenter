namespace GF.UCenter.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using UCenter.Common;

    [TestClass]
    public class EncryptHashManagerTest
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