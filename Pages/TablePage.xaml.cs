using System.Windows.Controls;
using MO_kursasch_25.ViewModels;

namespace MO_kursasch_25.Pages
{
    /// <summary>
    /// Логика взаимодействия для TablePage.xaml
    /// </summary>
    public partial class TablePage : Page
    {
        public TablePage(TableViewModel tableViewModel)
        {
            DataContext = tableViewModel;
            InitializeComponent();
        }
    }
}
