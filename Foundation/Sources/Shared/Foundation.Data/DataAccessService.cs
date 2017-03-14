using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SquaredInfinity.Extensions;
using SquaredInfinity.Diagnostics;
using System.Collections;
using System.Threading;
using System.Dynamic;
using System.Collections.ObjectModel;

namespace SquaredInfinity.Data
{
    public interface IExecuteOptions
    {
        CancellationToken CancellationToken { get; }
        TimeSpan Timeout { get; }

        bool ShouldAsyncOpenConnection { get; }
        bool ShouldAsyncExecuteCommand { get; }
    }

    public abstract class ExecuteOptions : IExecuteOptions
    {
        public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        public bool ShouldAsyncOpenConnection { get; set; } = false;
        public bool ShouldAsyncExecuteCommand { get; set; } = false;
    }

    public interface IExecuteReaderOptions : IExecuteOptions
    {
        int ExpectedResultCount { get; }
    }

    public class ExecuteReaderOptions : ExecuteOptions, IExecuteReaderOptions
    {
        public int ExpectedResultCount { get; set; } = 21;
    }

    public interface IDataAccessService
    {
        #region Execute All Reader Async (To Dictionary)

        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IExecuteReaderOptions options);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters);
        Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options);

        #endregion

        #region Execute All Reader Async (To Entity)

        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, Func<IDataReader, TEntity> createEntity);
        Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity);

        #endregion

        #region Execute Reader Async (Process Each Row)

        Task ExecuteReaderAsync(string procName, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IExecuteReaderOptions options, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, Action<IDataReader, CancellationToken> processRow);
        Task ExecuteReaderAsync(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, Action<IDataReader, CancellationToken> processRow);

        #endregion

        #region Execute Reader (To Entity)

        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, Func<IDataReader, TEntity> createEntity);
        IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity);

        #endregion
    }

    public abstract partial class DataAccessService<TConnection, TCommand, TParameter, TDataReader> : IDataAccessService
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


        public RetryPolicy RetryPolicy { get; set; } = new RetryPolicy();
        public IExecuteReaderOptions DefaultExecuteReaderOptions { get; set; } = new ExecuteReaderOptions();

        #region Constructors

        public DataAccessService(
            ConnectionFactory<TConnection> connectionFactory,
            TimeSpan defaultCommandTimeout)
            : this(null, connectionFactory, defaultCommandTimeout)
        // todo: get default (static) logger using LoggerLocator.GetLogger().CreateLoggerForType<>() and here just assing this static logger
        // by default this whould be InternalTrace limited to SquaredInfinity.Data
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

        #endregion

        #region Execute All Reader Async (To Dictionary)

        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(
            string procName)
        {
            return
                await
                ExecuteAllReaderAsync(procName, new IDbDataParameter[0], DefaultExecuteReaderOptions)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(
            string procName,
            IExecuteReaderOptions options)
        {
            return
                await
                ExecuteAllReaderAsync(procName, new IDbDataParameter[0], options)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(
            string procName,
            IReadOnlyList<IDbDataParameter> parameters)
        {
            return
                await
                ExecuteAllReaderAsync(procName, parameters, DefaultExecuteReaderOptions)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IReadOnlyList<IReadOnlyDictionary<string, object>>> ExecuteAllReaderAsync(
            string procName,
            IReadOnlyList<IDbDataParameter> parameters,
            IExecuteReaderOptions options)
        {
            return
                await ExecuteAllReaderWithRetryInternalAsync(
                    ConnectionFactory,
                    options,
                    procName,
                    new TParameter[0]
                    , (r) =>
                    {
                        var result = new Dictionary<string, object>(capacity: r.FieldCount);

                        for (int i = 0; i < r.FieldCount; i++)
                        {
                            result.Add(r.GetName(i), r[i]);
                        }

                        return new ReadOnlyDictionary<string, object>(result);
                    })
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        #endregion

        #region Execute All Reader Async (To Entity)

        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(
            string procName,
            Func<IDataReader, TEntity> createEntity)
        {
            return
                await
                ExecuteAllReaderAsync(procName, DefaultExecuteReaderOptions, createEntity)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(
            string procName,
            IExecuteReaderOptions options,
            Func<IDataReader, TEntity> createEntity)
        {
            return
                await
                ExecuteAllReaderAsync(procName, DefaultExecuteReaderOptions, createEntity)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(
            string procName,
            IReadOnlyList<IDbDataParameter> parameters,
            Func<IDataReader, TEntity> createEntity)
        {
            return
               await
               ExecuteAllReaderAsync(procName, new IDbDataParameter[0], DefaultExecuteReaderOptions, createEntity)
               .ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task<IReadOnlyList<TEntity>> ExecuteAllReaderAsync<TEntity>(string procName, IReadOnlyList<IDbDataParameter> parameters, IExecuteReaderOptions options, Func<IDataReader, TEntity> createEntity)
        {
            return
               await
               ExecuteAllReaderWithRetryInternalAsync(
                   ConnectionFactory,
                   options,
                   procName,
                   parameters.Cast<TParameter>().ToArray(),
                   createEntity)
               .ConfigureAwait(continueOnCapturedContext: false);
        }

        #endregion

        #region Execute All Reader Async (Internal)

        protected async Task<IReadOnlyList<TEntity>> ExecuteAllReaderWithRetryInternalAsync<TEntity>(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            var retryOptions = new RetryPolicyOptions
            {
                CancellationToken = options.CancellationToken
            };

            return
                await
                ExecuteAllReaderWithRetryInternalAsync(connectionFactory, options, retryOptions, procName, parameters, createEntity)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        protected async Task<IReadOnlyList<TEntity>> ExecuteAllReaderWithRetryInternalAsync<TEntity>(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            IRetryPolicyOptions retryOptions,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            return
                await
                RetryPolicy
                .ExecuteAsync(retryOptions, async () =>
                {
                    return 
                    await
                    ExecuteAllReaderInternalAsync(
                        ConnectionFactory,
                        options,
                        procName,
                        parameters,
                        createEntity)
                        .ConfigureAwait(continueOnCapturedContext: false);

                }).ConfigureAwait(continueOnCapturedContext: false);
        }

        protected virtual async Task<IReadOnlyList<TEntity>> ExecuteAllReaderInternalAsync<TEntity>(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            options.CancellationToken.ThrowIfCancellationRequested();

            var all_results = new List<TEntity>(capacity: options.ExpectedResultCount);

            using (var connection = connectionFactory.GetNewConnection())
            {
                if (options.ShouldAsyncOpenConnection)
                {
                    await
                        connection
                        .OpenAsync(options.CancellationToken);
                }
                else
                {
                    connection.Open();
                }

                options.CancellationToken.ThrowIfCancellationRequested();

                using (var cmd = PrepareCommand(connection, CommandType.StoredProcedure, procName, parameters))
                {
                    if (options.ShouldAsyncExecuteCommand)
                    {
                        var reader =
                            (TDataReader)
                            await
                            cmd.ExecuteReaderAsync(options.CancellationToken)
                            .ConfigureAwait(continueOnCapturedContext: false);

                        while (await reader.NextResultAsync(options.CancellationToken).ConfigureAwait(continueOnCapturedContext: false))
                        {
                            options.CancellationToken.ThrowIfCancellationRequested();

                            var result = createEntity(reader);

                            all_results.Add(result);
                        }
                    }
                    else
                    {
                        var reader = (TDataReader)cmd.ExecuteReader();

                        while (reader.NextResult())
                        {
                            options.CancellationToken.ThrowIfCancellationRequested();

                            var result = createEntity(reader);

                            all_results.Add(result);
                        }
                    }
                }
            }

            all_results.TrimExcess();

            return all_results;
        }

        #endregion

        #region Execute Reader Async (Process Each Row)

        public async Task ExecuteReaderAsync(
            string procName,
            Action<IDataReader, CancellationToken> processRow)
        {
            await
                ExecuteReaderAsync(procName, new IDbDataParameter[0], DefaultExecuteReaderOptions, processRow)
                .ConfigureAwait(continueOnCapturedContext: false);
        }
        public async Task ExecuteReaderAsync(
            string procName, 
            IExecuteReaderOptions options, 
            Action<IDataReader, CancellationToken> processRow)
        {
            await
                ExecuteReaderAsync(procName, new IDbDataParameter[0], options, processRow)
                .ConfigureAwait(continueOnCapturedContext: false);
        }
        public async Task ExecuteReaderAsync(
            string procName, 
            IReadOnlyList<IDbDataParameter> parameters, 
            Action<IDataReader, CancellationToken> processRow)
        {
            await
                ExecuteReaderAsync(procName, parameters, DefaultExecuteReaderOptions, processRow)
                .ConfigureAwait(continueOnCapturedContext: false);
        }
        public async Task ExecuteReaderAsync(
            string procName, 
            IReadOnlyList<IDbDataParameter> parameters, 
            IExecuteReaderOptions options, 
            Action<IDataReader, CancellationToken> processRow)
        {
            await
            ExecuteReaderWithRetryInternalAsync(
                ConnectionFactory,
                options,
                procName,
                parameters.Cast<TParameter>().ToArray(),
                processRow)
            .ConfigureAwait(continueOnCapturedContext: false);
        }

        #endregion

        #region Execute Reader Async (Internal)

        protected async Task ExecuteReaderWithRetryInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Action<IDataReader, CancellationToken> processRow)
        {
            var retryOptions = new RetryPolicyOptions
            {
                CancellationToken = options.CancellationToken
            };
            
            await
                ExecuteReaderWithRetryInternalAsync(connectionFactory, options, retryOptions, procName, parameters, processRow)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        protected async Task ExecuteReaderWithRetryInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            IRetryPolicyOptions retryOptions,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Action<IDataReader, CancellationToken> processRow)
        {
                await
                RetryPolicy
                .ExecuteAsync(retryOptions, async () =>
                {
                    await
                    ExecuteReaderInternalAsync(
                        ConnectionFactory,
                        options,
                        procName,
                        parameters,
                        processRow)
                        .ConfigureAwait(continueOnCapturedContext: false);

                }).ConfigureAwait(continueOnCapturedContext: false);
        }

        protected virtual async Task ExecuteReaderInternalAsync(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Action<IDataReader, CancellationToken> processRow)
        {
            options.CancellationToken.ThrowIfCancellationRequested();

            using (var connection = connectionFactory.GetNewConnection())
            {
                if (options.ShouldAsyncOpenConnection)
                {
                    await
                        connection
                        .OpenAsync(options.CancellationToken);
                }
                else
                {
                    connection.Open();
                }

                options.CancellationToken.ThrowIfCancellationRequested();

                using (var cmd = PrepareCommand(connection, CommandType.StoredProcedure, procName, parameters))
                {
                    if (options.ShouldAsyncExecuteCommand)
                    {
                        var reader =
                            (TDataReader)
                            await
                            cmd.ExecuteReaderAsync(options.CancellationToken)
                            .ConfigureAwait(continueOnCapturedContext: false);

                        while (await reader.NextResultAsync(options.CancellationToken).ConfigureAwait(continueOnCapturedContext: false))
                        {
                            options.CancellationToken.ThrowIfCancellationRequested();

                            processRow(reader, options.CancellationToken);
                        }
                    }
                    else
                    {
                        var reader = (TDataReader)cmd.ExecuteReader();

                        while (reader.NextResult())
                        {
                            options.CancellationToken.ThrowIfCancellationRequested();

                            processRow(reader, options.CancellationToken);
                        }
                    }
                }
            }

        }

        #endregion

        #region Execute Reader (To Entity)

        public IEnumerable<TEntity> ExecuteReader<TEntity>(string procName, Func<IDataReader, TEntity> createEntity)
        {
            return
                ExecuteReader<TEntity>(procName, new IDbDataParameter[0], DefaultExecuteReaderOptions, createEntity);
        }

        public IEnumerable<TEntity> ExecuteReader<TEntity>(
            string procName, 
            IExecuteReaderOptions options, 
            Func<IDataReader, TEntity> createEntity)
        {
            return
                ExecuteReader<TEntity>(procName, new IDbDataParameter[0], options, createEntity);
        }

        public IEnumerable<TEntity> ExecuteReader<TEntity>(
            string procName, 
            IReadOnlyList<IDbDataParameter> parameters, 
            Func<IDataReader, TEntity> createEntity)
        {
            return
                ExecuteReader<TEntity>(procName, parameters, DefaultExecuteReaderOptions, createEntity);
        }

        public IEnumerable<TEntity> ExecuteReader<TEntity>(
            string procName, 
            IReadOnlyList<IDbDataParameter> parameters, 
            IExecuteReaderOptions options, 
            Func<IDataReader, TEntity> createEntity)
        {
            return
                ExecuteReaderWithRetryInternal(ConnectionFactory, options, procName, parameters.Cast<TParameter>().ToArray(), createEntity);
        }

        #endregion

        #region Execute Reader (Internal)

        protected IEnumerable<TEntity> ExecuteReaderWithRetryInternal<TEntity>(
          ConnectionFactory<TConnection> connectionFactory,
          IExecuteReaderOptions options,
          string procName,
          IReadOnlyList<TParameter> parameters,
          Func<TDataReader, TEntity> createEntity)
        {
            var retryOptions = new RetryPolicyOptions
            {
                CancellationToken = options.CancellationToken
            };

            return
                ExecuteReaderWithRetryInternal(connectionFactory, options, retryOptions, procName, parameters, createEntity);
        }

        protected IEnumerable<TEntity> ExecuteReaderWithRetryInternal<TEntity>(
            ConnectionFactory<TConnection> connectionFactory,
            IExecuteReaderOptions options,
            IRetryPolicyOptions retryOptions,
            string procName,
            IReadOnlyList<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            return
                RetryPolicy
                .Execute<IEnumerable<TEntity>>(retryOptions, () =>
                {
                    return
                    ExecuteReaderInternal(
                        ConnectionFactory,
                        options,
                        procName,
                        new TParameter[0],
                        createEntity);
                });
        }


        protected virtual IEnumerable<TEntity> ExecuteReaderInternal<TEntity>(
        ConnectionFactory<TConnection> connectionFactory,
        IExecuteReaderOptions options,
        string procName,
        IReadOnlyList<TParameter> parameters,
        Func<TDataReader, TEntity> createEntity)
        {
            return
                new CommandResultIterator<TEntity>(
                    () =>
                    new CommandResultEnumerator<TConnection, TCommand, TDataReader, TParameter, TEntity>(
                        this,
                        connectionFactory,
                        procName,
                        parameters,
                        createEntity));
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

        #region Execute Reader - to Dictionary

        public IEnumerable<Dictionary<string, object>> ExecuteReader(
            string procName)
        {
            return ExecuteReader(procName, new TParameter[0]);
        }
        
        public IEnumerable<Dictionary<string, object>> ExecuteReader(
            string procName,
            IEnumerable<TParameter> parameters)
        {
            return ExecuteReaderInternalWithRetry(ConnectionFactory, procName, parameters);
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


        //public async Task<IReadOnlyList<Dictionary<string, object>>> ExecuteAllReaderAsync(string procName)
        //{
        //    return 
        //        await
        //        ExecuteAllReaderAsync(procName, new TParameter[0])
        //        .ConfigureAwait(continueOnCapturedContext: false);
        //}

        //public async Task<IReadOnlyList<Dictionary<string, object>>> ExecuteAllReaderAsync(
        //   string procName,
        //   IEnumerable<TParameter> parameters)
        //{
        //    return 
        //        await
        //        ExecuteAllReaderInternalWithRetryAsync(ConnectionFactory, procName, parameters)
        //        .ConfigureAwait(continueOnCapturedContext: false);
        //}

        //async Task<IReadOnlyList<Dictionary<string, object>>> ExecuteAllReaderInternalWithRetryAsync(
        //    ConnectionFactory<TConnection> connectionFactory,
        //    string procName,
        //    IEnumerable<TParameter> parameters)
        //{
        //    return 
        //        await
        //        RetryPolicy
        //        .ExecuteAsync(async () =>
        //        {
        //            return 
        //            await
        //            ExecuteAllReaderInternalAsync(
        //                connectionFactory,
        //                procName,
        //                parameters)
        //                .ConfigureAwait(continueOnCapturedContext: false);
        //        })
        //        .ConfigureAwait(continueOnCapturedContext: false);
        //}

        //async Task<IReadOnlyList<Dictionary<string, object>>> ExecuteAllReaderInternalAsync(
        //    ConnectionFactory<TConnection> connectionFactory,
        //    string procName,
        //    IEnumerable<TParameter> parameters)
        //{
        //    var all_results = new List<Dictionary<string, object>>(capacity: 1000);

        //    using (var connection = connectionFactory.GetNewConnection())
        //    {

        //        //await connection.OpenAsync().ConfigureAwait(continueOnCapturedContext: false);
        //        connection.Open();
        //        using (var cmd = PrepareCommand(connection, CommandType.StoredProcedure, procName, parameters))
        //        {

        //            //var r = await cmd.ExecuteReaderAsync().ConfigureAwait(continueOnCapturedContext: false);
        //            var r = cmd.ExecuteReader();

        //            //            while(await r.NextResultAsync().ConfigureAwait(continueOnCapturedContext: false))
        //            while (r.NextResult())
        //            {
        //                var result = new Dictionary<string, object>(capacity: r.FieldCount);

        //                for (int i = 0; i < r.FieldCount; i++)
        //                {
        //                    result.Add(r.GetName(i), r[i]);
        //                }

        //                all_results.Add(result);
        //            }
        //        }
        //    }

        //    all_results.TrimExcess();
        //    return all_results;
        //}










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

        #endregion

        public Dictionary<string, object> ExecuteReaderSingleRow(string procName)
        {
            return ExecuteReaderSingleRowInternalWithRetry(ConnectionFactory, procName, new TParameter[0]);
        }

        public Dictionary<string, object> ExecuteReaderSingleRow(string procName, IEnumerable<TParameter> parameters)
        {
            return ExecuteReaderSingleRowInternalWithRetry(ConnectionFactory, procName, parameters);
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

        public async void Execute(string procName)
        {
            await ExecuteNonQueryInternal(ConnectionFactory, CommandType.StoredProcedure, procName);
        }

        public async void ExecuteNonQuery(string procName, params TParameter[] parameters)
        {
            await ExecuteNonQueryInternal(ConnectionFactory, CommandType.StoredProcedure, procName, parameters);
        }

        public async void ExecuteNonQueryText(string sql)
        {
            await ExecuteNonQueryInternal(ConnectionFactory, CommandType.Text, sql, new TParameter[0]);
        }
        
        async Task ExecuteNonQueryInternal(ConnectionFactory<TConnection> connectionFactory, CommandType commandType, string procName, params TParameter[] parameters)
        {
            try
            {
                using (var connection = connectionFactory.GetNewConnection())
                {
                    connection.Open();

                    using (var command = PrepareCommand(connection, commandType, procName, parameters.EmptyIfNull()))
                    {
                        await command.ExecuteNonQueryAsync();

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
        
        public T ExecuteScalar<T>(string procName, IEnumerable<TParameter> parameters)
        {
            var result = ExecuteScalarInternalWithRetry(ConnectionFactory, CommandType.StoredProcedure, procName, parameters);

            if (result == null && !typeof(T).IsNullable())
                throw new InvalidCastException(
                    $"{procName} stored procedure returned NULL but expected type is non-nullable {typeof(T).Name}");

            return MapToClrValue<T>(result);
        }

        object ExecuteScalarInternalWithRetry(
            ConnectionFactory<TConnection> connectionFactory, 
            CommandType commandType,
            string commandText, 
            IEnumerable<TParameter> parameters)
        {
            return RetryPolicy
                .Execute(async () =>
                {
                    return await ExecuteScalarInternal(
                        connectionFactory,
                        commandType,
                        commandText,
                        parameters);
                });
        }

        protected async Task<object> ExecuteScalarInternal(
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
                    var result = await command.ExecuteScalarAsync();

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
