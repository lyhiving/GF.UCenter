using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Database
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : Attribute
    {
        public string ColumnName { get; set; }

        public bool AutoIncrement { get; set; } = false;

        public bool Ignore { get; set; } = false;

        public bool IsKey { get; set; } = false;

        public int Length { get; set; } = 0;
    }
}
