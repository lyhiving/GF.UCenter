using System;
using System.Collections.Generic;

namespace UCenter.Common.Portable
{
    [Serializable]
    public class ChargeInfo
    {
        public string OrderNo { get; set; }
        public string Channel { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
