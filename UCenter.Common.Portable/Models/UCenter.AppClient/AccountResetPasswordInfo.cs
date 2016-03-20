using System;
using System.Collections.Generic;

namespace UCenter.Common.Portable
{
    public class AccountResetPasswordInfo
    {
        public string AccountName { get; set; }
        public string Password { get; set; }
        public string SuperPassword { get; set; }
    }
}
