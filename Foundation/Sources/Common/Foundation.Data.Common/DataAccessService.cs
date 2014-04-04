using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Data
{
    public partial class DataAccessService<TConnection, TCommand, TParameter, TDataReader>
        where TConnection : DbConnection, new()
        where TCommand : DbCommand
        where TParameter : DbParameter
        where TDataReader : DbDataReader
    {
        ConnectionFactory<TConnection> ConnectionFactory { get; set; }

        public TimeSpan DefaultCommandTimeout { get; set; }

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
            return ExecuteReaderInternal<TEntity>(ConnectionFactory, procName, new List<TParameter>(), createEntity);
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
            return ExecuteReaderInternal<TEntity>(ConnectionFactory, procName, parameters, createEntity);
        }

        public Task<List<TEntity>> ExecuteReaderAsync<TEntity>(
            string procName,
            IEnumerable<TParameter> parameters,
            Func<TDataReader, TEntity> createEntity)
        {
            return Task.Factory.StartNew(() => ExecuteReader<TEntity>(procName, parameters, createEntity).ToList());
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
                    using (var reader = (TDataReader) command.ExecuteReader())
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
            return ExecuteReaderInternal(ConnectionFactory, procName, parameters);
        }

        public Task<List<Dictionary<string, object>>> ExecuteReaderAsync(
            string procName,
            IEnumerable<TParameter> parameters)
        {
            return Task.Factory.StartNew(() => ExecuteReader(procName, parameters).ToList());
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
                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
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
            return ExecuteReaderSingleRow(ConnectionFactory, procName, new List<TParameter>());
        }

        public Task<Dictionary<string, object>> ExecuteReaderSingleRowAsync(string procName)
        {
            return Task.Factory.StartNew(() => ExecuteReaderSingleRow(procName));
        }

        public Dictionary<string, object> ExecuteReaderSingleRow(string procName, IEnumerable<TParameter> parameters)
        {
            return ExecuteReaderSingleRow(ConnectionFactory, procName, parameters);
        }

        public Task<Dictionary<string, object>> ExecuteReaderSingleRowAsync(string procName, IEnumerable<TParameter> parameters)
        {
            return Task.Factory.StartNew(() => ExecuteReaderSingleRow(procName, parameters));
        }

        Dictionary<string, object> ExecuteReaderSingleRow(
            ConnectionFactory<TConnection> connectionFactory,
            string procName,
            IEnumerable<TParameter> parameters)
        {
            var result = (Dictionary<string, object>) null;

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
            Execute(ConnectionFactory, procName, new List<TParameter>());
        }

        public Task ExecuteAsync(string procName)
        {
            return Task.Factory.StartNew(() => Execute(procName));
        }

        public void Execute(string procName, IEnumerable<TParameter> parameters)
        {
            Execute(ConnectionFactory, procName, parameters);
        }

        public Task ExecuteAsync(string procName, IEnumerable<TParameter> parameters)
        {
            return Task.Factory.StartNew(() => Execute(procName, parameters));
        }

        void Execute(ConnectionFactory<TConnection> connectionFactory, string procName, IEnumerable<TParameter> parameters)
        {
            using (var connection = connectionFactory.GetNewConnection())
            {
                connection.Open();

                using (var command = PrepareCommand(connection, CommandType.StoredProcedure, procName, parameters))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public T ExecuteScalar<T>(string procName)
        {
            return (T) ExecuteScalar(ConnectionFactory, procName, new List<TParameter>());
        }
        
        public Task<T> ExecuteScalarAsync<T>(string procName)
        {
            return Task.Factory.StartNew(() => ExecuteScalar<T>(procName));
        }

        public T ExecuteScalar<T>(string procName, IEnumerable<TParameter> parameters)
        {
            return (T) ExecuteScalar(ConnectionFactory, procName, parameters);
        }

        public Task<T> ExecuteScalarAsync<T>(string procName, IEnumerable<TParameter> parameters)
        {
            return Task.Factory.StartNew(() => ExecuteScalar<T>(procName, parameters));
        }

        object ExecuteScalar(ConnectionFactory<TConnection> connectionFactory, string procname, IEnumerable<TParameter> parameters)
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
    }
}
