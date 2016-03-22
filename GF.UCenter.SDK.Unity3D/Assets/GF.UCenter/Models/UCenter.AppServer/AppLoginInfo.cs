using System;
using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [Serializable]
    [DataContract]
    public class AppLoginInfo
    {
        [DataMember]
        public string AppId;
        [DataMember]
        public string AppSecret;
    }
}
