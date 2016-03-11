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
using System.Linq.Expressions;
using Couchbase.Linq;

namespace UCenter.Common.Database.Couch
{
    [Export]
    public class CouchBaseBucketContext<TEntity> where TEntity : IBaseEntity
    {
        // todo: this class is useless now...
        private readonly CouchBaseContext context;
        private readonly BucketContext bucketContext;

        public static readonly string TableName = typeof(TEntity).GetCustomAttribute<DatabaseTableNameAttribute>().TableName;

        public readonly IBucket Bucket;

        [ImportingConstructor]
        public CouchBaseBucketContext(CouchBaseContext context)
        {
            this.context = context;
            this.Bucket = ClusterHelper.GetBucket(TableName);
            this.bucketContext = new BucketContext(this.Bucket);
        }

        public async Task<TEntity> Get(string key)
        {
            var result = await this.Bucket.GetAsync<TEntity>(key);
            if (result.Success)
            {
                return result.Value;
            }

            return default(TEntity);
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var result = await this.Bucket.InsertAsync(this.CreateDocument(entity));

            return result.Content;
        }

        public async Task<TEntity> UpsertAsync(TEntity entity)
        {
            var result = await this.Bucket.UpsertAsync(this.CreateDocument(entity));

            return result.Content;
        }

        public IQueryable<TEntity> Query(Func<TEntity, bool> filter)
        {
            return this.bucketContext.Query<TEntity>().Where(t => filter(t));
        }

        public async Task RemoveAsync(TEntity entity)
        {
            await this.Bucket.RemoveAsync(this.CreateDocument(entity));
        }

        public async Task RemoveAsync(string key)
        {
            await this.Bucket.RemoveAsync(key);
        }

        private IDocument<TEntity> CreateDocument(TEntity entity)
        {
            return new Document<TEntity>
            {
                Id = entity.Id,
                Content = entity
            };
        }
    }
}
