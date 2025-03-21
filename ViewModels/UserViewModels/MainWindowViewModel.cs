using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MO_kursasch_25.Calculation;
using MO_kursasch_25.DbConnector;
using MO_kursasch_25.DbConnector.Models;
using MO_kursasch_25.Enums;
using MO_kursasch_25.Models;
using MO_kursasch_25.Service;
using MO_kursasch_25.ViewModels.TaskViewModels;
using MO_kursasch_25.Windows;


namespace MO_kursasch_25.ViewModels.UserViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly UsersContext _context;

        [ObservableProperty]
        private Dictionary<string, Type> _methods = new Dictionary<string, Type>
            {
                { "Метод бокса", typeof(BoxComplexMethod) },
            {"Метод сканирования", typeof(FullSearchMethod) }
            };
        [ObservableProperty]
        private Type _selectedMethod;

        private User _currentUser;

        [ObservableProperty]
        TaskV26ViewModel taskV26ViewModel;

        [ObservableProperty]
        LimitationsViewModel limitationsViewModel;

        [ObservableProperty]
        private FuncPoint extrPoint;

        [ObservableProperty]
        private VisualizationViewModel visualizationViewModel;

        public MainWindowViewModel(User user)
        {
            _currentUser = user;
            _context = new();
            taskV26ViewModel = new(new TaskV26());
            limitationsViewModel = new LimitationsViewModel();
        }

        [RelayCommand]
        void OpenAboutWindow()
        {
            var aboutWindow = new AboutWindow(new AboutViewModel());
            aboutWindow.Show();
            aboutWindow.Closing += AboutWindowClosing;
            System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault()?.Hide();
        }

        private void AboutWindowClosing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            var mainWindow = System.Windows.Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

            if (mainWindow != null)
            {
                mainWindow.Show();

                mainWindow.Activate();
            }
        }

        [RelayCommand]
        private void ChangePassword()
        {
            var changePasswordWindow = new ChangePasswordWindow();
            if (changePasswordWindow.ShowDialog() == true)
            {
                var user = _context.Users.Where(user => user.Id == _currentUser.Id).FirstOrDefault();
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordWindow.NewPassword);
                _context.SaveChanges();
                MessageBox.Show("Пароль успешно изменен.", "Изменение пароля", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        [RelayCommand]
        void Calc()
        {
            try
            {
                if (SelectedMethod is not null)
                {
                    var optTask = TaskV26ViewModel.GetTask();
                    var limitations = LimitationsViewModel.GetLimitation();
                    this.VisualizationViewModel = new VisualizationViewModel(optTask.task, limitations);
                    if (SelectedMethod == typeof(BoxComplexMethod))
                    {
                        var boxComplexMethod = new BoxComplexMethod(limitations.Item1, optTask.task, optTask.extrType, epsilon: limitations.epsilon, precision: limitations.precision);
                        var point = boxComplexMethod.Optimize();
                        point.FuncNum *= optTask.tau;
                        ExtrPoint = point;
                    }
                    else if (SelectedMethod == typeof(FullSearchMethod))
                    {
                        var fullSearchMethod = new FullSearchMethod(limitations.Item1, optTask.task, maximize: optTask.extrType, step: limitations.epsilon, precision: limitations.precision);
                        var point = fullSearchMethod.Optimize();
                        point.FuncNum *= optTask.tau;
                        ExtrPoint = point;
                    }
                }
                else
                {
                    MessageBox.Show("Не выбран метод оптимизации", "Ошибка расчётов", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при расчёте оптимального значения функции:{ex.Message}", "Ошибка расчётов", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task ExportResultsToTxt()
        {
            try
            {
                string filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                string filePath = FileProvider.GetFilePath(FileMode.Save, filter, $"Results", "Сохранить параметры в TXT");

                if (!string.IsNullOrEmpty(filePath))
                {
                    await TxtExplorer.ExportResultsToTxtAsync(funcPoint: ExtrPoint, task: TaskV26ViewModel.GetTask().task, filePath: filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте результатов в текстовый файл: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        [RelayCommand]
        private async Task ExportParametersToTxt()
        {
            try
            {
                string filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                string filePath = FileProvider.GetFilePath(FileMode.Save, filter, $"Parameters", "Сохранить параметры в TXT");

                if (!string.IsNullOrEmpty(filePath))
                {
                    await TxtExplorer.ExportParametersToTxtAsync(TaskV26ViewModel.GetTask().task, filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте параметров в текстовый файл: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }

        }
        [RelayCommand]
        private async Task ImportParametersFromTxt()
        {
            try
            {
                string filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";

                string filePath = FileProvider.GetFilePath(FileMode.Open, filter, "Выберите текстовый файл с параметрами");

                if (!string.IsNullOrEmpty(filePath))
                {
                    TaskV26 taskFromTxt = (TaskV26)Service.TaskFactory.CreateTaskFromTxt(filePath, typeof(TaskV26));

                    TaskV26ViewModel = new TaskV26ViewModel(taskFromTxt);

                    MessageBox.Show("Параметры успешно загружены из текстового файла.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке параметров из текстового файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
