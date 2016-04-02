namespace GF.UCenter.CouchBase.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class DocumentTypeAttribute : Attribute
    {
        public DocumentTypeAttribute(string type)
        {
            this.Type = type;
        }

        public string Type { get; private set; }
    }
}