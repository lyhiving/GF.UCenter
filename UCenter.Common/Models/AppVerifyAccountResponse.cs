using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AppVerifyAccountResponse
    {
        public UCenterResult result;
        public string app_id;
        public ulong acc_id;
        public string acc_name;
        public string token;
        public DateTime last_login_dt;
        public DateTime now_dt;
        public AppData app_data;
    }
}
