using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Database.Entities
{
    [DatabaseTableName("account")]
    public class AppEntity : BaseEntity<AppEntity>
    {
        [Column(Length = 64, IsKey = true)]
        public string AppId { get; set; }

        [Column(Length = 64)]
        public string AppSecret { get; set; }

        [Column(Length = 512)]
        public string Token { get; set; }
    }
}
