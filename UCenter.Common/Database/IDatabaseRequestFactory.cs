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
        IDatabaseRequest GenerateCreateTableRequest<TEntity>() where TEntity : BaseEntity;

        IDatabaseRequest GenerateDeleteTableRequest<TEntity>() where TEntity : BaseEntity;

        IDatabaseRequest GenerateInsertRequest<TEntity>(TEntity entity) where TEntity : BaseEntity;

        IDatabaseRequest GenerateDeleteRequest<TEntity>(TEntity entity) where TEntity : BaseEntity;

        IDatabaseRequest GenerateUpdateRequest<TEntity>(TEntity entity) where TEntity : BaseEntity;

        IDatabaseRequest GenerateReteriveRequest<TEntity>(TEntity entity) where TEntity : BaseEntity;

        IDatabaseRequest GenerateInsertOrUpdateRequest<TEntity>(TEntity entity) where TEntity : BaseEntity;

        IDatabaseRequest GenerateQueryRequest<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : BaseEntity;

        [Obsolete("Use common Reterive instead of this request.")]
        IDatabaseRequest CreateGetAccountRequest(string accountName);
    }
}
