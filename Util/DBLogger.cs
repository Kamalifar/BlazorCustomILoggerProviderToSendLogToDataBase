﻿using System.Text.Json;
using System.Text.Json.Serialization;
using SendBlazorLoggerToDataBase.Entities;

namespace SendBlazorLoggerToDataBase.Util
{
    public class DBLogger:ILogger
    {
        private readonly LogLevel _minLevel;
        private readonly DbLoggerProvider _loggerProvider;
        private readonly string _categoryName;

        public DBLogger(
            DbLoggerProvider loggerProvider,
            string categoryName
        )
        {
            _loggerProvider= loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
            _categoryName= categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NoopDisposable();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _minLevel;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);

            if (exception != null)
            {
                message = $"{message}{Environment.NewLine}{exception}";
            }

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            var dblLogItem = new DBLog()
            {
                EventName = eventId.Name,
                LogLevel = logLevel.ToString(),
                Message = $"{_categoryName}{Environment.NewLine}{message}",
                StackTrace=exception?.StackTrace
            };
            _loggerProvider.AddLogItem(dblLogItem);
        }

      

        private class NoopDisposable : IDisposable
        {
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
            }
        }
    }
}
