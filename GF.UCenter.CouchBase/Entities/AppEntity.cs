
namespace GF.UCenter.CouchBase
{
    [DocumentType("App")]
    public class AppEntity : BaseEntity<AppEntity>
    {
        public string AppSecret { get; set; }
    }
}
