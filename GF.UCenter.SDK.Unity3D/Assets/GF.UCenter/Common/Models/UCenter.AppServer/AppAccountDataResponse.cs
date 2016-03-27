using System;
using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [DataContract]
    public class AppAccountDataResponse
    {
        [DataMember]
        public string AccountId;
        [DataMember]
        public string AppId;
        [DataMember]
        public string Data;
    }
}
