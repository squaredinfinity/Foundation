using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Data
{
    public abstract partial class DataAccessService<TConnection, TCommand, TParameter, TDataReader>
        where TConnection : DbConnection, new()
        where TCommand : DbCommand
        where TParameter : DbParameter
        where TDataReader : DbDataReader
    {
        ConnectionFactory<TConnection> ConnectionFactory { get; set; }

        public TimeSpan DefaultCommandTimeout { get; set; }

        public RetryPolicy RetryPolicy { get; set; }

        public DataAccessService(
            ConnectionFactory<TConnection> connectionFactory,
            TimeSpan defaultCommandTimeout)
        {
            this.ConnectionFactory = connectionFactory;
            this.DefaultCommandTimeout = defaultCommandTimeout;
        }

        public IEnumerable<TEntity> ExecuteReader<TEntity>(
            string procName,
            Func<TDataReader, TEntity> createEntity)
        {
            return ExecuteReaderInternalWithRetry<TEntity>(ConnectionFactory, procName, new List<TParameter>(), createEntity);
        }

        public Task<List<TEntity>> ExecuteReaderAsync<TEntity>(
            string procName,
            Func<TDataReader, TEntity> createEntity)
        {
            return Task.Factory.StartNew(() => ExecuteReader<TEntity>(procName, createEntity).ToList());
        }


        public IEnumerable<TEntity> ExecuteReader<TEntity>(
            string procName,
            IEnumerable<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            return ExecuteReaderInternalWithRetry<TEntity>(ConnectionFactory, procName, parameters, createEntity);
        }

        public Task<List<TEntity>> ExecuteReaderAsync<TEntity>(
            string procName,
            IEnumerable<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            return Task.Factory.StartNew(() => ExecuteReader<TEntity>(procName, parameters, createEntity).ToList());
        }

        IEnumerable<TEntity> ExecuteReaderInternalWithRetry<TEntity>(
            ConnectionFactory<TConnection> connectionFactory,
            string procName,
            IEnumerable<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            return 
                RetryPolicy.Execute(() => 
                    {
                        return ExecuteReaderInternal<TEntity>(
                            connectionFactory,
                            procName,
                            parameters,
                            createEntity);
                    });
        }

        IEnumerable<TEntity> ExecuteReaderInternal<TEntity>(
            ConnectionFactory<TConnection> connectionFactory,
            string procName,
            IEnumerable<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            using (var connection = connectionFactory.GetNewConnection())
            {
                connection.Open();

                using (var command = PrepareCommand(connection, CommandType.StoredProcedure, procName, parameters))
                {
                    using (var reader = (TDataReader)command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return createEntity(reader);
                        }
                    }
                }
            }
        }

        public IEnumerable<Dictionary<string, object>> ExecuteReader(
            string procName)
        {
            return ExecuteReader(procName, new List<TParameter>());
        }

        public Task<List<Dictionary<string, object>>> ExecuteReaderAsync(
            string procName)
        {
            return Task.Factory.StartNew(() => ExecuteReader(procName).ToList());
        }

        public IEnumerable<Dictionary<string, object>> ExecuteReader(
            string procName,
            IEnumerable<TParameter> parameters)
        {
            return ExecuteReaderInternalWithRetry(ConnectionFactory, procName, parameters);
        }

        public Task<List<Dictionary<string, object>>> ExecuteReaderAsync(
            string procName,
            IEnumerable<TParameter> parameters)
        {
            return Task.Factory.StartNew(() => ExecuteReader(procName, parameters).ToList());
        }

        IEnumerable<Dictionary<string, object>> ExecuteReaderInternalWithRetry(
            ConnectionFactory<TConnection> connectionFactory,
            string procName,
            IEnumerable<TParameter> parameters)
        {
            return RetryPolicy
                .Execute(() =>
                {
                    return ExecuteReaderInternal(
                        connectionFactory,
                        procName,
                        parameters);
                });
        }

        IEnumerable<Dictionary<string, object>> ExecuteReaderInternal(
            ConnectionFactory<TConnection> connectionFactory,
            string procName,
            IEnumerable<TParameter> parameters)
        {
            using (var connection = connectionFactory.GetNewConnection())
            {
                connection.Open();

                using (var command = PrepareCommand(connection, CommandType.StoredProcedure, procName, parameters))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var result = new Dictionary<string, object>(capacity: reader.FieldCount);

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result.Add(reader.GetName(i), reader[i]);
                            }

                            yield return result;
                        }
                    }
                }
            }
        }

        public Dictionary<string, object> ExecuteReaderSingleRow(string procName)
        {
            return ExecuteReaderSingleRowInternalWithRetry(ConnectionFactory, procName, new List<TParameter>());
        }

        public Task<Dictionary<string, object>> ExecuteReaderSingleRowAsync(string procName)
        {
            return Task.Factory.StartNew(() => ExecuteReaderSingleRow(procName));
        }

        public Dictionary<string, object> ExecuteReaderSingleRow(string procName, IEnumerable<TParameter> parameters)
        {
            return ExecuteReaderSingleRowInternalWithRetry(ConnectionFactory, procName, parameters);
        }

        public Task<Dictionary<string, object>> ExecuteReaderSingleRowAsync(string procName, IEnumerable<TParameter> parameters)
        {
            return Task.Factory.StartNew(() => ExecuteReaderSingleRow(procName, parameters));
        }

        Dictionary<string, object> ExecuteReaderSingleRowInternalWithRetry(
           ConnectionFactory<TConnection> connectionFactory,
           string procName,
           IEnumerable<TParameter> parameters)
        {
            return RetryPolicy
                .Execute(() =>
                {
                    return ExecuteReaderSingleRowInternal(
                        connectionFactory, 
                        procName,
                        parameters);
                });
        }

        Dictionary<string, object> ExecuteReaderSingleRowInternal(
            ConnectionFactory<TConnection> connectionFactory,
            string procName,
            IEnumerable<TParameter> parameters)
        {
            var result = (Dictionary<string, object>)null;

            using (var connection = connectionFactory.GetNewConnection())
            {
                connection.Open();

                using (var command = PrepareCommand(connection, CommandType.StoredProcedure, procName, parameters))
                {
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            result = new Dictionary<string, object>(capacity: reader.FieldCount);

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result.Add(reader.GetName(i), reader[i]);
                            }
                        }
                    }
                }
            }

            if (result == null)
                result = new Dictionary<string, object>();

            return result;
        }

        public void Execute(string procName)
        {
            Execute(ConnectionFactory, procName);
        }

        public Task ExecuteAsync(string procName)
        {
            return Task.Factory.StartNew(() => Execute(procName));
        }

        public void Execute(string procName, params TParameter[] parameters)
        {
            Execute(ConnectionFactory, procName, parameters);
        }

        public Task ExecuteAsync(string procName, params TParameter[] parameters)
        {
            return Task.Factory.StartNew(() => Execute(procName, parameters));
        }

        void Execute(ConnectionFactory<TConnection> connectionFactory, string procName, params TParameter[] parameters)
        {
            using (var connection = connectionFactory.GetNewConnection())
            {
                connection.Open();

                using (var command = PrepareCommand(connection, CommandType.StoredProcedure, procName, parameters.EmptyIfNull()))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public T ExecuteScalarFunction<T>(string functionName)
        {
            var resultParameter = CreateParameter("", null);
            resultParameter.Direction = ParameterDirection.ReturnValue;

            var parameters = new List<TParameter>();
            parameters.Add(resultParameter);

            ExecuteScalarInternalWithRetry(ConnectionFactory, functionName, parameters);

            var result = resultParameter.Value;

            return MapToClrValue<T>(result);
        }

        public T ExecuteScalarFunction<T>(string functionName, IEnumerable<TParameter> parameters)
        {
            var resultParameter = CreateParameter("", null);
            resultParameter.Direction = ParameterDirection.ReturnValue;

            var all_parameters = parameters.ToList();

            all_parameters.Add(resultParameter);

            ExecuteScalarInternalWithRetry(ConnectionFactory, functionName, all_parameters);

            var result = resultParameter.Value;

            return MapToClrValue<T>(result);
        }

        public T ExecuteScalar<T>(string procName)
        {
            var result = ExecuteScalarInternalWithRetry(ConnectionFactory, procName, new List<TParameter>());


            if (result == null && !typeof(T).IsNullable())
                throw new InvalidCastException(
                    "{0} stored procedure returned NULL but expected type is non-nullable {1}"
                    .FormatWith(procName, typeof(T).Name));

            return (T) result;
        }

        public Task<T> ExecuteScalarAsync<T>(string procName)
        {
            return Task.Factory.StartNew(() => ExecuteScalar<T>(procName));
        }

        public T ExecuteScalar<T>(string procName, IEnumerable<TParameter> parameters)
        {
            return (T)ExecuteScalarInternalWithRetry(ConnectionFactory, procName, parameters);
        }

        public Task<T> ExecuteScalarAsync<T>(string procName, IEnumerable<TParameter> parameters)
        {
            return Task.Factory.StartNew(() => ExecuteScalar<T>(procName, parameters));
        }

        object ExecuteScalarInternalWithRetry(
            ConnectionFactory<TConnection> connectionFactory, 
            string procname, 
            IEnumerable<TParameter> parameters)
        {
            return RetryPolicy
                .Execute(() =>
                {
                    return ExecuteScalarInternal(
                        connectionFactory,
                        procname,
                        parameters);
                });
        }

        object ExecuteScalarInternal(
            ConnectionFactory<TConnection> connectionFactory, 
            string procname, 
            IEnumerable<TParameter> parameters)
        {
            using (var connection = connectionFactory.GetNewConnection())
            {
                connection.Open();

                using (var command = PrepareCommand(connection, CommandType.StoredProcedure, procname, parameters))
                {
                    return command.ExecuteScalar();
                }
            }
        }

        protected virtual TCommand PrepareCommand(TConnection connection, CommandType commandType, string commandText, IEnumerable<TParameter> parameters)
        {
            var command = connection.CreateCommand();

            command.CommandType = commandType;
            command.CommandText = commandText;
            command.CommandTimeout = (int)DefaultCommandTimeout.TotalMilliseconds;

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }

            return (TCommand)command;
        }


        public abstract TParameter CreateParameter(string parameterName, object clrValue);

        public TTarget MapToClrValue<TTarget>(object dbValue, TTarget defaultValue = default(TTarget))
        {
            TTarget clrTarget = default(TTarget);

            MapToClrValue<TTarget>(dbValue, ref clrTarget, defaultValue);

            return clrTarget;
        }

        public void MapToClrValue<TTarget>(object dbValue, TTarget target, Action<TTarget> setValue, TTarget defaultValue = default(TTarget))
        {
            TTarget clrValue = default(TTarget);

            MapToClrValue<TTarget>(dbValue, ref clrValue, defaultValue);

            setValue(clrValue);
        }

        /// <summary>
        /// Provides a logic for mapping DB values to Clr values.
        /// For example, mapping of DBNull to null.
        /// </summary>
        /// <typeparam name="TTarget">.Net type to map to</typeparam>
        /// <param name="dbValue">DB value</param>
        /// <param name="clrTarget">.Net class or struct member where mapped value will be written to.</param>
        /// <param name="defaultValue">Value to use to substitute NULL</param>
        public virtual void MapToClrValue<TTarget>(object dbValue, ref TTarget clrTarget, TTarget defaultValue = default(TTarget))
        {
            if (dbValue == null)
            {
                clrTarget = defaultValue;
                return;
            }

            if (dbValue == DBNull.Value)
            {
                clrTarget = defaultValue;
                return;
            }

            var targetType = typeof(TTarget);
            var sourceType = dbValue.GetType();

            if (sourceType.IsTypeEquivalentTo(targetType, treatNullableAsEquivalent: true) || sourceType.ImplementsOrExtends(targetType))
            {
                clrTarget = (TTarget)dbValue;
                return;
            }

            //# Timespan
            //  Sql Server does not support TimeSpan directly
            //  Convert from number of ticks instead
            if (targetType == typeof(TimeSpan?) || targetType == typeof(TimeSpan))
            {
                clrTarget = (TTarget)(object)TimeSpan.FromTicks((long)dbValue);
                return;
            }

            //# XDocument
            //  Sql Server does not support XDocument directly
            //  Convert from string representation instead
            if (targetType == typeof(XDocument))
            {
                clrTarget = (TTarget)(object)XDocument.Parse(dbValue as string);
                return;
            }

            //# XElement
            //  Sql Server does not support XElement directly
            //  Convert from string representation instead
            if (targetType == typeof(XElement))
            {
                clrTarget = (TTarget)(object)XElement.Parse(dbValue as string);
                return;
            }

            //# IConvertible
            //  Convert DB value to .NET value if it can be converted without loss of information
            if (dbValue is IConvertible)
            {
                clrTarget = (TTarget)Convert.ChangeType(dbValue, typeof(TTarget));
            }
            else
            {
                var ex = new InvalidCastException(
                    "Unable to Map type {0} to {1}."
                    .FormatWith(
                        sourceType.Name,
                        targetType.Name));

                // todo: add context data AssemblyQName
            }
        }
    }
}
