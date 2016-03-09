using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Database.Entities;
using Couchbase;
using Couchbase.Core;
using System.ComponentModel.Composition;

namespace UCenter.Common.Database.Couch
{
    [Export]
    public class CouchBaseTable<TEntity> : DisposableObjectSlim where TEntity : BaseEntity
    {
        private readonly CouchBaseContext context;

        public readonly string TableName = typeof(TEntity).GetCustomAttribute<DatabaseTableNameAttribute>().TableName;

        private readonly Cluster Cluster;
        private readonly IBucket Bucket;

        [ImportingConstructor]
        public CouchBaseTable(CouchBaseContext context)
        {
            this.context = context;
            this.Cluster = context.CreateCluster();
            try
            {
                this.Bucket = Cluster.OpenBucket(this.TableName);
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var document = new Document<TEntity>()
            {
                Id = entity.Id,
                Content = entity
            };

            var result = await this.Bucket.InsertAsync(document);

            return document.Content;
        }

        public async Task<TEntity> UpsertAsync(TEntity entity)
        {
            var document = new Document<TEntity>
            {
                Id = entity.Id,
                Content = entity
            };

            var result = await this.Bucket.UpsertAsync(document);

            return result.Content;
        }

        protected override void DisposeInternal()
        {
            if (this.Bucket != null)
            {
                this.Bucket.Dispose();
            }

            if (this.Cluster != null)
            {
                this.Cluster.Dispose();
            }
        }
    }
}
