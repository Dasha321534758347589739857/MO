using CommunityToolkit.Mvvm.ComponentModel;
using MO_kursasch_25.Models;
using MO_kursasch_25.Pages;
using MO_kursasch_25.ViewModels.TaskViewModels.Interface;

namespace MO_kursasch_25.ViewModels
{
    public partial class VisualizationViewModel : ObservableObject
    {
        private readonly ITask task;
        private readonly double epsilon;
        private readonly int precision;

        [ObservableProperty]
        TableViewModel tableViewModel;

        [ObservableProperty]
        private Chart2D chart2DPage;

        [ObservableProperty]
        private Chart3D chart3DPage;

        [ObservableProperty]
        private TablePage tablePage;

        public VisualizationViewModel(ITask task, ((bool terationMode, int itterationCount), double epsilon, int precision) limitations)
        {
            this.task = task;
            this.epsilon = limitations.epsilon;
            this.precision = limitations.precision;
            this.TableViewModel = new(task, epsilon, precision);
            Chart2DPage = new Chart2D(this);
            Chart3DPage = new Chart3D(this);
            TablePage = new TablePage(TableViewModel);
        }



        public (List<double> DataX, List<double> DataY, List<double> DataZ) ParseData()
        {
            List<double> DataX = new();
            List<double> DataY = new();
            List<double> DataZ = new();
            var lowerBounds = task.GetLowerBounds();
            var upperBounds = task.GetUpperBounds();
            // Перебор всех возможных точек в пределах ограничений
            int firstSteps = (int)((upperBounds.FirstUpper - lowerBounds.FirstLower) / epsilon) + 1;
            int secondSteps = (int)((upperBounds.SecondUpper - lowerBounds.SecondLower) / epsilon) + 1;
            for (double first = lowerBounds.FirstLower; first <= upperBounds.FirstUpper; first += epsilon)
            {
                for (double second = lowerBounds.SecondLower; second <= upperBounds.SecondUpper; second += epsilon)
                {
                    // Создаем точку с текущими значениями
                    var point = new FuncPoint(first, second);
                    // Вычисляем значение целевой функции
                    double funcValue = task.CalculateObjectiveFunction(point);
                    funcValue = Math.Round(funcValue, precision);
                    // Округляем значение функции для согласованности с precision
                    funcValue = Math.Round(funcValue, precision);
                    if (task.CheckSecondOrderConstraints(point))
                    {
                        DataX.Add(first);
                        DataY.Add(second);
                        DataZ.Add(funcValue);
                    }
                }
            }
            return (DataX, DataY, DataZ);
        }
    }
}
