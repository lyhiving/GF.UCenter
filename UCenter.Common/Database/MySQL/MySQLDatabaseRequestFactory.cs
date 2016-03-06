using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using UCenter.Common.Database.Entities;
using UCenter.Common.Expressions;
using UCenter.Common.Models;

namespace UCenter.Common.Database.MySQL
{
    [Export(typeof(IDatabaseRequestFactory))]
    public class MySQLDatabaseRequestFactory : IDatabaseRequestFactory
    {
        private const string MySQLSelectLastInsertIdCommand = "SELECT LAST_INSERT_ID()";
        private readonly ConcurrentDictionary<string, string> commandTexts = new ConcurrentDictionary<string, string>();

        public IDatabaseRequest CreateGetAccountRequest(string accountName)
        {
            string sql_str = "SELECT * FROM Account WHERE binary AccountName='{0}';".FormatInvariant(
                accountName);

            return new MySQLDatabaseRequest(sql_str);
        }

        public IDatabaseRequest GenerateInsertRequest<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            return RequestFactory<TEntity>.GenerateInsertRequest(entity);
        }

        public IDatabaseRequest GenerateDeleteRequest<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            return RequestFactory<TEntity>.GenerateDeleteRequest(entity);
        }

        public IDatabaseRequest GenerateUpdateRequest<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            return RequestFactory<TEntity>.GenerateUpdateRequest(entity);
        }

        public IDatabaseRequest GenerateReteriveRequest<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            return RequestFactory<TEntity>.GenerateReteriveRequest(entity);
        }

        public IDatabaseRequest GenerateInsertOrUpdateRequest<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            return RequestFactory<TEntity>.GenerateInsertOrUpdateRequest(entity);
        }

        public IDatabaseRequest GenerateCreateTableRequest<TEntity>() where TEntity : BaseEntity
        {
            return RequestFactory<TEntity>.GenerateCreateTableRequest();
        }

        public IDatabaseRequest GenerateDeleteTableRequest<TEntity>() where TEntity : BaseEntity
        {
            return RequestFactory<TEntity>.GenerateDeleteTableRequest();
        }

        public IDatabaseRequest GenerateQueryRequest<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : BaseEntity
        {
            return RequestFactory<TEntity>.GenerateQueryRequest(expression);
        }

        private static class RequestFactory<TEntity> where TEntity : BaseEntity
        {
            private const string MySQLSelectLastInsertIdCommand = "SELECT LAST_INSERT_ID()";
            private static readonly ConcurrentDictionary<string, string> commandTexts = new ConcurrentDictionary<string, string>();
            private static readonly string TableName = DatabaseTableModel<TEntity>.TableName;
            private static readonly IEnumerable<ColumnInfo> Columns = DatabaseTableModel<TEntity>.Columns;
            public static IDatabaseRequest CreateGetAccountRequest(string accountName)
            {
                string sql_str = "SELECT * FROM Account WHERE binary AccountName='{0}';".FormatInvariant(
                    accountName);

                return new MySQLDatabaseRequest(sql_str);
            }

            public static IDatabaseRequest GenerateInsertRequest(TEntity entity)
            {
                string key = $"insert-{ TableName}";

                var insertColumns = Columns.Where(c => !c.AutoIncrement);

                string command = commandTexts.GetOrAdd(key, k =>
                {
                    // todo: consider the auto increment column as key when return the new inserted row.
                    var keyColumns = Columns.Where(c => c.IsKey);
                    string columns = insertColumns.JoinToString(",", c => c.ColumnName);
                    string values = insertColumns.JoinToString(",", c => c.MySQLParameterName);
                    string where = keyColumns.JoinToString(",", c => c.ColumnName + "=" + (c.AutoIncrement ? $"({MySQLSelectLastInsertIdCommand})" : c.MySQLParameterName));
                    return $@"INSERT INTO {TableName} ({columns}) VALUES ({values}); SELECT {Columns.JoinToString(",", c => c.ColumnName)} FROM {TableName} WHERE {where};";
                });

                MySQLDatabaseRequest request = new MySQLDatabaseRequest(command);
                request.AddParameters(insertColumns, entity);

                return request;
            }

            public static IDatabaseRequest GenerateDeleteRequest(TEntity entity)
            {
                string key = $"delete-{ TableName}";
                var keyColumns = Columns.Where(c => c.IsKey);

                string command = commandTexts.GetOrAdd(key, k =>
                {
                    return $"DELETE FROM {TableName} WHERE { keyColumns.JoinToString(" AND ", c => c.ColumnName + "=" + c.MySQLParameterName)};";
                });

                MySQLDatabaseRequest request = new MySQLDatabaseRequest(command);
                request.AddParameters(keyColumns, entity);

                return request;
            }

            public static IDatabaseRequest GenerateUpdateRequest(TEntity entity)
            {
                var key = $"update-{TableName}";
                string command = commandTexts.GetOrAdd(key, k =>
                {
                    var assignColumns = Columns.Where(c => !c.IsKey && !c.AutoIncrement);
                    var keyColumns = Columns.Where(c => c.IsKey);

                    string assign = assignColumns.JoinToString(",", c => $"{c.ColumnName}={c.MySQLParameterName}");
                    string where = keyColumns.JoinToString(" AND ", c => $"{c.ColumnName}={c.MySQLParameterName}");
                    string select = Columns.JoinToString(",", c => c.ColumnName);

                    return $"UPDATE {TableName} SET {assign} WHERE {where}; SELECT {select} FROM {TableName} WHERE {where};";
                });

                MySQLDatabaseRequest request = new MySQLDatabaseRequest(command);
                request.AddParameters(Columns, entity);

                return request;
            }

            public static IDatabaseRequest GenerateReteriveRequest(TEntity entity)
            {
                var keyColumns = Columns.Where(c => c.IsKey);
                var key = $"reterive-{TableName}";
                string command = commandTexts.GetOrAdd(key, k =>
                {
                    return "SELECT {0} From {1} WHERE {2};".FormatInvariant(
                        Columns.JoinToString(",", c => c.ColumnName),
                        TableName,
                        keyColumns.JoinToString(",", c => $"{c.ColumnName}={c.MySQLParameterName}"));
                });

                MySQLDatabaseRequest request = new MySQLDatabaseRequest(command);
                request.AddParameters(keyColumns, entity);

                return request;
            }

            public static IDatabaseRequest GenerateInsertOrUpdateRequest(TEntity entity)
            {
                var key = $"insert-update-{TableName}";
                string command = commandTexts.GetOrAdd(key, k =>
                {
                    var keyColumns = Columns.Where(c => c.IsKey);
                    var assignColumns = Columns.Where(c => !c.AutoIncrement);
                    var assign = assignColumns.JoinToString(",", c => c.ColumnName);
                    var values = assignColumns.JoinToString(",", c => c.MySQLParameterName);
                    var update = assignColumns.Except(keyColumns).JoinToString(",", c => $"{c.ColumnName}={c.MySQLParameterName}");
                    var select = Columns.JoinToString(",", c => c.ColumnName);
                    var where = keyColumns.JoinToString(" AND ", c => $"{c.ColumnName}={c.MySQLParameterName}");

                    return $"INSERT INTO {TableName} ({assign}) VALUES ({values}) ON DUPLICATE KEY UPDATE {update}; SELECT {select} FROM {TableName} WHERE {where};";
                });

                MySQLDatabaseRequest request = new MySQLDatabaseRequest(command);
                request.AddParameters(Columns, entity);

                return request;
            }

            public static IDatabaseRequest GenerateQueryRequest(Expression<Func<TEntity, bool>> expression)
            {
                var translator = new MySQLQueryTranslator();
                var command = translator.Translate(expression);
                var selectCommand = $"SELECT {Columns.JoinToString(",", c => c.ColumnName)} FROM {TableName} WHERE {command.Command};";
                MySQLDatabaseRequest request = new MySQLDatabaseRequest(selectCommand);
                foreach (var parameter in command.Parameters)
                {
                    request.AddParameter(parameter.ToMySqlParameter());
                }

                return request;
            }

            public static IDatabaseRequest GenerateCreateTableRequest()
            {
                var key = $"create-{TableName}";
                string command = commandTexts.GetOrAdd(key, k =>
                {
                    List<string> lines = new List<string>();
                    var typeMap = new Dictionary<MySqlDbType, string>
                    {
                        {MySqlDbType.Int16,"SMALLINT" },
                        {MySqlDbType.Int24,"MEDIUMINT" },
                        {MySqlDbType.Int32,"INT" },
                        {MySqlDbType.Int64,"BIGINT" },
                        {MySqlDbType.UInt16,"SMALLINT" },
                        {MySqlDbType.UInt24,"MEDIUMINT" },
                        {MySqlDbType.UInt32,"INT" },
                        {MySqlDbType.UInt64,"BIGINT" },
                        {MySqlDbType.Newdate,"DATE" },
                        {MySqlDbType.VarString,"VARCHAR" },
                        {MySqlDbType.NewDecimal,"DECIMAL" },
                        {MySqlDbType.Enum,"INT" },
                        {MySqlDbType.Set,"VARCHAR" },
                        {MySqlDbType.String,"NVARCHAR" },
                        {MySqlDbType.Guid,"VARCHAR(36)" },
                        {MySqlDbType.Text,"NVARCHAR" }
                    };
                    lines.Add($"CREATE TABLE IF NOT EXISTS {TableName} (");
                    lines.AddRange(Columns.Select(c =>
                    {
                        var type = QueryParameter.GetMySqlDbType(Type.GetTypeCode(c.DataType));
                        string columnType = typeMap.ContainsKey(type) ? typeMap[type] : type.ToString().ToUpper();
                        string line = $"{c.ColumnName} {columnType}";
                        if (c.Length > 0)
                        {
                            line += $"({c.Length})";
                        }

                        if (!c.Nullable)
                        {
                            line += " NOT NULL";
                        }

                        if (c.AutoIncrement)
                        {
                            line += " AUTO_INCREMENT";
                        }

                        line += ",";
                        return line;
                    }));

                    if (Columns.Any(c => c.IsKey))
                    {
                        lines.Add($"PRIMARY KEY ({Columns.Where(c => c.IsKey).JoinToString(",", c => c.ColumnName)})");
                    }
                    lines.Add($") Engine=InnoDB  DEFAULT CHARSET=utf8;");

                    return lines.JoinToString(Environment.NewLine);
                });

                MySQLDatabaseRequest request = new MySQLDatabaseRequest(command);
                return request;
            }

            public static IDatabaseRequest GenerateDeleteTableRequest()
            {
                return new MySQLDatabaseRequest($"DROP TABLE IF EXISTS {TableName};");
            }
        }
    }
}
