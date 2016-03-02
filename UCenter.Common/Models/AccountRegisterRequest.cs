using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    /// <summary>
    /// 注册帐号信息
    /// </summary>
    [Serializable]
    public class AccountRegisterRequest
    {
        public string acc;
        public string pwd;
        public string super_pwd;
        public string phone_num;
        public string name;
        public string identity_num;
        public Sex sex_type;
    }
}
