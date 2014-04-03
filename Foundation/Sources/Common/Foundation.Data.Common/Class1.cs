//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SquaredInfinity.Foundation.Data
//{
//    public class ConnectionFactory
//    {
//        public ConnectionFactory(string connectionString)
//        {
//            _connectionString = connectionString;
//        }

//        public SqlConnection GetNewConnection()
//        {
//            return new SqlConnection(_connectionString);
//        }

//        private string _connectionString;
//    }

//    public partial class DataAccessService
//    {
//        private ConnectionFactory ConnectionFactory { get; set; }
//        static readonly int CommandTimeoutInMilliseconds = 10000;

//        protected DataAccessService(ConnectionFactory connectionFactory)
//        {
//            this.ConnectionFactory = connectionFactory;
//        }


//        protected IEnumerable<TEntity> ExecuteReader<TEntity>(
//            string procName,
//            IEnumerable<SqlParameter> parameters,
//            Func<SqlDataReader, TEntity> createEntity)
//        {
//            return ExecuteReader<TEntity>(ConnectionFactory, procName, parameters, createEntity);
//        }

//        IEnumerable<TEntity> ExecuteReader<TEntity>(
//            ConnectionFactory connectionFactory,
//            string procName,
//            IEnumerable<SqlParameter> parameters,
//            Func<SqlDataReader, TEntity> createEntity
//            )
//        {
//            using (SqlConnection sqlConnection = connectionFactory.GetNewConnection())
//            {
//                sqlConnection.Open();

//                var results = new List<TEntity>();

//                using (SqlCommand command = sqlConnection.CreateCommand())
//                {
//                    command.CommandType = CommandType.StoredProcedure;
//                    command.CommandText = procName;
//                    command.CommandTimeout = CommandTimeoutInMilliseconds;

//                    foreach (SqlParameter sqlParameter in parameters)
//                        command.Parameters.Add(sqlParameter);

//                    using (var reader = command.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            results.Add(createEntity(reader));
//                        }
//                    }
//                }

//                return results;
//            }
//        }

//        TEntity ExecuteReaderForSingleEntity<TEntity>(
//            ConnectionFactory connectionFactory,
//            string procName,
//            IEnumerable<SqlParameter> parameters,
//            Func<SqlDataReader, TEntity> createEntity
//            )
//        {
//            using (SqlConnection sqlConnection = connectionFactory.GetNewConnection())
//            {
//                sqlConnection.Open();

//                var result = default(TEntity);

//                using (SqlCommand command = sqlConnection.CreateCommand())
//                {
//                    command.CommandType = CommandType.StoredProcedure;
//                    command.CommandText = procName;
//                    command.CommandTimeout = CommandTimeoutInMilliseconds;

//                    foreach (SqlParameter sqlParameter in parameters)
//                        command.Parameters.Add(sqlParameter);

//                    using (var reader = command.ExecuteReader())
//                    {
//                        result = createEntity(reader);
//                    }
//                }

//                return result;
//            }
//        }


//        protected Dictionary<string, object> ExecuteReaderSingleRow(
//            string procName,
//            IEnumerable<SqlParameter> parameters)
//        {
//            return ExecuteReaderSingleRow(ConnectionFactory, procName, parameters);
//        }

//        Dictionary<string, object> ExecuteReaderSingleRow(
//            ConnectionFactory connectionFactory,
//            string procName,
//            IEnumerable<SqlParameter> parameters)
//        {
//            var result = new Dictionary<string, object>();

//            using (SqlConnection sqlConnection = connectionFactory.GetNewConnection())
//            {
//                sqlConnection.Open();

//                using (SqlCommand command = sqlConnection.CreateCommand())
//                {
//                    command.CommandType = CommandType.StoredProcedure;
//                    command.CommandText = procName;
//                    command.CommandTimeout = CommandTimeoutInMilliseconds;

//                    foreach (SqlParameter sqlParameter in parameters)
//                        command.Parameters.Add(sqlParameter);

//                    using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
//                    {
//                        if (reader.Read())
//                        {
//                            for (int i = 0; i < reader.FieldCount; i++)
//                            {
//                                result.Add(reader.GetName(i), reader[i]);
//                            }
//                        }
//                    }
//                }
//            }

//            return result;
//        }


//        protected void Execute(string procName)
//        {
//            Execute(ConnectionFactory, procName);
//        }

//        void Execute(ConnectionFactory connectionFactory, string procName)
//        {
//            Execute(connectionFactory, procName, new List<SqlParameter>());
//        }

