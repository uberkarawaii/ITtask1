using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogManagerApp.Models;

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

        public void SaveToFile(string path)
        {
            var lines = _messages.Select(m => $"[{m.Timestamp:yyyy-MM-dd HH:mm:ss}] {m.Type}");
            File.WriteAllLines(path, lines);
        }

        public IEnumerable<LogMessage> GetAll()
        {
            return _messages;
        }
    }
}
