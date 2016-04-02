namespace GF.UCenter.CouchBase
{
    using System;
    using System.Reflection;
    using Couchbase;

    public abstract class BaseEntity<TEntity> : IBaseEntity, IBaseEntity<TEntity> where TEntity : class, IBaseEntity
    {
        public static readonly string DocumentType = typeof (TEntity).GetCustomAttribute<DocumentTypeAttribute>().Type;
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
            set { this.id = value; }
        }

        public string Type { get; } = DocumentType;

        public IDocument<TEntity> ToDocument()
        {
            return new Document<TEntity>
            {
                Id = GetDocumentId(this.Id),
                Content = this as TEntity
            };
        }

        public static string GetDocumentId(TEntity entity)
        {
            return GetDocumentId(entity.Id);
        }

        public static string GetDocumentId(string entityId)
        {
            return $"{DocumentType}_{entityId}";
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