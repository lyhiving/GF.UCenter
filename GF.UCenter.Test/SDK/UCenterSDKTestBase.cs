using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GF.UCenter.Test
{
    [TestClass]
    public class UCenterSDKTestBase : UCenterTestBase
    {
        protected const string TestAppId = "utapp";
        protected const string TestAppSecret = "#pA554&3321#";
        protected const string ValidPassword = "#pA554&3321#";

        public UCenterSDKTestBase()
        {
        }
    }
}
