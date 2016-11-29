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
using SquaredInfinity.Foundation.Diagnostics;
using System.Collections;
using System.Threading;

namespace SquaredInfinity.Foundation.Data
{
    public abstract partial class DataAccessService<TConnection, TCommand, TParameter, TDataReader>
        where TConnection : DbConnection, new()
        where TCommand : DbCommand
        where TParameter : DbParameter
        where TDataReader : DbDataReader
    {
        protected ILogger Logger { get; private set; }

        protected ConnectionFactory<TConnection> ConnectionFactory { get; set; }

        IDatabaseObjectNameResolver _databaseObjectNameResolver;
        public IDatabaseObjectNameResolver DatabaseObjectNameResolver
        {
            get { return _databaseObjectNameResolver; }
            set { _databaseObjectNameResolver = value; }
        }

        public TimeSpan DefaultCommandTimeout { get; set; }

        public RetryPolicy RetryPolicy { get; set; }

        public DataAccessService(
            ConnectionFactory<TConnection> connectionFactory,
            TimeSpan defaultCommandTimeout)
            : this(null, connectionFactory, defaultCommandTimeout) 
            // todo: get default (static) logger using LoggerLocator.GetLogger().CreateLoggerForType<>() and here just assing this static logger
            // by default this whould be InternalTrace limited to SquaredInfinity.Foundation.Data
        { }

        public DataAccessService(
            ILogger logger,
            ConnectionFactory<TConnection> connectionFactory,
            TimeSpan defaultCommandTimeout)
        {
            this.Logger = logger;
            this.ConnectionFactory = connectionFactory;
            this.DefaultCommandTimeout = defaultCommandTimeout;
        }

        public IEnumerable<TEntity> ExecuteReader<TEntity>(
            string procName,
            Func<TDataReader, TEntity> createEntity)
        {
            return ExecuteReaderInternalWithRetry<TEntity>(ConnectionFactory, procName, new TParameter[0], createEntity);
        }

        public async Task<List<TEntity>> ExecuteReaderAsync<TEntity>(
            string procName,
            Func<TDataReader, TEntity> createEntity)
        {
            return await Task.Run(() => ExecuteReader<TEntity>(procName, createEntity).ToList()).ConfigureAwait(continueOnCapturedContext: false);
        }

        #region Execute Reader - create entity

        public IEnumerable<TEntity> ExecuteReader<TEntity>(
            string procName,
            IEnumerable<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            return ExecuteReaderInternalWithRetry<TEntity>(ConnectionFactory, procName, parameters, createEntity);
        }

        public async Task<List<TEntity>> ExecuteReaderAsync<TEntity>(
            string procName,
            IEnumerable<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            return await Task.Run(() => ExecuteReader<TEntity>(procName, parameters, createEntity).ToList()).ConfigureAwait(continueOnCapturedContext: false);
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
            var iterator = new Iterator<TEntity>(() =>
                {
                    return
                        new CommandResultEnumerator<TConnection, TCommand, TDataReader, TParameter, TEntity>(
                            this,
                            connectionFactory,
                            procName,
                            parameters,
                            createEntity);
                });

            return iterator;
        }

        #endregion

        #region Execute Reader - process each row

        public void ExecuteReader(
            string procName,
            IEnumerable<TParameter> parameters,
            Action<TDataReader> processReader)
        {
            ExecuteReaderInternalWithRetry(ConnectionFactory, procName, parameters, processReader);
        }

        public async Task ExecuteReaderAsync<TEntity>(
            string procName,
            IEnumerable<TParameter> parameters,
            Action<TDataReader> processReader)
        {
            await Task.Run(() => ExecuteReader(procName, parameters, processReader)).ConfigureAwait(continueOnCapturedContext: false);
        }

        void ExecuteReaderInternalWithRetry(
            ConnectionFactory<TConnection> connectionFactory,
            string procName,
            IEnumerable<TParameter> parameters,
            Action<TDataReader> processReader)
        {
            RetryPolicy.Execute(() =>
                {
                    ExecuteReaderInternal(
                        connectionFactory,
                        procName,
                        parameters,
                        processReader);
                });
        }

        void ExecuteReaderInternal(
            ConnectionFactory<TConnection> connectionFactory,
            string procName,
            IEnumerable<TParameter> parameters,
            Action<TDataReader> processReader)
        {
            var iterator = new Iterator<TDataReader>(() =>
            {
            return
                new CommandResultEnumerator<TConnection, TCommand, TDataReader, TParameter, TDataReader>(
                    this,
                    connectionFactory,
                    procName,
                    parameters,
                    (r) =>
                    {
                        processReader(r);
                        return r;
                    });
            });

            iterator.WalkAllItems();
        }

        #endregion

        public IEnumerable<Dictionary<string, object>> ExecuteReader(
            string procName)
        {
            return ExecuteReader(procName, new TParameter[0]);
        }

        public async Task<List<Dictionary<string, object>>> ExecuteReaderAsync(
            string procName)
        {
            return await Task.Run(() => ExecuteReader(procName).ToList()).ConfigureAwait(continueOnCapturedContext: false);
        }

        public IEnumerable<Dictionary<string, object>> ExecuteReader(
            string procName,
            IEnumerable<TParameter> parameters)
        {
            return ExecuteReaderInternalWithRetry(ConnectionFactory, procName, parameters);
        }

        public async Task<List<Dictionary<string, object>>> ExecuteReaderAsync(
            string procName,
            IEnumerable<TParameter> parameters)
        {
            return await Task.Run(() => ExecuteReader(procName, parameters).ToList()).ConfigureAwait(continueOnCapturedContext: false);
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
            var iterator = new Iterator<Dictionary<string, object>>(() =>
            {
                return
                    new CommandResultEnumerator<TConnection, TCommand, TDataReader, TParameter, Dictionary<string, object>>(
                        this,
                        connectionFactory,
                        procName,
                        parameters,
                        (r) =>
                        {
                            var result = new Dictionary<string, object>(capacity: r.FieldCount);

                            for (int i = 0; i < r.FieldCount; i++)
                            {
                                result.Add(r.GetName(i), r[i]);
                            }

                            return result;
                        });
            });

            return iterator;
        }

        public Dictionary<string, object> ExecuteReaderSingleRow(string procName)
        {
            return ExecuteReaderSingleRowInternalWithRetry(ConnectionFactory, procName, new TParameter[0]);
        }

        public async Task<Dictionary<string, object>> ExecuteReaderSingleRowAsync(string procName)
        {
            return await Task.Run(() => ExecuteReaderSingleRow(procName)).ConfigureAwait(continueOnCapturedContext: false);
        }

        public Dictionary<string, object> ExecuteReaderSingleRow(string procName, IEnumerable<TParameter> parameters)
        {
            return ExecuteReaderSingleRowInternalWithRetry(ConnectionFactory, procName, parameters);
        }

        public async Task<Dictionary<string, object>> ExecuteReaderSingleRowAsync(string procName, IEnumerable<TParameter> parameters)
        {
            return await Task.Run(() => ExecuteReaderSingleRow(procName, parameters)).ConfigureAwait(continueOnCapturedContext: false);
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
            return ExecuteReaderInternal(connectionFactory, procName, parameters).FirstOrDefault();
        }

        public void Execute(string procName)
        {
            ExecuteNonQuery(ConnectionFactory, CommandType.StoredProcedure, procName);
        }

        public async Task ExecuteAsync(string procName)
        {
            await Task.Run(() => Execute(procName)).ConfigureAwait(continueOnCapturedContext: false);
        }

        public void ExecuteNonQuery(string procName, params TParameter[] parameters)
        {
            ExecuteNonQuery(ConnectionFactory, CommandType.StoredProcedure, procName, parameters);
        }

        public async Task ExecuteNonQueryAsync(string procName, params TParameter[] parameters)
        {
            await Task.Run(() => ExecuteNonQuery(procName, parameters)).ConfigureAwait(continueOnCapturedContext: false);
        }

        public void ExecuteNonQueryText(string sql)
        {
            ExecuteNonQuery(ConnectionFactory, CommandType.Text, sql, new TParameter[0]);
        }

        void ExecuteNonQuery(ConnectionFactory<TConnection> connectionFactory, CommandType commandType, string procName, params TParameter[] parameters)
        {
            try
            {
                using (var connection = connectionFactory.GetNewConnection())
                {
                    connection.Open();

                    using (var command = PrepareCommand(connection, commandType, procName, parameters.EmptyIfNull()))
                    {
                        command.ExecuteNonQuery();

                        // NOTE:    reference to parameters may be held up the call stack
                        //          which will prevent command from being disposed properly and parameters from being reused.
                        //          we will now clear command's parameters to prevent this from happening
                        command.Parameters.Clear();
                    }
                }
            }
            catch(Exception ex)
            {
                var ex_new = new Exception("Failed during command execution.", ex);

                ex_new.TryAddContextData("connection factory", () => connectionFactory.ToString());
                ex_new.TryAddContextData("command", () => procName);
                ex_new.TryAddContextData("parameters", () => parameters);

                throw ex_new;
            }
        }

        public T ExecuteScalarFunction<T>(string functionName)
        {
            var resultParameter = CreateParameter("", null);
            resultParameter.Direction = ParameterDirection.ReturnValue;

            var parameters = new List<TParameter>();
            parameters.Add(resultParameter);

            ExecuteScalarInternalWithRetry(ConnectionFactory, CommandType.StoredProcedure, functionName, parameters);

            var result = resultParameter.Value;

            return MapToClrValue<T>(result);
        }

        public T ExecuteScalarFunction<T>(string functionName, IEnumerable<TParameter> parameters)
        {
            var resultParameter = CreateParameter("", null);
            resultParameter.Direction = ParameterDirection.ReturnValue;

            var all_parameters = parameters.ToList();

            all_parameters.Add(resultParameter);

            ExecuteScalarInternalWithRetry(ConnectionFactory, CommandType.StoredProcedure, functionName, all_parameters);

            var result = resultParameter.Value;

            return MapToClrValue<T>(result);
        }

        public T ExecuteScalar<T>(string procName)
        {
            var result = ExecuteScalarInternalWithRetry(ConnectionFactory, CommandType.StoredProcedure, procName, new TParameter[0]);

            if (result == null && !typeof(T).IsNullable())
                throw new InvalidCastException(
                    $"{procName} stored procedure returned NULL but expected type is non-nullable {typeof(T).Name}");

            return MapToClrValue<T>(result);
        }

        public async Task<T> ExecuteScalarAsync<T>(string procName)
        {
            return await Task.Run(() => ExecuteScalar<T>(procName)).ConfigureAwait(continueOnCapturedContext: false);
        }

        public T ExecuteScalar<T>(string procName, IEnumerable<TParameter> parameters)
        {
            var result = ExecuteScalarInternalWithRetry(ConnectionFactory, CommandType.StoredProcedure, procName, parameters);

            if (result == null && !typeof(T).IsNullable())
                throw new InvalidCastException(
                    $"{procName} stored procedure returned NULL but expected type is non-nullable {typeof(T).Name}");

            return MapToClrValue<T>(result);
        }

        public async Task<T> ExecuteScalarAsync<T>(string procName, IEnumerable<TParameter> parameters)
        {
            return await Task.Run(() => ExecuteScalar<T>(procName, parameters)).ConfigureAwait(continueOnCapturedContext: false);
        }

        object ExecuteScalarInternalWithRetry(
            ConnectionFactory<TConnection> connectionFactory, 
            CommandType commandType,
            string commandText, 
            IEnumerable<TParameter> parameters)
        {
            return RetryPolicy
                .Execute(() =>
                {
                    return ExecuteScalarInternal(
                        connectionFactory,
                        commandType,
                        commandText,
                        parameters);
                });
        }

        protected object ExecuteScalarInternal(
            ConnectionFactory<TConnection> connectionFactory,
            CommandType commandType,
            string commandText,
            IEnumerable<TParameter> parameters)
        {
            using (var connection = connectionFactory.GetNewConnection())
            {
                connection.Open();

                using (var command = PrepareCommand(connection, commandType, commandText, parameters))
                {
                    var result = command.ExecuteScalar();

                    // NOTE:    reference to parameters may be held up the call stack
                    //          which will prevent command from being disposed properly and parameters from being reused.
                    //          we will now clear command's parameters to prevent this from happening
                    command.Parameters.Clear();

                    return result;
                }
            }
        }

        public virtual void Execute(Action<TConnection> execute)
        {
            RetryPolicy
                .Execute(() =>
                {
                    ExecuteInternal(execute);
                });
        }

        public virtual void ExecuteInternal(Action<TConnection> execute)
        {
            using (var connection = ConnectionFactory.GetNewConnection())
            {
                connection.Open();

                execute(connection);
            }
        }

        public T ExecuteScalarText<T>(string sql)
        {
            var result = ExecuteScalarInternalWithRetry(ConnectionFactory, CommandType.Text, sql, new TParameter[0]);

            if (result == null && !typeof(T).IsNullable())
                throw new InvalidCastException(
                    $"{sql} command returned NULL but expected type is non-nullable {typeof(T).Name}");

            return MapToClrValue<T>(result);
        }

        protected internal virtual TCommand PrepareCommand(TConnection connection, CommandType commandType, string commandText, IEnumerable<TParameter> parameters)
        {
            var nameResolver = DatabaseObjectNameResolver;

            if (nameResolver != null)
            {
                if (commandType == CommandType.StoredProcedure)
                {
                    commandText = nameResolver.GetActualStoredProcedureOrFunctionName(commandText);
                }
            }

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

        public abstract TParameter CreateOutParameter(string parameterName, DbType type);

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
                    $"Unable to Map type {sourceType.Name} to {targetType.Name}.");

                // todo: add context data AssemblyQName
            }
        }


        #region Check XXX Exists

        public bool CheckStoredProcedureExists(string storedProcedureName)
        {
            // "P" means Stored Procedure, see http://msdn.microsoft.com/en-us/library/ms190324.aspx
            return CheckDatabaseObjectExists(storedProcedureName, "P");
        }

        public bool CheckScalarFunctionExists(string functionName)
        {
            // "FN" means Scalar Function, see http://msdn.microsoft.com/en-us/library/ms190324.aspx
            return CheckDatabaseObjectExists(functionName, "FN");
        }

        public bool CheckViewExists(string viewName)
        {
            // "V" means View, see http://msdn.microsoft.com/en-us/library/ms190324.aspx
            return CheckDatabaseObjectExists(viewName, "V");
        }

        /// <summary>
        /// see http://msdn.microsoft.com/en-us/library/ms190324.aspx
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public bool CheckDatabaseObjectExists(string objectName, string objectType = null)
        {
            if (objectType == null)
            {
                return ExecuteScalarText<bool>($"select case when OBJECT_ID('{objectName}') IS NULL then 0 else 1 end");
            }
            else
            {
                return ExecuteScalarText<bool>($"select case when OBJECT_ID('{objectName}', '{objectType}') IS NULL then 0 else 1 end");
            }
        }

        #endregion
    }
}
