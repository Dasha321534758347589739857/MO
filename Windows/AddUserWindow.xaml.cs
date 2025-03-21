using System.Windows;
using System.Windows.Controls;
using MO_kursasch_25.DbConnector.Models;

namespace MO_kursasch_25.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        public AddUserWindow()
        {
            InitializeComponent();
        }

        public User NewUser { get; private set; }


        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            NewUser = new User
            {
                Username = UsernameTextBox.Text,
                PasswordHash = PasswordBox.Password,
                Role = RoleComboBox.SelectedIndex == 0 ? Role.User : Role.Admin
            };

            DialogResult = true;
            Close();
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

}
