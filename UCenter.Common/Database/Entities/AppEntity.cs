using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Database.Entities
{
    public class AppEntity : BaseEntity<AppEntity>
    {
        public string AppId { get; set; }
        
        public string AppSecret { get; set; }
        
        public string Token { get; set; }
    }
}