//        protected void Execute(string procName, IEnumerable<SqlParameter> parameters)
//        {
//            Execute(ConnectionFactory, procName, parameters);
//        }

//        void Execute(ConnectionFactory connectionFactory, string procName, IEnumerable<SqlParameter> parameters)
//        {
//            using (SqlConnection sqlConnection = connectionFactory.GetNewConnection())
//            {
//                sqlConnection.Open();
//                using (SqlCommand command = sqlConnection.CreateCommand())
//                {
//                    command.CommandType = CommandType.StoredProcedure;
//                    command.CommandText = procName;
//                    command.CommandTimeout = CommandTimeoutInMilliseconds;

//                    foreach (SqlParameter sqlParameter in parameters)
//                        command.Parameters.Add(sqlParameter);

//                    command.ExecuteNonQuery();
//                }
//            }
//        }


//        protected object ExecuteScalar(string procName)
//        {
//            return ExecuteScalar(ConnectionFactory, procName);
//        }

//        object ExecuteScalar(ConnectionFactory connectionFactory, string procName)
//        {
//            return ExecuteScalar(connectionFactory, procName, new List<SqlParameter>());
//        }

//        protected object ExecuteScalar(string procName, IEnumerable<SqlParameter> parameters)
//        {
//            return ExecuteScalar(ConnectionFactory, procName, parameters);
//        }

//        object ExecuteScalar(ConnectionFactory connectionFactory, string procname, IEnumerable<SqlParameter> parameters)
//        {
//            using (SqlConnection sqlConnection = connectionFactory.GetNewConnection())
//            {
//                sqlConnection.Open();
//                using (SqlCommand command = sqlConnection.CreateCommand())
//                {
//                    command.CommandType = CommandType.StoredProcedure;
//                    command.CommandText = procname;
//                    command.CommandTimeout = CommandTimeoutInMilliseconds;
//                    foreach (SqlParameter sqlParameter in parameters)
//                        command.Parameters.Add(sqlParameter);

//                    return command.ExecuteScalar();
//                }
//            }
//        }
//    }

//    public partial class DataAccessService
//    {
//        //NOTE: Ideally static methods of this service would not be used because it may encourage bad practice of having data access code in random places
//        //      Instead, a specific derived service should be created which would then contain all data access logic
//        //      Unfortunately at this moment I don't have enough time to refactor all code, so some parts of ARCo will still call static methods

//        static DataAccessService StaticDataAccessService;

//        static DataAccessService()
//        {
//            StaticDataAccessService = new DataAccessService(null);
//        }

//        public static Task<TEntity> ExecuteReaderForSingleEntityAsync<TEntity>(
//            string connectionString,
//            string procName,
//            IEnumerable<SqlParameter> parameters,
//            Func<SqlDataReader, TEntity> createEntity)
//        {
//            var connectionFactory = new ConnectionFactory(connectionString);

//            return Task.Factory.StartNew(() => StaticDataAccessService.ExecuteReaderForSingleEntity<TEntity>(connectionFactory, procName, parameters, createEntity));
//        }

//        public static TEntity ExecuteReaderForSingleEntity<TEntity>(
//            string connectionString,
//            string procName,
//            IEnumerable<SqlParameter> parameters,
//            Func<SqlDataReader, TEntity> createEntity)
//        {
//            var connectionFactory = new ConnectionFactory(connectionString);

//            return StaticDataAccessService.ExecuteReaderForSingleEntity<TEntity>(connectionFactory, procName, parameters, createEntity);
//        }

//        public static IEnumerable<TEntity> ExecuteReader<TEntity>(
//            string connectionString,
//            string procName,
//            IEnumerable<SqlParameter> parameters,
//            Func<SqlDataReader, TEntity> createEntity)
//        {
//            var connectionFactory = new ConnectionFactory(connectionString);

//            return StaticDataAccessService.ExecuteReader<TEntity>(connectionFactory, procName, parameters, createEntity);
//        }

//        public static Dictionary<string, object> ExecuteReaderSingleRow(
//            string connectionString,
//            string procName,
//            IEnumerable<SqlParameter> parameters)
//        {
//            var connectionFactory = new ConnectionFactory(connectionString);

//            return StaticDataAccessService.ExecuteReaderSingleRow(connectionFactory, procName, parameters);
//        }

//        public static void Execute(string connectionString, string procName)
//        {
//            var connectionFactory = new ConnectionFactory(connectionString);

