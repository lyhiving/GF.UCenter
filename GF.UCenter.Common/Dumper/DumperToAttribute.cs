namespace GF.UCenter.Common
{
    using System;

    [AttributeUsage(AttributeTargets.All)]
    public class DumperToAttribute : Attribute
    {
        public DumperToAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}