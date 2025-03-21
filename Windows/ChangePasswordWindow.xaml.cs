using System.Windows;

namespace MO_kursasch_25.Windows
{
    /// <summary>
    /// Логика взаимодействия для ChangePasswordWindow.xaml
    /// </summary>
    public partial class ChangePasswordWindow : Window
    {
        public ChangePasswordWindow()
        {
            InitializeComponent();
        }


        public string NewPassword { get; private set; }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Новый пароль и подтверждение пароля не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NewPassword = NewPasswordBox.Password;

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Закрываем окно с результатом false
            DialogResult = false;
            Close();
        }
    }
}
