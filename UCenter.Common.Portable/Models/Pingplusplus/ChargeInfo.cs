using System;
using System.Collections.Generic;

namespace UCenter.Common.Portable
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
