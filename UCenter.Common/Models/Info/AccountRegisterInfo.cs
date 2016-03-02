using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AccountRegisterInfo
    {
        public string AccountName;
        public string Password;
        public string SuperPassword;
        public string Name;
        public string PhoneNum;
        public string IdentityNum;
        public Sex Sex;
    }
}
