using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Core;
using Couchbase.N1QL;
using UCenter.Common;
using UCenter.Common.Database.Entities;
using UCenter.Common.Exceptions;
using UCenter.Common.Models;

namespace UCenter.Common.Database.Couch
{
    public static class CouchBucketExtensions
    {
        public async static Task<IQueryResult<TEntity>> QueryAsync<TEntity>(this IBucket bucket, Expression<Func<TEntity, bool>> expression) where TEntity : class, IBaseEntity
        {
            string condition = $"type='{BaseEntity<TEntity>.DocumentType}'";
            var request = new QueryRequest();

            if (expression != null)
            {
                CouchQueryTranslator translator = new CouchQueryTranslator();
                var command = translator.Translate(expression);
                condition = $"{command.Command} AND {condition}";
                request.AddPositionalParameter(command.Parameters.Select(p => p.Value).ToArray());
            }

            request.Statement($"SELECT {bucket.Name}.* FROM {bucket.Name} WHERE {condition}");

            var query = await bucket.QueryAsync<TEntity>(request);

            return query;
        }

        public async static Task<IEnumerable<TEntity>> QuerySlimAsync<TEntity>(this IBucket bucket, Expression<Func<TEntity, bool>> expression, bool throwIfFailed = true) where TEntity : class, IBaseEntity
        {
            var result = await bucket.QueryAsync<TEntity>(expression);

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
                else // if (result.Errors != null && result.Errors.Count > 0)
                {
                    throw new CouchBaseException(result);
                }
            }
            else
            {
                return new List<TEntity>();
            }
        }

        public async static Task<TEntity> InsertSlimAsync<TEntity>(this IBucket bucket, TEntity entity, bool throwIfFailed = true) where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            var result = await bucket.InsertAsync<TEntity>(entity.ToDocument());

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

        public async static Task<TEntity> UpsertSlimAsync<TEntity>(this IBucket bucket, TEntity entity, bool throwIfFailed = true) where TEntity : BaseEntity<TEntity>, IBaseEntity
        {
            var result = await bucket.UpsertAsync<TEntity>(entity.ToDocument());

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

        public async static Task<TEntity> GetSlimAsync<TEntity>(this IBucket bucket, string key, bool throwIfFailed = true) where TEntity : BaseEntity<TEntity>, IBaseEntity
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

        public async static Task<TEntity> FirstOrDefaultAsync<TEntity>(this IBucket bucket, Expression<Func<TEntity, bool>> expression, bool throwIfFailed = true) where TEntity : class, IBaseEntity
        {
            var result = await bucket.QuerySlimAsync(expression, throwIfFailed);

            return result.FirstOrDefault();
        }
    }
}
