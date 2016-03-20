using System;
using System.Collections.Generic;

namespace UCenter.Common.Portable
{
    public class AccountGuestLoginResponse
    {
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
    }
}
