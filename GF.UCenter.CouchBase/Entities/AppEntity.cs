
namespace GF.UCenter.CouchBase
{
    [DocumentType("App")]
    public class AppEntity : BaseEntity<AppEntity>
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }
}
