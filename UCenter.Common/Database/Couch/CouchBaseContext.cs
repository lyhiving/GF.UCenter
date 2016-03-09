using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Configuration.Client;
using UCenter.Common.Database.Entities;

namespace UCenter.Common.Database.Couch
{
    [Export]
    public class CouchBaseContext
    {
        private readonly ClientConfiguration configuration;

        [ImportingConstructor]
        public CouchBaseContext()
        {
            this.configuration = new ClientConfiguration
            {
                // todo: config the following settings.
                Servers = new List<Uri>
                {
                    new Uri("http://127.0.0.1:8091/pools")
                },
                UseSsl = false,
                DefaultOperationLifespan = 1000,
                EnableTcpKeepAlives = true,
                TcpKeepAliveTime = 1000 * 60 * 60,
                TcpKeepAliveInterval = 5000,
                BucketConfigs = new Dictionary<string, BucketConfiguration>()
            };

            InitBucktConfigs(this.configuration);
        }

        private void InitBucktConfigs(ClientConfiguration clientConfig)
        {
            var configurations = this.GetType().Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(BaseEntity)))
                 .Select(t =>
                 {
                     // todo: config the following settings.
                     var tableName = t.GetCustomAttribute<DatabaseTableNameAttribute>().TableName;
                     return new BucketConfiguration
                     {
                         BucketName = tableName,
                         UseSsl = false,
                         Password = "",
                         DefaultOperationLifespan = 2000,
                         PoolConfiguration = new PoolConfiguration
                         {
                             MaxSize = 10,
                             MinSize = 5,
                             SendTimeout = 12000
                         }
                     };
                 });

            foreach (var conf in configurations)
            {
                clientConfig.BucketConfigs.Add(conf.BucketName, conf);
            }
        }

        internal Cluster CreateCluster()
        {
            return new Cluster(this.configuration);
        }
    }
}
