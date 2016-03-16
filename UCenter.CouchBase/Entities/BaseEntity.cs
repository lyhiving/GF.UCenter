using System;
using System.Reflection;
using Couchbase;
using UCenter.CouchBase.Attributes;

namespace UCenter.CouchBase.Entities
{
    public abstract class BaseEntity<TEntity> : IBaseEntity, IBaseEntity<TEntity> where TEntity : class, IBaseEntity
    {
        public static readonly string DocumentType = typeof(TEntity).GetCustomAttribute<DocumentTypeAttribute>().Type;
        private readonly string type = DocumentType;
        private string id;

        public virtual string Id
        {
            get
            {
                if (string.IsNullOrEmpty(this.id))
                {
                    this.id = Guid.NewGuid().ToString();
                }

                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        public string Type
        {
            get
            {
                return this.type;
            }
        }

        public IDocument<TEntity> ToDocument()
        {
            return new Document<TEntity>
            {
                Id = this.Id,
                Content = this as TEntity
            };
        }
    }

    public interface IBaseEntity
    {
        string Id { get; }

        string Type { get; }
    }

    public interface IBaseEntity<TEntity>
    {
        IDocument<TEntity> ToDocument();
    }
}
