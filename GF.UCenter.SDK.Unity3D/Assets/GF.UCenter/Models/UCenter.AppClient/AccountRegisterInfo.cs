using System;
using System.Collections.Generic;

namespace UCenter.Common.Portable
{
    [Serializable]
    public class AccountRegisterInfo
    {
        /// <summary>
        /// Gets or sets the account name
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        public string SuperPassword { get; set; }

        public string Name { get; set; }

        public string PhoneNum { get; set; }

        public string IdentityNum { get; set; }

        public Sex Sex { get; set; }
    }
}
