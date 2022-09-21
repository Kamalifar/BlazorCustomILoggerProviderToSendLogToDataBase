namespace SendBlazorLoggerToDataBase.Entities
{
    public class DBLog
    {
        public int  DBLogId { get; set; }
        public string LogLevel { get; set; }
        public string EventName { get; set; }
        public string ExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public DateTime CreatedDate { get; set; }=DateTime.Now;
    }
}
