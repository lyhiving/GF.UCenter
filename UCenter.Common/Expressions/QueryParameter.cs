using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace UCenter.Common.Expressions
{
    internal class QueryParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public Type DataType { get; set; }

        public TypeCode TypeCode { get; set; }

        public MySqlParameter ToMySqlParameter()
        {
            MySqlParameter parameter = new MySqlParameter(
                this.Name,
                this.GetMySqlDbType());
            parameter.Value = this.Value;

            return parameter;
        }

        private MySqlDbType GetMySqlDbType()
        {
            switch (this.TypeCode)
            {
                case TypeCode.Boolean:
                    return MySqlDbType.Bit;
                case TypeCode.Char:
                    return MySqlDbType.VarChar;
                case TypeCode.SByte:
                    return MySqlDbType.Byte;
                case TypeCode.Byte:
                    return MySqlDbType.Byte;
                case TypeCode.UInt16:
                    return MySqlDbType.UInt16;
                case TypeCode.Int16:
                    return MySqlDbType.Int16;
                case TypeCode.Int32:
                    return MySqlDbType.Int32;
                case TypeCode.UInt32:
                    return MySqlDbType.UInt32;
                case TypeCode.Int64:
                    return MySqlDbType.Int64;
                case TypeCode.UInt64:
                    return MySqlDbType.UInt64;
                case TypeCode.Single:
                    return MySqlDbType.Int64;
                case TypeCode.Double:
                    return MySqlDbType.Double;
                case TypeCode.Decimal:
                    return MySqlDbType.Decimal;
                case TypeCode.DateTime:
                    return MySqlDbType.DateTime;
                case TypeCode.String:
                    return MySqlDbType.String;
                case TypeCode.Empty:
                case TypeCode.Object:
                case TypeCode.DBNull:
                default:
                    throw new Exception($"Type code not supported for mysql. {this.TypeCode}");
            }
        }
    }
}
