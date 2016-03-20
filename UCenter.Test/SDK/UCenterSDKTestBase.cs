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
        protected const string TestAppId = "texaspoker";
        protected const string TestAppSecret = "767c71c5-1bc5-4323-9e46-03a6a55c6ab1";
        protected const string ValidPassword = "#pA554&3321#";

        public UCenterSDKTestBase()
        {
        }
    }
}
