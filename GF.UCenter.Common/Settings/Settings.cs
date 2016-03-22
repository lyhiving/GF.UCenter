using System.ComponentModel;
using System.ComponentModel.Composition;

namespace GF.UCenter.Common
{
    [Export]
    public class Settings
    {
        [DefaultValue("ucenter")]
        public string BucketName { get; set; }

        [DefaultValue("ucenter-temp")]
        public string TempBucketName { get; set; }

        [DefaultValue("http://127.0.0.1:8091")]
        public string ServerUris { get; set; }

        [DefaultValue(1000)]
        public uint DefaultOperationLifespan { get; set; }

        [DefaultValue(true)]
        public bool EnableTcpKeepAlives { get; set; }

        [DefaultValue(1000 * 60 * 60)]
        public uint TcpKeepAliveTime { get; set; }

        [DefaultValue(1000)]
        public uint TcpKeepAliveInterval { get; set; }

        [DefaultValue(false)]
        public bool UseSsl { get; set; }

        [DefaultValue(10)]
        public int PoolMaxSize { get; set; }

        [DefaultValue(5)]
        public int PoolMinSize { get; set; }

        [DefaultValue(12000)]
        public int PoolSendTimeout { get; set; }
    }
}
