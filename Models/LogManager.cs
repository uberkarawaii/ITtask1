using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogManagerApp.Models
{
    public class LogManager
    {
        private readonly List<LogMessage> _messages = new();

        public void AddMessage(LogMessage message)
        {
            _messages.Add(message);
        }

        public IEnumerable<LogMessage> GetByType(LogMessageType type)
        {
            return _messages.Where(m => m.Type == type);
        }

        public IEnumerable<LogMessage> GetByTimeRange(DateTime from, DateTime to)
        {
            return _messages.Where(m => m.Timestamp >= from && m.Timestamp <= to);
        }

        public void SaveToFile(string filePath)
        {
            using StreamWriter writer = new(filePath);
            foreach (var msg in _messages)
            {
                writer.WriteLine($"[{msg.Timestamp}] {msg.Type}: {msg.Text}");
            }
        }
    }
}
