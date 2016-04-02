namespace GF.UCenter.CouchBase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Couchbase.Core;
    using Couchbase.N1QL;

    public static class CouchBucketExtensions
    {
        public static async Task<IQueryResult<TEntity>> QueryAsync<TEntity>(this IBucket bucket,
            Expression<Func<TEntity, bool>> expression) where TEntity : class, IBaseEntity
        {
            string condition = $"type='{BaseEntity<TEntity>.DocumentType}'";
            var request = new QueryRequest();

            if (expression != null)
            {
                var translator = new CouchQueryTranslator();
                var command = translator.Translate(expression);
                condition = $"{command.Command} AND {condition}";
                request.AddPositionalParameter(command.Parameters.Select(p => p.Value).ToArray());
            }

            request.Statement($"SELECT {bucket.Name}.* FROM {bucket.Name} WHERE {condition}");

            var query = await bucket.QueryAsync<TEntity>(request);

            return query;
        }

        public static async Task<IEnumerable<TEntity>> QuerySlimAsync<TEntity>(this IBucket bucket,
            Expression<Func<TEntity, bool>> expression, bool throwIfFailed = true) where TEntity : class, IBaseEntity
        {
            var result = await bucket.QueryAsync(expression);

            if (result.Success)
            {
                return result.Rows;
            }

            if (throwIfFailed)
            {
                if (result.Exception != null)
                {
                    throw result.Exception;
                }
                throw new CouchBaseException(result);
            }
            return new List<TEntity>();
        }

        public static async Task<TEntity> InsertSlimAsync<TEntity>(this IBucket bucket, TEntity entity,
            bool throwIfFailed = true) where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            var result = await bucket.InsertAsync(entity.ToDocument());

            if (result.Success)
            {
                return result.Content;
            }

            if (throwIfFailed)
            {
                throw new CouchBaseException(result);
            }

            return null;
        }

        public static async Task<TEntity> UpsertSlimAsync<TEntity>(this IBucket bucket, TEntity entity,
            bool throwIfFailed = true) where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            var result = await bucket.UpsertAsync(entity.ToDocument());

            if (result.Success)
            {
                return result.Content;
            }

            if (throwIfFailed)
            {
                throw new CouchBaseException(result);
            }

            return null;
        }

        public static async Task<TEntity> GetSlimAsync<TEntity>(this IBucket bucket, string key,
            bool throwIfFailed = true) where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            var result = await bucket.GetAsync<TEntity>(key);

            if (result.Success)
            {
                return result.Value;
            }

            if (throwIfFailed)
            {
                throw new CouchBaseException(result);
            }

            return null;
        }

        public static Task<TEntity> GetByEntityIdSlimAsync<TEntity>(this IBucket bucket, string entityId,
            bool throwIfFailed = false) where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            var key = BaseEntity<TEntity>.GetDocumentId(entityId);

            return bucket.GetSlimAsync<TEntity>(key, throwIfFailed);
        }

        public static async Task<TEntity> FirstOrDefaultAsync<TEntity>(this IBucket bucket,
            Expression<Func<TEntity, bool>> expression, bool throwIfFailed = true) where TEntity : class, IBaseEntity
        {
            var result = await bucket.QuerySlimAsync(expression, throwIfFailed);

            return result.FirstOrDefault();
        }
    }
}