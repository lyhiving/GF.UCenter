using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DocumentTypeAttribute : Attribute
    {
        public DocumentTypeAttribute(string type)
        {
            this.Type = type;
        }

        public string Type { get; private set; }
    }
}
