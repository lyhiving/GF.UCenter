using System;
using System.Collections.Generic;

namespace UCenter.Common.Portable
{
    public class AccountResetPasswordInfo
    {
        public string AccountId { get; set; }
        public string Password { get; set; }
        public string SuperPassword { get; set; }
    }
}
