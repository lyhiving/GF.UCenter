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
using UCenter.Common.Database.Entities;

namespace UCenter.Common.Database.Couch
{
    [Export]
    public class CouchBaseContext
    {
        private readonly ClientConfiguration configuration;
        private readonly ConcurrentDictionary<Type, string> bucketNameMap = new ConcurrentDictionary<Type, string>();
        private Settings settings;

        [ImportingConstructor]
        public CouchBaseContext(Settings settings)
        {
            this.settings = settings;

            var servers = this.settings
                .ServerUris
                .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => new Uri(s))
                .ToList();

            this.configuration = new ClientConfiguration
            {
                Servers = servers,
                UseSsl = this.settings.UseSsl,
                DefaultOperationLifespan = this.settings.DefaultOperationLifespan,
                EnableTcpKeepAlives = this.settings.EnableTcpKeepAlives,
                TcpKeepAliveTime = this.settings.TcpKeepAliveTime,
                TcpKeepAliveInterval = this.settings.TcpKeepAliveInterval,
                BucketConfigs = new Dictionary<string, BucketConfiguration>()
                {
                    {
                        this.settings.BucketName,
                        new BucketConfiguration
                        {
                            BucketName = this.settings.BucketName,
                            UseSsl = this.settings.UseSsl,
                            Password = "",
                            DefaultOperationLifespan = this.settings.DefaultOperationLifespan,
                            PoolConfiguration = new PoolConfiguration
                            {
                                MaxSize = this.settings.PoolMaxSize,
                                MinSize = this.settings.PoolMinSize,
                                SendTimeout = this.settings.PoolSendTimeout
                            }
                        }
                    }
                }
            };

            ClusterHelper.Initialize(this.configuration);
        }

        internal Cluster CreateCluster()
        {
            return ClusterHelper.Get();
        }

        public IBucket GetBucket<TEntity>() where TEntity : IBaseEntity
        {
            return ClusterHelper.GetBucket(this.settings.BucketName);
        }

        public IBucket Bucket
        {
            get
            {
                return ClusterHelper.GetBucket(this.settings.BucketName);
            }
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
