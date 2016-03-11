using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Couchbase;
using Couchbase.Configuration.Client;
using Couchbase.Core;
using Couchbase.Linq;
using UCenter.Common.Database.Entities;

namespace UCenter.Common.Database.Couch
{
    [Export]
    public class CouchBaseContext
    {
        private readonly ClientConfiguration configuration;
        private readonly ConcurrentDictionary<Type, string> bucketNameMap = new ConcurrentDictionary<Type, string>();

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

            ClusterHelper.Initialize(this.configuration);
        }

        private void InitBucktConfigs(ClientConfiguration clientConfig)
        {
            var configurations = this.GetType().Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(IBaseEntity)))
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
            return ClusterHelper.Get();
        }

        public IBucket GetBucket<TEntity>() where TEntity : IBaseEntity
        {
            return ClusterHelper.GetBucket(this.GetBucketName<TEntity>());
        }

        public BucketContext GetBucketContext<TEntity>() where TEntity : IBaseEntity
        {
            return new BucketContext(this.GetBucket<TEntity>());
        }

        private string GetBucketName<TEntity>() where TEntity : IBaseEntity
        {
            return this.bucketNameMap.GetOrAdd(typeof(TEntity), t =>
            {
                var attr = t.GetCustomAttribute<DatabaseTableNameAttribute>();
                return attr == null ? t.Name : attr.TableName;
            });
        }

        public IBucket Accounts
        {
            get
            {
                return this.GetBucket<AccountEntity>();
            }
        }

        public IBucket LoginRecords
        {
            get
            {
                return this.GetBucket<LoginRecordEntity>();
            }
        }
    }
}
