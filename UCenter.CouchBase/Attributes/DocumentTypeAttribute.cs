using System;

namespace UCenter.CouchBase.Attributes
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
