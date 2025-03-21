using System.Windows;
using MO_kursasch_25.ViewModels;

namespace MO_kursasch_25.Windows
{
    /// <summary>
    /// Логика взаимодействия для AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow(AboutViewModel aboutViewModel)
        {
            DataContext = aboutViewModel;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AboutViewModel aboutViewModel)
            {
                MainView.Navigate(aboutViewModel.SourceString);
            };
        }
    }
}
