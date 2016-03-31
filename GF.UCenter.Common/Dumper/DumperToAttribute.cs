using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GF.UCenter.Common
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class DumperToAttribute : Attribute
    {
        public DumperToAttribute(string value)
        {
            this.Value = value;
        }

        public string Value { get; private set; }
    }
}
