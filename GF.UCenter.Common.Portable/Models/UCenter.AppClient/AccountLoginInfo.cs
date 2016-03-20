using System;
using System.Collections.Generic;

namespace UCenter.Common.Portable
{
    [Serializable]
    public class AccountLoginInfo
    {
        public string AccountName { get; set; }
        public string Password { get; set; }
    }

}
