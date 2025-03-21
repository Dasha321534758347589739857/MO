using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MO_kursasch_25.DbConnector;
using MO_kursasch_25.DbConnector.Models;
using MO_kursasch_25.ViewModels.UserViewModels;
using MO_kursasch_25.Windows;



namespace MO_kursasch_25.ViewModels
{
    public partial class AuthenticationViewModel : ObservableObject
    {
        private UsersContext _context;

        private PasswordBox password;
        [ObservableProperty]
        private string error = "";
        [ObservableProperty]
        private string login = "";
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EnterCommand))]
        private int tryEnterCount = 0;

        public AuthenticationViewModel(PasswordBox password)
        {
            this.password = password;
        }

        [RelayCommand(CanExecute = nameof(CanEnter))]
        private void Enter(Window window)
        {
            var user = Autorizate(this.Login, password.Password);
            if (user != null)
            {
                if (user.Role == Role.User)
                {
                    var mainWindow = new MainWindow(new MainWindowViewModel(user));
                    mainWindow.Closing += IsWindowClosing;
                    mainWindow.Show();
                }
                else
                {
                    var adminWindow = new AdminWindow(new AdminViewModel(user));
                    adminWindow.Closing += IsWindowClosing;
                    adminWindow.Show();
                }
                Application.Current.Windows.OfType<AuthenticationWindow>().FirstOrDefault()?.Hide();
            }
            else
            {
                MessageBox.Show("Некорректное имя пользователя или пароль.", "Ошибка авторизации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void IsWindowClosing(object? sender, CancelEventArgs e)
        {
            var loginWindow = Application.Current.Windows.OfType<AuthenticationWindow>().FirstOrDefault();

            if (loginWindow != null)
            {
                loginWindow.Show();

                loginWindow.Activate();
            }
        }

        private bool CanEnter() =>
           TryEnterCount <= 10;
        private User Autorizate(string username, string password)
        {
            _context = new UsersContext();
            var users = _context.Users.ToList();
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return null; // Пользователь не найден
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            return isPasswordValid ? user : null; // Возвращаем пользователя, если пароль верный
        }
    }
}
