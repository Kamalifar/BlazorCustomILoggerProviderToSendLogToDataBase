namespace SendBlazorLoggerToDataBase.Util
{
    public class ApplicationLoggerProvider : ILoggerProvider
    {
        private bool _isDisposed;
        private readonly ApplicationDbContext _dbContext;
        public ApplicationLoggerProvider(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DatabaseLogger(_dbContext);
        }

        public void Dispose()
        {
        }
    }
}
