using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UCenter.SDK;

namespace UCenter.Test.SDK
{
    [TestClass]
    public class UCenterClientTestBase
    {
        protected UCenterClient client;
        private readonly string host;

        public UCenterClientTestBase()
        {
            this.host = "http://localhost:24865/";
            this.client = new UCenterClient(host);
        }
    }
}
