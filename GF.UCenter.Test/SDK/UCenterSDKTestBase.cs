using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCenter.Common.Portable;

namespace UCenter.Test.SDK
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
