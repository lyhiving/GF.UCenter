using System;
using System.Runtime.Serialization;

namespace GF.UCenter.Common.Portable
{
    [DataContract]
    public class AccountUploadProfileImageInfo
    {
        [DataMember]
        public string AccountId { get; set; }
    }
}
