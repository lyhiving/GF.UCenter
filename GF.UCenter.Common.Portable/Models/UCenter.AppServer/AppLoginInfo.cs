using System;
using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [DataContract]
    public class AppLoginInfo
    {
        [DataMember]
        public string AppId;
        [DataMember]
        public string AppSecret;
    }
}
