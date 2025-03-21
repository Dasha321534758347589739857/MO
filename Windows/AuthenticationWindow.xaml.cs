using System.Windows;
using MO_kursasch_25.DbConnector;

namespace MO_kursasch_25.Windows
{
    /// <summary>
    /// Логика взаимодействия для AuthenticationWindow.xaml
    /// </summary>
    public partial class AuthenticationWindow : Window
    {
        public AuthenticationWindow()
        {
            InitializeComponent();
            DataContext = new ViewModels.AuthenticationViewModel(Password);
            DbInitializer.Initialize(new UsersContext());
        }
    }
}
