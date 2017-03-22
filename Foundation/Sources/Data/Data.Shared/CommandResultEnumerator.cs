using SquaredInfinity.Collections;
using SquaredInfinity.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using SquaredInfinity.Extensions;
using System.Data;

namespace SquaredInfinity.Data
{
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

                this.DataReader = (TDataReader)Command.ExecuteReader();
            }
            catch (Exception ex)
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
            catch (Exception ex)
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
            if (DataReader != null)
            {
                DataReader.Dispose();
                DataReader = null;
            }

            if (Connection != null)
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
