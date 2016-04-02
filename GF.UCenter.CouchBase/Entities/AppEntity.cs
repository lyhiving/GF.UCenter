namespace GF.UCenter.CouchBase.Entities
{
    using Attributes;

    [DocumentType("App")]
    public class AppEntity : BaseEntity<AppEntity>
    {
        public string AppSecret { get; set; }
    }
}