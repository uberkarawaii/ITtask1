using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive;
using ReactiveUI;
using Avalonia.Threading;
using Avalonia.Controls;
using LogManagerApp.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia.Controls.ApplicationLifetimes;
using System.Reactive.Linq;

namespace LogManagerApp.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        //неизменияемый список сообщений
        private readonly LogManager _logManager = new();
        
        //для открытия диалогового окна для сохранения файла
        public Interaction<Unit, string?> SaveFileDialogInteraction { get; } = new();

        //4 метки для фильтров времени
        [ObservableProperty]
        private DateTimeOffset? filterStartDate = DateTimeOffset.Now.Date;

        [ObservableProperty]
        private DateTimeOffset? filterEndDate = DateTimeOffset.Now.Date;

        [ObservableProperty]
        private TimeSpan filterStartTime = DateTime.Now.TimeOfDay;

        [ObservableProperty]
        private TimeSpan filterEndTime = DateTime.Now.TimeOfDay;

        //перечилсение для выбора типа сообщения (выпадающий список)
        public IEnumerable<LogMessageType> Types => Enum.GetValues<LogMessageType>();

        //все сообщения отражаемые снизу
        public ObservableCollection<LogMessage> Messages { get; } = new();

        //то, что было выбрано в выпадающем окошке
        [ObservableProperty]
        private LogMessageType selectedType = LogMessageType.Info;

        //для всех функций по кнопкам
        public IRelayCommand AddMessageCommand { get; }
        public IRelayCommand FilterByTypeCommand { get; }
        public IRelayCommand SaveToFileCommand { get; }
        public IRelayCommand FilterByTimeCommand { get; }

        public MainWindowViewModel()
        {
            AddMessageCommand = new AsyncRelayCommand(AddMessage);
            FilterByTypeCommand = new AsyncRelayCommand(FilterByType);
            SaveToFileCommand = new AsyncRelayCommand(SaveToFile);
            FilterByTimeCommand = new AsyncRelayCommand(FilterByTime);

            SaveFileDialogInteraction.RegisterHandler(async interaction =>
            {
                var dialog = new SaveFileDialog
                {
                    Title = "Save Logs",
                    Filters =
                    {
                        new FileDialogFilter { Name = "Text files", Extensions = { "txt" } }
                    },
                    DefaultExtension = "txt"
                };

                var mainWindow = App.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                    ? desktop.MainWindow
                    : null;

                var result = mainWindow != null
                    ? await dialog.ShowAsync(mainWindow)
                    : null;

                interaction.SetOutput(result);
            });
        }

        private async Task AddMessage()
        {
            var msg = new LogMessage
            {
                Type = SelectedType,
                Timestamp = DateTime.Now,
                Text = $"[{DateTime.Now:HH:mm:ss}] Sample {SelectedType} message"
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
            var filePath = await SaveFileDialogInteraction.Handle(Unit.Default);

            if (!string.IsNullOrEmpty(filePath))
            {
                await Task.Run(() =>
                {
                    _logManager.SaveToFile(filePath);
                });
            }
        }

        private async Task FilterByTime()
        {
            if (FilterStartDate == null || FilterEndDate == null)
                return;

            var start = FilterStartDate.Value.Date + FilterStartTime;
            var end = FilterEndDate.Value.Date + FilterEndTime;

            var filtered = _logManager.GetAll()
                .Where(m => m.Timestamp >= start && m.Timestamp <= end)
                .ToList();

            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                Messages.Clear();
                foreach (var msg in filtered)
                {
                    Messages.Add(msg);
                }
            });
        }

    }
}
