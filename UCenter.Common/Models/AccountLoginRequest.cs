using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AccountLoginRequest
    {
        public string acc;
        public string pwd;
        public string project_name;
        public string protocol_version;
        public string channel_name;
        public Dictionary<string, string> map_param;
    }

}
