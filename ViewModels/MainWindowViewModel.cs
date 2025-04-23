using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive;
using Avalonia.Threading;
using ReactiveUI;
using LogManagerApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LogManagerApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly LogManager _logManager = new();
        public IEnumerable<LogMessageType> Types => Enum.GetValues(typeof(LogMessageType)).Cast<LogMessageType>();

        public ObservableCollection<LogMessage> Messages { get; } = new();

        [ObservableProperty]
        private LogMessageType selectedType = LogMessageType.Info;

        public IRelayCommand AddMessageCommand { get; }
        public IRelayCommand FilterByTypeCommand { get; }
        public IRelayCommand SaveToFileCommand { get; }

        public MainWindowViewModel()
        {
            AddMessageCommand = new AsyncRelayCommand(AddMessage);
            FilterByTypeCommand = new AsyncRelayCommand(FilterByType);
            SaveToFileCommand = new AsyncRelayCommand(SaveToFile);
        }

        private async Task AddMessage()
        {
            var msg = new LogMessage
            {
                Type = SelectedType,
                Timestamp = DateTime.Now,
                Text = $"Sample {SelectedType} message"
            };

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                _logManager.AddMessage(msg);
                Messages.Add(msg);
            });
        }

        private async Task FilterByType()
        {
            var filteredMessages = _logManager.GetByType(SelectedType);

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Messages.Clear();
                foreach (var msg in filteredMessages)
                {
                    Messages.Add(msg);
                }
            });
        }

        private async Task SaveToFile()
        {
            await Task.Run(() =>
            {
                _logManager.SaveToFile("logs.txt");
            });
        }
    }
}
    