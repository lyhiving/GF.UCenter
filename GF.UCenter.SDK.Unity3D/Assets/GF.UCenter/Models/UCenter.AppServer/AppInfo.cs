using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace UCenter.Common.Portable
{
    [DataContract]
    public class AppInfo
    {
        [DataMember]
        public string AppId { get; set; }
        [DataMember]
        public string AppSecret { get; set; }
    }
}
