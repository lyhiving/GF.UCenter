using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common.Database
{
    public class MySQLDatabaseRequest : IDatabaseRequest
    {
        private readonly List<MySqlParameter> parameters = new List<MySqlParameter>();
        public readonly string CommandText;

        public MySQLDatabaseRequest(string commandText)
        {
            this.CommandText = commandText;
        }

        public MySqlCommand GetCommand(MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand(this.CommandText, connection);
            this.parameters.ForEach(p => command.Parameters.Add(p));
            return command;
        }

        public MySQLDatabaseRequest AddParameter(MySqlParameter parameter)
        {
            this.parameters.Add(parameter);
            return this;
        }

        public MySQLDatabaseRequest AddParameter(string name, object value)
        {
            if (value is DateTime)
            {
                value = ((DateTime)value).ToString("yyyy-MM-dd hh:mm:ss");
            }
            else if (value != null && value.GetType().IsEnum)
            {
                value = Convert.ToInt32(value);
            }
            else if (value == null)
            {
                value = string.Empty;
            }

            return this.AddParameter(new MySqlParameter("@" + name, value));
        }

        public MySQLDatabaseRequest AddParameters<TEntity>(IEnumerable<ColumnInfo> columns, TEntity entity)
        {
            foreach (var column in columns)
            {
                this.AddParameter(column.Name, column.Property.GetValue(entity));
            }

            return this;
        }
    }
}
