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

        ConnectionFactory<TConnection> ConnectionFactory { get; set; }

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
            return ExecuteReaderInternal(connectionFactory, procName, parameters).FirstOrDefault();
        }

        public void Execute(string procName)
        {
            ExecuteNonQuery(ConnectionFactory, procName);
        }

        public Task ExecuteAsync(string procName)
        {
            return Task.Factory.StartNew(() => Execute(procName));
        }

        public void ExecuteNonQuery(string procName, params TParameter[] parameters)
        {
            ExecuteNonQuery(ConnectionFactory, procName, parameters);
        }

        public Task ExecuteNonQueryAsync(string procName, params TParameter[] parameters)
        {
            return Task.Factory.StartNew(() => ExecuteNonQuery(procName, parameters));
        }

        void ExecuteNonQuery(ConnectionFactory<TConnection> connectionFactory, string procName, params TParameter[] parameters)
        {
            try
            {
                using (var connection = connectionFactory.GetNewConnection())
                {
                    connection.Open();

                    using (var command = PrepareCommand(connection, CommandType.StoredProcedure, procName, parameters.EmptyIfNull()))
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

            return MapToClrValue<T>(result);
        }

        public Task<T> ExecuteScalarAsync<T>(string procName)
        {
            return Task.Factory.StartNew(() => ExecuteScalar<T>(procName));
        }

        public T ExecuteScalar<T>(string procName, IEnumerable<TParameter> parameters)
        {
            var result = ExecuteScalarInternalWithRetry(ConnectionFactory, procName, parameters);

            if (result == null && !typeof(T).IsNullable())
                throw new InvalidCastException(
                    "{0} stored procedure returned NULL but expected type is non-nullable {1}"
                    .FormatWith(procName, typeof(T).Name));

            return MapToClrValue<T>(result);
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
                    var result = command.ExecuteScalar();

                    // NOTE:    reference to parameters may be held up the call stack
                    //          which will prevent command from being disposed properly and parameters from being reused.
                    //          we will now clear command's parameters to prevent this from happening
                    command.Parameters.Clear();

                    return result;
                }
            }
        }

        protected internal virtual TCommand PrepareCommand(TConnection connection, CommandType commandType, string commandText, IEnumerable<TParameter> parameters)
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

    class CommandResultIterator<TEntity> : IEnumerable<TEntity>
    {
        readonly Func<IEnumerator<TEntity>> GetEnumeratorFunc;

        public CommandResultIterator(Func<IEnumerator<TEntity>> getEnumerator)
        {
            this.GetEnumeratorFunc = getEnumerator;
        }
        public IEnumerator<TEntity> GetEnumerator()
        {
            return GetEnumeratorFunc();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumeratorFunc();
        }
    }

    class Iterator<TSource> : 
        IEnumerable<TSource>, 
        IEnumerable
    {        
        protected Func<IEnumerator<TSource>> CreateNewEnumerator { get; private set; }

        public Iterator(Func<IEnumerator<TSource>> createNewEnumerator)
        {
            this.CreateNewEnumerator = createNewEnumerator;
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            return CreateNewEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }
    }

    class CommandResultEnumerator<TConnection, TCommand, TDataReader, TParameter, TEntity> : EnumeratorBase<TEntity>
        where TConnection : DbConnection, new()
        where TCommand : DbCommand
        where TParameter : DbParameter
        where TDataReader : DbDataReader
    {
        protected ConnectionFactory<TConnection> ConnectionFactory { get; private set; }
        protected TConnection Connection { get; private set; }
        protected string CommandText { get; private set; }
        protected IEnumerable<TParameter> Parameters { get; private set; }
        protected Func<TDataReader, TEntity> CreateEntity { get; private set; }
        protected TCommand Command { get; private set; }
        protected TDataReader DataReader { get; private set; }

        public CommandResultEnumerator(
            DataAccessService<TConnection, TCommand, TParameter, TDataReader> dataAccessService,
            ConnectionFactory<TConnection> connectionFactory,
            string commandText,
            IEnumerable<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            try
            {
                this.ConnectionFactory = connectionFactory;
                this.CommandText = commandText;
                this.Parameters = parameters;
                this.CreateEntity = createEntity;

                this.Connection = connectionFactory.GetNewConnection();

                Connection.Open();

                this.Command = dataAccessService.PrepareCommand(Connection, CommandType.StoredProcedure, CommandText, parameters);

                this.DataReader = (TDataReader) Command.ExecuteReader();
            }
            catch(Exception ex)
            {
                var ex_new = new Exception("Unable to intitialize command.", ex);

                ex_new.TryAddContextData("connection factory", () => connectionFactory.ToString());
                ex_new.TryAddContextData("command", () => commandText);
                ex_new.TryAddContextData("parameters", () => parameters);
                ex_new.TryAddContextData("connection", () => Connection);

                throw ex_new;
            }
        }

        public override void Reset()
        {
            throw new NotSupportedException();
        }

        public override bool MoveNext()
        {
            try
            {
                if (DataReader.Read())
                {
                    Current = CreateEntity(DataReader);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                var ex_new = new Exception("Failed during command execution.", ex);

                ex_new.TryAddContextData("connection factory", () => ConnectionFactory.ToString());
                ex_new.TryAddContextData("command", () => CommandText);
                ex_new.TryAddContextData("parameters", () => Parameters);
                ex_new.TryAddContextData("connection", () => Connection);

                throw ex_new;
            }
        }

        protected override void DisposeManagedResources()
        {
 	         if(DataReader != null)
             {
                 DataReader.Dispose();
                 DataReader = null;
             }

            if(Connection != null)
            {
                Connection.Dispose();
                Connection = null;
            }

            if (Command != null)
            {
                // NOTE:    reference to parameters may be held up the call stack
                //          which will prevent command from being disposed properly and parameters from being reused.
                //          we will now clear command's parameters to prevent this from happening
                Command.Parameters.Clear();

                Command.Dispose();
                Command = null;
            }
        }
    }
}

    public interface IDatabaseObjectNameResolver
    {
        string GetActualStoredProcedureName(string storedProcedureName);
        string GetActualScalarFunctionName(string scalarFunctionName);
    }

    public class SqlDatabaseObjectNameResolver : IDatabaseObjectNameResolver
    {
        SqlDataAccessService2 _dataAccessService;
        protected SqlDataAccessService2 DataAccessService
        {
            get { return _dataAccessService; }
            private set { _dataAccessService = value; }
        }

        public SqlDatabaseObjectNameResolver(SqlDataAccessService2 dataAccessService)
        {
            this.DataAccessService = dataAccessService;
        }

        public virtual string GetActualStoredProcedureName(string storedProcedureName)
        {
            return storedProcedureName;
        }

        public virtual string GetActualScalarFunctionName(string scalarFunction)
        {
            return scalarFunction;
        }
    }

    // todo: make connection factory protected

    public class SqlDataAccessService_TEMP : SqlDataAccessService
    {
        IDatabaseObjectNameResolver _databaseObjectNameResolver;
        public IDatabaseObjectNameResolver DatabaseObjectNameResolver
        {
            get { return _databaseObjectNameResolver; }
            set { _databaseObjectNameResolver = value; }
        }

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
                return ExecuteScalarText<bool>("select case when OBJECT_ID('{0}') IS NULL then 0 else 1 end".FormatWith(objectName));
            }
            else
            {
                return ExecuteScalarText<bool>("select case when OBJECT_ID('{0}', '{1}') IS NULL then 0 else 1 end".FormatWith(objectName, objectType));
            }
        }

        public T ExecuteScalarText<T>(string sql) { }

        // todo: ExecuteScalarTextInternalWithRetry and ExecuteScalarTextInternal, instead of new method, make CommandType a parameter
        // also make protected

        protected override SqlCommand PrepareCommand(SqlConnection connection, CommandType commandType, string commandText, IEnumerable<SqlParameter> parameters)
        {
            var nameResolver = DatabaseObjectNameResolver;

            if (nameResolver != null)
            {
                if (commandType == CommandType.StoredProcedure)
                {
                    commandText = nameResolver.GetActualStoredProcedureName(commandText);
                }
            }

            return base.PrepareCommand(connection, commandType, commandText, parameters);
        }
    }
