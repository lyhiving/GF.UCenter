using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    [Serializable]
    public class VerifyAccountData
    {
        public UCenterErrorCode result;
        public ulong acc_id;
        public string token;
        public DateTime last_login_dt;
        public DateTime now_dt;
    }
}
