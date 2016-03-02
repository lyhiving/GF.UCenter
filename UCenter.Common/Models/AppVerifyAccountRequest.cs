using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AppVerifyAccountRequest
    {
        public string app_id;
        public string app_token;
        public ulong acc_id;
        public string acc_name;
        public string token;
        public bool get_appdata;
    }
}
