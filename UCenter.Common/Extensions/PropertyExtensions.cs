using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using UCenter.Common.Database;

namespace UCenter.Common
{
    public static class PropertyExtensions
    {
        private static ConcurrentDictionary<Type, SqlDbType> typeMap = new ConcurrentDictionary<Type, SqlDbType>();

        public static string GetDbColumnName(this PropertyInfo property)
        {
            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            var name = columnAttribute == null || string.IsNullOrEmpty(columnAttribute.ColumnName)
                ? property.Name
                : columnAttribute.ColumnName;

            return name;
        }

        public static ColumnInfo GetDbColumnInfo(this PropertyInfo property)
        {
            return new ColumnInfo(property);
        }

        public static SqlDbType ToSqlDbType(this Type type)
        {
            return typeMap.GetOrAdd(type, t =>
            {
                if (type == typeof(string))
                    return SqlDbType.NVarChar;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    type = Nullable.GetUnderlyingType(type);

                var param = new SqlParameter("", Activator.CreateInstance(type));
                return param.SqlDbType;
            });
        }

        public static MySqlDbType ToMySqlDbType(this Type type)
        {
            var dbType = type.ToSqlDbType();
            switch (dbType)
            {
                case SqlDbType.BigInt:
                    return MySqlDbType.Int64;
                case SqlDbType.Binary:
                    return MySqlDbType.Binary;
                case SqlDbType.Bit:
                    return MySqlDbType.Bit;
                case SqlDbType.NChar:
                case SqlDbType.Char:
                    return MySqlDbType.Text;
                case SqlDbType.Date:
                    return MySqlDbType.Date;
                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                    return MySqlDbType.DateTime;
                case SqlDbType.Decimal:
                    return MySqlDbType.Decimal;
                case SqlDbType.Float:
                    return MySqlDbType.Float;
                case SqlDbType.Image:
                    return MySqlDbType.LongBlob;
                case SqlDbType.Int:
                    return MySqlDbType.Int32;
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return MySqlDbType.Decimal;
                case SqlDbType.NVarChar:
                case SqlDbType.VarChar:
                    return MySqlDbType.VarChar;
                case SqlDbType.SmallInt:
                    return MySqlDbType.Int16;
                case SqlDbType.NText:
                case SqlDbType.Text:
                    return MySqlDbType.LongText;
                case SqlDbType.Time:
                    return MySqlDbType.Time;
                case SqlDbType.Timestamp:
                    return MySqlDbType.Timestamp;
                case SqlDbType.TinyInt:
                    return MySqlDbType.Byte;
                case SqlDbType.UniqueIdentifier:
                    return MySqlDbType.Guid;
                case SqlDbType.VarBinary:
                    return MySqlDbType.VarBinary;
                case SqlDbType.Xml:
                    return MySqlDbType.Text;
                default:
                    throw new NotSupportedException(string.Format("The SQL type '{0}' is not supported", dbType));
            }
        }
    }
}
