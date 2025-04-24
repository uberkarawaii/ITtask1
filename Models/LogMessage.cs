using System;
namespace LogManagerApp.Models
{
    public enum LogMessageType
    {
        Error,
        Warning,
        Info
    }
}

namespace LogManagerApp.Models
{
    public struct LogMessage
    {
        public LogMessageType Type { get; set; }
        public DateTime Timestamp { get; set; }
        public string Text { get; set; }
    }
}