using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UCenter.Common.Portable
{
    [Serializable]
    [DataContract]
    public class AppDataInfo
    {
        [DataMember]
        public string AppId;
        [DataMember]
        public string AppSecret;
        [DataMember]
        public string AccountId;
        [DataMember]
        public string Data;
    }
}
