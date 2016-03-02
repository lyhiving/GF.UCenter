using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Models
{
    [Serializable]
    public class AppReadDataResponse
    {
        public UCenterResult result;
        public string app_id;
        public ulong acc_id;
        public AppData app_data;
    }

}
