using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.Linq;
using UCenter.Common.Database.Entities;

namespace UCenter.Common.Database.Couch
{
    public static class CouchBucketExtensions
    {
        public static IQueryable<TEntity> QueryByType<TEntity>(this IBucket bucket, Func<TEntity, bool> filter) where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            BucketContext context = new BucketContext(bucket);
            return context.Query<TEntity>().Where(t => filter(t) & t.Type == BaseEntity<TEntity>.DocumentType);
        }
    }
}
