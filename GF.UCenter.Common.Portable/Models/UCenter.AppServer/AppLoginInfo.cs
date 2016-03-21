using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UCenter.Common.Portable
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
