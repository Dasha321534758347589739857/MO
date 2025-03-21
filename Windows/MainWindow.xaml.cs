using System.Windows;
using MO_kursasch_25.ViewModels.UserViewModels;



namespace MO_kursasch_25
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow(MainWindowViewModel mainWindowViewModel)
        {
            DataContext = mainWindowViewModel;
            InitializeComponent();
        }


    }
}