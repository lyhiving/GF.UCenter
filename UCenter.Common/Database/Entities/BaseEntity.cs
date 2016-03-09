using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Database.Entities
{
    public class BaseEntity
    {
        private string id;

        public string Id
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
    }
}
