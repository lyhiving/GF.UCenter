using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Database.Entities;
using UCenter.Common.Models;

namespace UCenter.Common.Database
{
    public interface IDatabaseRequestFactory
    {
        IDatabaseRequest GenerateCreateTableRequest<TEntity>() where TEntity : IBaseEntity;

        IDatabaseRequest GenerateDeleteTableRequest<TEntity>() where TEntity : IBaseEntity;

        IDatabaseRequest GenerateInsertRequest<TEntity>(TEntity entity) where TEntity : IBaseEntity;

        IDatabaseRequest GenerateDeleteRequest<TEntity>(TEntity entity) where TEntity : IBaseEntity;

        IDatabaseRequest GenerateUpdateRequest<TEntity>(TEntity entity) where TEntity : IBaseEntity;

        IDatabaseRequest GenerateReteriveRequest<TEntity>(TEntity entity) where TEntity : IBaseEntity;

        IDatabaseRequest GenerateInsertOrUpdateRequest<TEntity>(TEntity entity) where TEntity : IBaseEntity;

        IDatabaseRequest GenerateQueryRequest<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : IBaseEntity;

        [Obsolete("Use common Reterive instead of this request.")]
        IDatabaseRequest CreateGetAccountRequest(string accountName);
    }
}