//            StaticDataAccessService.Execute(connectionFactory, procName);
//        }

//        public static void Execute(
//            string connectionString,
//            string procName,
//            IEnumerable<SqlParameter> parameters)
//        {
//            var connectionFactory = new ConnectionFactory(connectionString);

//            StaticDataAccessService.Execute(connectionFactory, procName, parameters);
//        }

//        public static object ExecuteScalar(
//            string connectionString,
//            string procname)
//        {
//            var connectionFactory = new ConnectionFactory(connectionString);

//            return StaticDataAccessService.ExecuteScalar(connectionFactory, procname);
//        }

//        public static object ExecuteScalar(
//            string connectionString,
//            string procname,
//            IEnumerable<SqlParameter> parameters)
//        {
//            var connectionFactory = new ConnectionFactory(connectionString);

//            return StaticDataAccessService.ExecuteScalar(connectionFactory, procname, parameters);
//        }
//    }

//    public static class IEnumerableExtensions
//    {
//        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> list)
//        {
//            if (list == null)
//                return new EmptyEnumerator<T>();

//            return list;
//        }

//        public class EmptyEnumerator<T> : IEnumerator<T>, IEnumerable<T>, IEnumerator, IEnumerable
//        {
//            public object Current
//            {
//                get { return null; }
//            }

//            public bool MoveNext()
//            {
//                return false;
//            }

//            public void Reset()
//            {
//            }

//            public IEnumerator GetEnumerator()
//            {
//                return this;
//            }

//            T IEnumerator<T>.Current
//            {
//                get { return default(T); }
//            }

//            public void Dispose()
//            {
//            }

//            IEnumerator<T> IEnumerable<T>.GetEnumerator()
//            {
//                return this;
//            }
//        }
//    }

//    public static class ArrayExtensions
//    {
//        public static bool ArrayContentsEqual<T>(this Array a1, Array a2, IEqualityComparer<T> comparer = null)
//        {
//            if (ReferenceEquals(a1, a2))
//                return true;

//            if (a1 == null || a2 == null)
//                return false;

//            if (a1.Length != a2.Length)
//                return false;

//            if (a1.Rank != a2.Rank)
//                return false;

//            if (comparer == null)
//                comparer = EqualityComparer<T>.Default;

//            if (a1.Rank == 1)
//            {
//                for (int i = 0; i < a1.Length; i++)
//                {
//                    var e1 = (T)a1.GetValue(i);
//                    var e2 = (T)a2.GetValue(i);

//                    if (!comparer.Equals(e1, e2))
//                        return false;
//                }
//            }
//            else if (a1.Rank == 2)
//            {
//                var r0LB = a1.GetLowerBound(0);
//                var r0UB = a1.GetUpperBound(0);

//                var r1LB = a1.GetLowerBound(1);
//                var r1UB = a1.GetUpperBound(1);

//                for (int i = r0LB; i <= r0UB; i++)
//                {
//                    for (int j = r1LB; j <= r1UB; j++)
//                    {
//                        var e1 = (T)a1.GetValue(new int[] { i, j });
//                        var e2 = (T)a2.GetValue(new int[] { i, j });

//                        if (!comparer.Equals(e1, e2))
//                            return false;
//                    }
//                }
//            }

//            return true;
//        }
//    }
//}

//public static class ObjectExtensions
//{
//    public static bool IsIn<T>(this object obj, params T[] args)
//    {
//        foreach (var a in args)
//        {
//            if (object.Equals(obj, a))
//                return true;
//        }

//        return false;
//    }

//    public static object ToDbValue(this object obj)
//    {
//        if (obj == null) return DBNull.Value;

//        var ts = obj as TimeSpan?;
//        if (ts != null)
//        {
//            return ts.Value.Ticks;
//        }

//        if (obj.GetType() == typeof(XDocument))
//            return (obj as XDocument).ToString(SaveOptions.None);

//        return obj;
//    }

//    public static TValue ToClrValue<TValue>(this object obj, TValue defaultValue = default(TValue))
//    {
//        if (obj == null)
//            return defaultValue;

//        if (obj == DBNull.Value)
//            return defaultValue;

//        if (typeof(TValue) == typeof(TimeSpan?) || typeof(TValue) == typeof(TimeSpan))
//            return (TValue)(object)TimeSpan.FromTicks((long)obj);

//        if (typeof(TValue) == typeof(XDocument))
//            return (TValue)(object)XDocument.Parse(obj as string);

//        return (TValue)obj;
//    }
//}
