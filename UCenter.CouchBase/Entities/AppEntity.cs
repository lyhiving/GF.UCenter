using UCenter.CouchBase.Attributes;

namespace UCenter.CouchBase.Entities
{
    [DocumentType("App")]
    public class AppEntity : BaseEntity<AppEntity>
    {
        public string AppId { get; set; }

        public string AppSecret { get; set; }

        public string Token { get; set; }
    }
}
