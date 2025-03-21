using System.Windows;
using MO_kursasch_25.ViewModels;



namespace MO_kursasch_25.Windows
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow(AdminViewModel adminWindow)
        {
            DataContext = adminWindow;
            InitializeComponent();

        }
    }
}
