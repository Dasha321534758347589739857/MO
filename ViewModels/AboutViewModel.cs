using CommunityToolkit.Mvvm.ComponentModel;
using MO_kursasch_25.Service;

namespace MO_kursasch_25.ViewModels
{
    public partial class AboutViewModel : ObservableObject
    {
        [ObservableProperty]
        private string sourceString = PathService.GetCurentFolderPath("Resources\\About.html");
        public AboutViewModel() { }
    }
}
