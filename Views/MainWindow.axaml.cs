using Avalonia.Controls;
using LogManagerApp.ViewModels;

namespace LogManagerApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new MainWindowViewModel();
            DataContext = viewModel;

            viewModel.SaveFileDialogInteraction.RegisterHandler(async interaction =>
            {
                var dialog = new SaveFileDialog
                {
                    Title = "Save Log File",
                    Filters =
        {
            new FileDialogFilter { Name = "Text Files", Extensions = { "txt" } },
            new FileDialogFilter { Name = "All Files", Extensions = { "*" } }
        },
                    InitialFileName = "logs.txt"
                };

                var result = await dialog.ShowAsync(this);
                interaction.SetOutput(result);
            });
        }
    }
}