using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MO_kursasch_25.Calculation;
using MO_kursasch_25.DbConnector;
using MO_kursasch_25.DbConnector.Models;
using MO_kursasch_25.Models;
using MO_kursasch_25.Service;
using MO_kursasch_25.Windows;

namespace MO_kursasch_25.ViewModels
{
    public partial class AdminViewModel : ObservableObject
    {
        private readonly UsersContext _context;

        [ObservableProperty]
        private ObservableCollection<User> _users;


        [ObservableProperty]
        private User _selectedUser;


        [ObservableProperty]
        private User _currentAdmin;


        [ObservableProperty]
        private ObservableCollection<FormulaModel> _formulas;

        [ObservableProperty]
        private ObservableCollection<MethodModel> _methods;

        public AdminViewModel(User currentAdmin)
        {
            _context = new UsersContext();
            CurrentAdmin = currentAdmin;
            LoadUsers();
            InitializeFormulas();
            InitializeMethods();
        }


        private void LoadUsers()
        {
            Users = new ObservableCollection<User>(_context.Users.ToList());
        }


        private void InitializeFormulas()
        {
            Formulas = new ObservableCollection<FormulaModel>
            {
                new() { Formula = "С  =  α * (Т2– Т1)^А1 + β * 1 /  V1 * (Т1+Т2 -  γ *V2)^A2" }
            };
        }


        private void InitializeMethods()
        {
            Methods = new ObservableCollection<MethodModel>
            {
                new BoxComplexMethod(),
                new FullSearchMethod()
            };
        }


        [RelayCommand]
        private void ResetPassword()
        {
            if (SelectedUser != null)
            {
                var newPassword = GenerationService.GenerateRandomPassword();
                SelectedUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _context.SaveChanges();
                LoadUsers();
                MessageBox.Show($"Пароль успешно сброшен. Новый пароль для входа: {newPassword}", "Сброс пароля", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для сброса пароля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        [RelayCommand]
        private void DeleteUser()
        {
            if (SelectedUser != null)
            {
                if (SelectedUser.Id == CurrentAdmin.Id)
                {
                    MessageBox.Show("Вы не можете удалить самого себя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _context.Users.Remove(SelectedUser);
                _context.SaveChanges();
                LoadUsers();
                MessageBox.Show("Пользователь успешно удален.", "Удаление пользователя", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите пользователя для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        [RelayCommand]
        private void AddUser()
        {
            var addUserDialog = new AddUserWindow();
            if (addUserDialog.ShowDialog() == true)
            {
                var newUser = addUserDialog.NewUser;
                if (newUser != null)
                {
                    // Проверка на уникальность логина
                    bool isUsernameUnique = !_context.Users.Any(u => u.Username == newUser.Username);

                    if (!isUsernameUnique)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существует. Пожалуйста, выберите другой логин.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Хэшируем пароль перед сохранением
                    newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newUser.PasswordHash);
                    _context.Users.Add(newUser);
                    _context.SaveChanges();
                    LoadUsers();
                    MessageBox.Show("Пользователь успешно добавлен.", "Добавление пользователя", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }


        [RelayCommand]
        private void ChangePassword()
        {
            var changePasswordWindow = new ChangePasswordWindow();
            if (changePasswordWindow.ShowDialog() == true)
            {
                var user = _context.Users.Where(user => user.Id == CurrentAdmin.Id).FirstOrDefault();
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordWindow.NewPassword);
                _context.SaveChanges();
                MessageBox.Show("Пароль успешно изменен.", "Изменение пароля", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }


        [RelayCommand]
        private void AddMethod()
        {
            MessageBox.Show("Функционал добавления метода будет реализован позже.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        [RelayCommand]
        private void DeleteMethod()
        {
            MessageBox.Show("Функционал удаления метода будет реализован позже.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        [RelayCommand]
        private void AddFunction()
        {
            MessageBox.Show("Функционал добавления функции будет реализован позже.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        [RelayCommand]
        private void DeleteFunction()
        {
            MessageBox.Show("Функционал удаления функции будет реализован позже.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
