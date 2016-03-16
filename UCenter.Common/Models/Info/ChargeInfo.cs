using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Database.Entities;

namespace UCenter.Common.Models
{
    [Serializable]
    public class ChargeInfo
    {
        public string OrderNo;
        public string Channel;
        public double Amount;
        public string Subject;
    }
}
