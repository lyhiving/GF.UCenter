using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UCenter.Common.Portable
{
    [Serializable]
    [DataContract]
    public class ChargeInfo
    {
        [DataMember]
        public string OrderNo { get; set; }
        [DataMember]
        public string Channel { get; set; }
        [DataMember]
        public double Amount { get; set; }
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public string Subject { get; set; }
        [DataMember]
        public string Body { get; set; }
    }
}
