using System;
using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [Serializable]
    [DataContract]
    public class AppVerifyAccountInfo
    {
        [DataMember]
        public string AppId;
        [DataMember]
        public string AppSecret;
        [DataMember]
        public string AccountId;
        [DataMember]
        public string AccountToken;
    }
}
