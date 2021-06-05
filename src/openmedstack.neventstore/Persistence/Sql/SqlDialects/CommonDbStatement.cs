namespace OpenMedStack.NEventStore.Persistence.Sql.SqlDialects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Sql;

    public class CommonDbStatement : IDbStatement
    {
        private const int InfinitePageSize = 0;
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(CommonDbStatement));
        private readonly IDbConnection _connection;

        public CommonDbStatement(
            ISqlDialect dialect,
            IDbConnection connection)
        {
            Parameters = new Dictionary<string, Tuple<object, DbType?>>();

            Dialect = dialect;
            _connection = connection;
        }

        protected IDictionary<string, Tuple<object, DbType?>> Parameters { get; }

        protected ISqlDialect Dialect { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual int PageSize { get; set; }

        public virtual void AddParameter(string name, object value, DbType? parameterType = null)
        {
            Logger.Debug(PersistenceMessages.AddingParameter, name);
            Parameters[name] = Tuple.Create(Dialect.CoalesceParameterValue(value), parameterType);
        }

        public virtual Task<int> ExecuteWithoutExceptions(string commandText)
        {
            try
            {
                return ExecuteNonQuery(commandText);
            }
            catch (Exception)
            {
                Logger.Debug(PersistenceMessages.ExceptionSuppressed);
                return Task.FromResult(0);
            }
        }

        public virtual Task<int> ExecuteNonQuery(string commandText)
        {
            try
            {
                var totalRowsAffected = 0;
                foreach (var text in commandText.Split(new[] { "__" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    using var command = BuildCommand(text);
                    totalRowsAffected += command.ExecuteNonQuery();
                }
                return Task.FromResult(totalRowsAffected);
            }
            catch (Exception e)
            {
                if (Dialect.IsDuplicate(e))
                {
                    throw new UniqueKeyViolationException(e.Message, e);
                }

                throw;
            }
        }

        public virtual Task<object?> ExecuteScalar(string commandText)
        {
            try
            {
                using var command = BuildCommand(commandText);
                var affected = command.ExecuteScalar();
                return Task.FromResult(affected);
            }
            catch (Exception e)
            {
                if (Dialect.IsDuplicate(e))
                {
                    throw new UniqueKeyViolationException(e.Message, e);
                }
                throw;
            }
        }

        public virtual IAsyncEnumerable<IDataRecord> ExecuteWithQuery(
            string queryText,
            CancellationToken cancellationToken)
        {
            return ExecuteQuery(queryText, (query, latest) => { }, InfinitePageSize, cancellationToken);
        }

        public virtual IAsyncEnumerable<IDataRecord> ExecutePagedQuery(string queryText, NextPageDelegate nextpage, CancellationToken cancellation = default)
        {
            var pageSize = Dialect.CanPage ? PageSize : InfinitePageSize;
            if (pageSize > 0)
            {
                Logger.Verbose(PersistenceMessages.MaxPageSize, pageSize);
                Parameters.Add(Dialect.Limit, Tuple.Create((object)pageSize, (DbType?)null));
            }

            var result = ExecuteQuery(queryText, nextpage, pageSize, cancellation);
            return result;
        }

        protected virtual void Dispose(bool disposing)
        {
            Logger.Verbose(PersistenceMessages.DisposingStatement);
            _connection?.Close();
            _connection?.Dispose();
        }

        protected virtual IAsyncEnumerable<IDataRecord> ExecuteQuery(string queryText, NextPageDelegate nextpage, int pageSize, CancellationToken cancellationToken)
        {
            Parameters.Add(Dialect.Skip, Tuple.Create((object)0, (DbType?)null));
            var command = BuildCommand(queryText);

            try
            {
                return new PagedEnumerationCollection(Dialect, command, nextpage, pageSize, cancellationToken, this);
            }
            catch (Exception)
            {
                command.Dispose();
                throw;
            }
        }

        protected virtual IDbCommand BuildCommand(string statement)
        {
            Logger.Verbose(PersistenceMessages.CreatingCommand);
            var command = _connection.CreateCommand();

            if (Settings.CommandTimeout > 0)
            {
                command.CommandTimeout = Settings.CommandTimeout;
            }

            command.CommandText = statement;

            Logger.Verbose(PersistenceMessages.CommandTextToExecute, statement);

            BuildParameters(command);

            return command;
        }

        protected virtual void BuildParameters(IDbCommand command)
        {
            foreach (var (key, (item, dbType)) in Parameters)
            {
                BuildParameter(command, key, item, dbType);
            }
        }

        protected virtual void BuildParameter(IDbCommand command, string name, object value, DbType? dbType)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            SetParameterValue(parameter, value, dbType);

            Logger.Verbose(PersistenceMessages.BindingParameter, name, parameter.Value);
            command.Parameters.Add(parameter);
        }

        protected virtual void SetParameterValue(IDataParameter param, object value, DbType? type)
        {
            param.DbType = type ?? (value == null ? DbType.Binary : param.DbType);
            param.Value = value ?? DBNull.Value;
        }
    }
}