using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AccountLoginResponse
    {
        public UCenterResult result;
        public string token;
        public ulong acc_id;
        public string acc_name;
        public Sex sex;
    }
}
