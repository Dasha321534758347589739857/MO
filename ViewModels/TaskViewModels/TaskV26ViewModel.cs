using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Office.Interop.Excel;
using MO_kursasch_25.Service;
using MO_kursasch_25.ViewModels.TaskViewModels.Interface;

namespace MO_kursasch_25.ViewModels.TaskViewModels
{
    public partial class TaskV26ViewModel : ObservableValidator
    {
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Alpha должен быть больше 0")]
        private double alphaP = 0;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Betta должен быть больше 0")]
        private double betaP = 0;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "Gamma должен быть больше 0")]
        private double gammaP = 0;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "A1 должен быть больше 0")]
        private double a1P = 0;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "A2 должен быть больше 0")]
        private double a2P = 0;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "V1 должен быть больше 0")]
        private double v1P = 0;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Range(0.00000001, double.MaxValue, ErrorMessage = "V2 должен быть больше 0")]
        private double v2P = 0;

        [ObservableProperty]
        private double minT1 = 0;

        [ObservableProperty]
        private double maxT1 = 0;

        [ObservableProperty]
        private double minT2 = 0;

        [ObservableProperty]
        private double maxT2 = 0;

        [ObservableProperty]
        private double limitT = 0;

        [ObservableProperty]
        private double tP = 40;

        [ObservableProperty]
        private bool _extrType = true;


        public TaskV26ViewModel(TaskV26 taskV26)
        {
            if (taskV26 == null)
                throw new ArgumentNullException(nameof(taskV26));


            AlphaP = taskV26.AlphaP;
            BetaP = taskV26.BetaP;
            GammaP = taskV26.GammaP;
            A1P = taskV26.A1P;
            A2P = taskV26.A2P;
            V1P = taskV26.V1P;
            V2P = taskV26.V2P;
            MinT1 = taskV26.MinT1;
            MaxT1 = taskV26.MaxT1;
            MinT2 = taskV26.MinT2;
            MaxT2 = taskV26.MaxT2;
            LimitT = taskV26.LimitT;
        }
        public (ITask task, bool extrType, double tau) GetTask()
        {
            ValidateAllProperties();
            // Список для накопления ошибок
            List<string> errors = new List<string>();
            ClearErrors();


            ValidateAllProperties();


            if (HasErrors)
            {
                throw new ValidationException("Невозможно получить значения из-за ошибок валидации");
            }

            if (MinT1 > MaxT1)
            {
                errors.Add("Нижняя граница первой переменной должна быть меньше или равна верхней границе.");
            }

            if (MinT2 > MaxT2)
            {
                errors.Add("Нижняя граница второй переменной должна быть меньше или равна верхней границе.");
            }

            if (errors.Count > 0)
            {
                throw new ValidationException(ErorrsCombinator.CombinateErorrs(errors));
            }


            var task = new TaskV26(
        alphaP: AlphaP,
        betaP: BetaP,
        gammaP: GammaP,
        a1P: A1P,
        a2P: A2P,
        v1P: V1P,
        v2P: V2P,
        limitT: LimitT,
        minT1: MinT1,
        maxT1: MaxT1,
        minT2: MinT2,
        maxT2: MaxT2
    );


            return (task, ExtrType, TP);

        }
    }
}
