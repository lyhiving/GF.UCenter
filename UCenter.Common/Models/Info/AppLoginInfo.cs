using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AppLoginInfo
    {
        public string AppId;
        public string AppSecret;
        public string Type;
    }
}
