using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UCenter.Common.Exceptions;
using UCenter.Common.Models;

namespace UCenter.Common.Database
{
    [Export(typeof(IDatabaseClient))]
    public class MySQLDatabaseClient : IDatabaseClient, IDatabaseClient<MySQLDatabaseRequest>
    {
        private readonly DatabaseContext context;

        [ImportingConstructor]
        public MySQLDatabaseClient(DatabaseContext context)
        {
            this.context = context;
        }

        public string ConnectionString
        {
            get
            {
                return this.context.ConnectionString;
            }
        }

        // The MySQL.Data have a bug that it not readly have an async reader function
        // so all of the following async function won't be really async.
        // please referance to: https://bugs.mysql.com/bug.php?id=70111
        async Task<TResponse> IDatabaseClient<MySQLDatabaseRequest>.ExecuteSingleAsync<TResponse>(MySQLDatabaseRequest request, CancellationToken token)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(this.ConnectionString))
                using (MySqlCommand command = request.GetCommand(connection))
                {
                    await connection.OpenAsync(token);

                    var reader = await command.ExecuteReaderAsync(token);
                    if (await reader.ReadAsync(token))
                    {
                        return MySQLResponseRawGenerator<TResponse>.Generate(request, reader);
                    }
                    else
                    {
                        return default(TResponse);
                    }
                }
            }
            catch (MySqlException ex)
            {
                // in normally, we should not re-throw an exception, here is for multi type database support
                // we donot want to change up level code when the database change to some other type.
                throw new DatabaseException(ex);
            }
        }

        async Task<int> IDatabaseClient<MySQLDatabaseRequest>.ExecuteNoQueryAsync(MySQLDatabaseRequest request, CancellationToken token)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(this.ConnectionString))
                using (MySqlCommand command = request.GetCommand(connection))
                {
                    await connection.OpenAsync(token);

                    return await command.ExecuteNonQueryAsync(token);
                }
            }
            catch (MySqlException ex)
            {
                // in normally, we should not re-throw an exception, here is for multi type database support
                // we donot want to change up level code when the database change to some other type.
                throw new DatabaseException(ex);
            }
        }

        async Task<ICollection<TResponse>> IDatabaseClient<MySQLDatabaseRequest>.ExecuteListAsync<TResponse>(MySQLDatabaseRequest request, CancellationToken token)
        {
            try
            {
                var result = new List<TResponse>();
                using (MySqlConnection connection = new MySqlConnection(this.ConnectionString))
                using (MySqlCommand command = request.GetCommand(connection))
                {
                    await connection.OpenAsync(token);

                    var reader = await command.ExecuteReaderAsync(token);
                    while (await reader.ReadAsync(token))
                    {
                        result.Add(MySQLResponseRawGenerator<TResponse>.Generate(request, reader));
                    }
                }

                return result;
            }
            catch (MySqlException ex)
            {
                // in normally, we should not re-throw an exception, here is for multi type database support
                // we donot want to change up level code when the database change to some other type.
                throw new DatabaseException(ex);
            }
        }

        public Task<TResponse> ExecuteSingleAsync<TResponse>(IDatabaseRequest request, CancellationToken token)
        {
            return (this as IDatabaseClient<MySQLDatabaseRequest>).ExecuteSingleAsync<TResponse>(request as MySQLDatabaseRequest, token);
        }

        public Task<int> ExecuteNoQueryAsync(IDatabaseRequest request, CancellationToken token)
        {
            return (this as IDatabaseClient<MySQLDatabaseRequest>).ExecuteNoQueryAsync(request as MySQLDatabaseRequest, token);
        }

        public Task<ICollection<TResponse>> ExecuteListAsync<TResponse>(IDatabaseRequest request, CancellationToken token)
        {
            return (this as IDatabaseClient<MySQLDatabaseRequest>).ExecuteListAsync<TResponse>(request as MySQLDatabaseRequest, token);
        }

        private static class MySQLResponseRawGenerator<TResponse>
        {
            private static ConcurrentDictionary<string, Func<DbDataReader, TResponse>> ResponseGeneraterFunctions = new ConcurrentDictionary<string, Func<DbDataReader, TResponse>>();

            private static string keyPrefix = typeof(TResponse).FullName;

            public static TResponse Generate(MySQLDatabaseRequest request, DbDataReader reader)
            {
                // we assume if the command text and the reture type not changed, the generater should be the same one.
                string key = $"{keyPrefix}-{request.CommandText}";
                var generater = GetGeneraterFunction(key, reader);
                return generater(reader);
            }

            private static Func<DbDataReader, TResponse> GetGeneraterFunction(string key, DbDataReader reader)
            {
                return ResponseGeneraterFunctions.GetOrAdd(key, k => BuildApplyValueAction(reader));
            }

            private static Func<DbDataReader, TResponse> BuildApplyValueAction(DbDataReader reader)
            {
                var expressions = new List<Expression>();

                var parameterExpression = Expression.Parameter(typeof(IDataRecord), "reader");

                var resultExpression = Expression.Variable(typeof(TResponse));
                expressions.Add(Expression.Assign(resultExpression, Expression.New(resultExpression.Type)));

                var indexerInfo = typeof(IDataRecord).GetProperty("Item", new[] { typeof(int) });

                var columnNames = Enumerable.Range(0, reader.FieldCount)
                                            .Select(i => new { Index = i, Name = reader.GetName(i), Type = reader[i].GetType() });

                var entityProperties = resultExpression.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Select(p =>
                    {
                        return new { Property = p, Name = p.GetDbColumnName() };
                    });
                foreach (var column in columnNames)
                {
                    var property = entityProperties.FirstOrDefault(p => string.Compare(p.Name, column.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
                    if (property == null)
                    {
                        continue;
                    }

                    var columnIndexExpression = Expression.Constant(column.Index);
                    var propertyExpression = Expression.MakeIndex(
                        parameterExpression, indexerInfo, new[] { columnIndexExpression });

                    var method = property.Property.PropertyType.GetMethod("op_Implicit", new Type[] { column.Type });
                    Expression convertExpression = null;
                    if (method != null)
                    {
                        convertExpression = Expression.Call(method, Expression.Convert(propertyExpression, column.Type));
                    }
                    else if (property.Property.PropertyType.IsEnum)
                    {
                        convertExpression = Expression.Convert(Expression.Convert(propertyExpression, Enum.GetUnderlyingType(property.Property.PropertyType)), property.Property.PropertyType);
                    }
                    else
                    {
                        convertExpression = Expression.Convert(propertyExpression, property.Property.PropertyType);
                    }

                    var bindExpression = Expression.Assign(
                        Expression.Property(resultExpression, property.Property), convertExpression);
                    expressions.Add(bindExpression);
                }

                expressions.Add(resultExpression);
                return Expression.Lambda<Func<DbDataReader, TResponse>>(
                    Expression.Block(new[] { resultExpression }, expressions), parameterExpression).Compile();
            }
        }
    }
}
