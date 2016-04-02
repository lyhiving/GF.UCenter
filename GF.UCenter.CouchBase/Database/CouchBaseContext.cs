namespace GF.UCenter.CouchBase.Database
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Linq;
    using Common.Settings;
    using Couchbase;
    using Couchbase.Configuration.Client;
    using Couchbase.Core;
    using Entities;

    [Export]
    public class CouchBaseContext
    {
        private readonly ConcurrentDictionary<Type, string> bucketNameMap = new ConcurrentDictionary<Type, string>();
        private readonly ClientConfiguration configuration;
        private readonly Settings settings;

        [ImportingConstructor]
        public CouchBaseContext(Settings settings)
        {
            this.settings = settings;

            var servers = this.settings
                .ServerUris
                .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => new Uri(s))
                .ToList();

            var bucketConfigs = new Dictionary<string, BucketConfiguration>();
            var configs = new[] {this.settings.BucketName}
                .Select(b => new BucketConfiguration
                {
                    BucketName = b,
                    UseSsl = this.settings.UseSsl,
                    Password = "",
                    DefaultOperationLifespan = this.settings.DefaultOperationLifespan,
                    PoolConfiguration = new PoolConfiguration
                    {
                        MaxSize = this.settings.PoolMaxSize,
                        MinSize = this.settings.PoolMinSize,
                        SendTimeout = this.settings.PoolSendTimeout
                    }
                });

            foreach (var config in configs)
            {
                bucketConfigs.Add(config.BucketName, config);
            }

            this.configuration = new ClientConfiguration
            {
                Servers = servers,
                UseSsl = this.settings.UseSsl,
                DefaultOperationLifespan = this.settings.DefaultOperationLifespan,
                EnableTcpKeepAlives = this.settings.EnableTcpKeepAlives,
                TcpKeepAliveTime = this.settings.TcpKeepAliveTime,
                TcpKeepAliveInterval = this.settings.TcpKeepAliveInterval,
                BucketConfigs = bucketConfigs
            };

            ClusterHelper.Initialize(this.configuration);
        }

        public IBucket Bucket
        {
            get { return ClusterHelper.GetBucket(this.settings.BucketName); }
        }

        public Cluster Cluster
        {
            get { return this.CreateCluster(); }
        }

        internal Cluster CreateCluster()
        {
            return ClusterHelper.Get();
        }

        public IBucket GetBucket<TEntity>() where TEntity : IBaseEntity
        {
            return ClusterHelper.GetBucket(this.settings.BucketName);
        }
    }
}