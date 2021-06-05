namespace OpenMedStack.NEventStore.Persistence.Sql.SqlDialects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Persistence;
    using Sql;

    public class PagedEnumerationCollection : IAsyncEnumerable<IDataRecord>, IAsyncEnumerator<IDataRecord>
    {
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(PagedEnumerationCollection));
        private readonly IDbCommand _command;
        private readonly ISqlDialect _dialect;
        private readonly IEnumerable<IDisposable> _disposable = Array.Empty<IDisposable>();
        private readonly NextPageDelegate _nextpage;
        private readonly int _pageSize;
        private readonly CancellationToken _cancellationToken;

        private IDataRecord? _current;
        private bool _disposed;
        private int _position;
        private IDataReader? _reader;

        public PagedEnumerationCollection(
            ISqlDialect dialect,
            IDbCommand command,
            NextPageDelegate nextpage,
            int pageSize,
            CancellationToken cancellationToken,
            params IDisposable[] disposable)
        {
            _dialect = dialect;
            _command = command;
            _nextpage = nextpage;
            _pageSize = pageSize;
            _cancellationToken = cancellationToken;
            _disposable = disposable ?? _disposable;
        }

        public virtual IAsyncEnumerator<IDataRecord> GetAsyncEnumerator(CancellationToken cancellationToken)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(PersistenceMessages.ObjectAlreadyDisposed);
            }

            return this;
        }

        public async ValueTask DisposeAsync()
        {
            await Dispose(true).ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        async ValueTask<bool> IAsyncEnumerator<IDataRecord>.MoveNextAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(PersistenceMessages.ObjectAlreadyDisposed);
            }

            if (await MoveToNextRecord().ConfigureAwait(false))
            {
                return true;
            }

            Logger.Verbose(PersistenceMessages.QueryCompleted);
            return false;
        }

        public virtual void Reset()
        {
            throw new NotSupportedException("Forward-only readers.");
        }

        public virtual IDataRecord Current
        {
            get
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(PersistenceMessages.ObjectAlreadyDisposed);
                }

                return (_current = _reader)!;
            }
        }

        protected virtual async ValueTask Dispose(bool disposing)
        {
            if (!disposing || _disposed)
            {
                return;
            }

            _disposed = true;
            _position = 0;
            _current = null;

            if (_reader is IAsyncDisposable dbdr)
            {
                await dbdr.DisposeAsync().ConfigureAwait(false);
            }
            else
            {
                _reader?.Dispose();
            }

            _reader = null;

            if (_command is IAsyncDisposable ad)
            {
                await ad.DisposeAsync().ConfigureAwait(false);
            }
            else
            {
                _command?.Dispose();
            }

            foreach (var dispose in _disposable)
            {
                dispose?.Dispose();
            }
        }

        private async Task<bool> MoveToNextRecord()
        {
            if (_pageSize > 0 && _position >= _pageSize)
            {
                _command.SetParameter(_dialect.Skip, _position);
                _nextpage(_command, _current);
            }

            _reader ??= await OpenNextPage().ConfigureAwait(false);

            if (_reader is DbDataReader dbdr)
            {
                if (await dbdr.ReadAsync(_cancellationToken).ConfigureAwait(false))
                {
                    return IncrementPosition();
                }
            }
            else if (_reader.Read())
            {
                return IncrementPosition();
            }

            if (!PagingEnabled())
            {
                return false;
            }

            if (!PageCompletelyEnumerated())
            {
                return false;
            }

            Logger.Verbose(PersistenceMessages.EnumeratedRowCount, _position);
            _reader.Dispose();
            _reader = await OpenNextPage().ConfigureAwait(false);

            if (_reader is DbDataReader ar)
            {
                if (await ar.ReadAsync(_cancellationToken).ConfigureAwait(false))
                {
                    return IncrementPosition();
                }
            }
            else if (_reader.Read())
            {
                return IncrementPosition();
            }

            return false;
        }

        private bool IncrementPosition()
        {
            _position++;
            return true;
        }

        private bool PagingEnabled() => _pageSize > 0;

        private bool PageCompletelyEnumerated() => _position > 0 && 0 == _position % _pageSize;

        private async Task<IDataReader> OpenNextPage()
        {
            try
            {
                if (_command is DbCommand dbcmd)
                {
                    return await dbcmd.ExecuteReaderAsync(_cancellationToken).ConfigureAwait(false);
                }
                return _command.ExecuteReader();
            }
            catch (Exception e)
            {
                Logger.Debug(PersistenceMessages.EnumerationThrewException, e.GetType());
                throw new StorageUnavailableException(e.Message, e);
            }
        }
    }
}