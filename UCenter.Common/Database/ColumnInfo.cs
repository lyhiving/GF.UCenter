using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UCenter.Common.Database.Entities;

namespace UCenter.Common.Database
{
    public class ColumnInfo
    {
        public readonly ColumnAttribute Attribute;
        public readonly PropertyInfo Property;
        public readonly string Name;
        public readonly string MySQLParameterName;
        public readonly string ColumnName;
        public readonly Type DataType;
        public readonly bool AutoIncrement;
        public readonly bool IsKey;
        public readonly int Length;

        public ColumnInfo(PropertyInfo property)
        {
            this.Property = property;
            this.Name = property.Name;
            this.MySQLParameterName = "@" + this.Name;
            this.DataType = property.PropertyType;
            this.Attribute = property.GetCustomAttribute<ColumnAttribute>();
            if (this.Attribute != null)
            {
                this.ColumnName = this.Attribute.ColumnName ?? this.Name;
                this.AutoIncrement = this.Attribute.AutoIncrement;
                this.IsKey = this.Attribute.IsKey;
                this.Length = this.Attribute.Length;
            }
            else
            {
                this.ColumnName = this.Name;
            }
        }

    }
}
