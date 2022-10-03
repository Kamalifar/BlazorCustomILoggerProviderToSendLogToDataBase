using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using SendBlazorLoggerToDataBase.Entities;

namespace SendBlazorLoggerToDataBase.Util
{
    public class DbLoggerProvider:ILoggerProvider
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private readonly IList<DBLog> _currentBatch = new List<DBLog>();
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(2);

        private readonly BlockingCollection<DBLog> _messageQueue = new(new ConcurrentQueue<DBLog>());

        private readonly Task _outputTask;
        private readonly ApplicationDbContext _dbContext;
        private bool _isDisposed;

        public DbLoggerProvider(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _outputTask = Task.Run(ProcessLogQueue);
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new DBLogger(this,categoryName);
        }
        private async Task ProcessLogQueue()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                while (_messageQueue.TryTake(out var message))
                {
                    try
                    {
                        _currentBatch.Add(message);
                    }
                    catch
                    {
                        //cancellation token canceled or CompleteAdding called
                    }
                }

                await SaveLogItemsAsync(_currentBatch, _cancellationTokenSource.Token);
                _currentBatch.Clear();

                await Task.Delay(_interval, _cancellationTokenSource.Token);
            }
        }
        internal void AddLogItem(DBLog appLogItem)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                _messageQueue.Add(appLogItem, _cancellationTokenSource.Token);
            }
        }
        private async Task SaveLogItemsAsync(IList<DBLog> items, CancellationToken cancellationToken)
        {
            try
            {
                if (!items.Any())
                {
                    return;
                }

                // We need a separate context for the logger to call its SaveChanges several times,
                // without using the current request's context and changing its internal state.
                foreach (var item in items)
                {
                    var addedEntry = _dbContext.DbLogs.Add(item);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);

            }
            catch
            {
                // don't throw exceptions from logger
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1031:catch a more specific allowed exception type, or rethrow the exception",
            Justification = "don't throw exceptions from logger")]
        private void Stop()
        {
            _cancellationTokenSource.Cancel();
            _messageQueue.CompleteAdding();

            try
            {
                _outputTask.Wait(_interval);
            }
            catch
            {
                // don't throw exceptions from logger
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                try
                {
                    if (disposing)
                    {
                        Stop();
                        _messageQueue.Dispose();
                        _cancellationTokenSource.Dispose();
                        _dbContext.Dispose();
                    }
                }
                finally
                {
                    _isDisposed = true;
                }
            }
        }
    }
}
