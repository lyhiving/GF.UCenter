using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UCenter.Common.Database;
using UCenter.Common.Database.Entities;
using System.Linq.Expressions;
using System.Collections.Concurrent;

namespace UCenter.Common.Models
{
    [Export]
    public class DatabaseTableModel<TEntity> where TEntity : IBaseEntity
    {
        public static readonly string TableName = typeof(TEntity).GetCustomAttribute<DatabaseTableNameAttribute>().TableName;

        private static readonly ConcurrentDictionary<string, ICollection<ColumnInfo>> columnsMap = new ConcurrentDictionary<string, ICollection<ColumnInfo>>();
        private static readonly Lazy<Action<TEntity, TEntity>> copyAction = new Lazy<Action<TEntity, TEntity>>(
            () => BuildCompareAndCopyAction(),
            LazyThreadSafetyMode.PublicationOnly);
        private static readonly Lazy<IEnumerable<ColumnInfo>> columns = new Lazy<IEnumerable<ColumnInfo>>(
            () => GetColumnList(),
            LazyThreadSafetyMode.PublicationOnly);

        private readonly IDatabaseClient client;
        private readonly IDatabaseRequestFactory requestFactory;

        [ImportingConstructor]
        public DatabaseTableModel(IDatabaseClient client, IDatabaseRequestFactory requestFactory)
        {
            this.client = client;
            this.requestFactory = requestFactory;
        }

        public static IEnumerable<ColumnInfo> Columns
        {
            get
            {
                return columns.Value;
            }
        }

        public virtual async Task<TEntity> InsertEntityAsync(TEntity entity, CancellationToken token)
        {
            var request = this.requestFactory.GenerateInsertRequest<TEntity>(entity);
            var newEntity = await this.client.ExecuteSingleAsync<TEntity>(request, token);

            Copy(entity, newEntity);
            return entity;
        }

        public virtual Task DeleteEntityAsync(TEntity entity, CancellationToken token)
        {
            var request = this.requestFactory.GenerateDeleteRequest<TEntity>(entity);

            return this.client.ExecuteNoQueryAsync(request, token);
        }

        public virtual Task<TEntity> UpdateEntityAsync(TEntity entity, CancellationToken token)
        {
            var request = this.requestFactory.GenerateUpdateRequest<TEntity>(entity);
            return this.client.ExecuteSingleAsync<TEntity>(request, token);
        }

        public virtual async Task<TEntity> InsertOrUpdateAsync(TEntity entity, CancellationToken token)
        {
            var request = this.requestFactory.GenerateInsertOrUpdateRequest<TEntity>(entity);
            var newEntity = await this.client.ExecuteSingleAsync<TEntity>(request, token);
            Copy(entity, newEntity);

            return entity;
        }

        public virtual Task<TEntity> RetrieveEntityAsync(TEntity entity, CancellationToken token)
        {
            var request = this.requestFactory.GenerateReteriveRequest<TEntity>(entity);

            return this.client.ExecuteSingleAsync<TEntity>(request, token);
        }

        public virtual Task<ICollection<TEntity>> RetrieveEntitiesAsync(Expression<Func<TEntity, bool>> queryExpression, CancellationToken token)
        {
            var request = this.requestFactory.GenerateQueryRequest(queryExpression);

            return this.client.ExecuteListAsync<TEntity>(request, token);
        }

        public virtual Task CreateIfNotExists(CancellationToken token)
        {
            var request = this.requestFactory.GenerateCreateTableRequest<TEntity>();
            return this.client.ExecuteNoQueryAsync(request, token);
        }

        public virtual Task DeleteAsync(CancellationToken token)
        {
            var request = this.requestFactory.GenerateDeleteTableRequest<TEntity>();
            return this.client.ExecuteNoQueryAsync(request, token);
        }

        public static void Copy(TEntity targetEntity, TEntity sourceEntity)
        {
            DatabaseTableModel<TEntity>.copyAction.Value(targetEntity, sourceEntity);
        }

        private static ICollection<ColumnInfo> GetColumnList()
        {
            var tableName = DatabaseTableModel<TEntity>.TableName;
            return columnsMap.GetOrAdd(tableName, key => GenerateColumnList());
        }

        private static ICollection<ColumnInfo> GenerateColumnList()
        {
            return typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
               .Select(p => p.GetDbColumnInfo()).ToArray();
        }

        private static Action<TEntity, TEntity> BuildCompareAndCopyAction()
        {
            var expressions = new List<Expression>();

            var sourceExpression = Expression.Parameter(typeof(TEntity), "source");
            var targetExpression = Expression.Parameter(typeof(TEntity), "target");

            expressions.AddRange(typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Select(p =>
                {
                    var leftExpression = Expression.Property(targetExpression, p);
                    var rightExpression = Expression.Property(sourceExpression, p);
                    return Expression.IfThen(
                        Expression.NotEqual(leftExpression, rightExpression),
                        Expression.Assign(leftExpression, rightExpression));
                }));

            return Expression.Lambda<Action<TEntity, TEntity>>(
                Expression.Block(expressions), targetExpression, sourceExpression).Compile();
        }
    }
}
