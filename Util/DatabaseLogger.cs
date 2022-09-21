using SendBlazorLoggerToDataBase.Entities;

namespace SendBlazorLoggerToDataBase.Util
{
    public class DatabaseLogger : ILogger

    {
        private bool _isDisposed;
        private readonly ApplicationDbContext _dbContext;
        public DatabaseLogger(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            DBLog log = new DBLog();

            log.EventName = eventId.Name;
            log.ExceptionMessage = exception?.Message;
            log.StackTrace = exception?.StackTrace;
            //log.Detail = exception?.Message;
            //string.Format("EventId:{eventId}, ExceptionMsg:{exception}",eventId.Name.ToString(),exception?.Message);
            //log.Place = exception?.StackTrace;
            _dbContext.DbLogs.Add(log);
            _dbContext.SaveChanges();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
