using MO_kursasch_25.Models;
using MO_kursasch_25.ViewModels.TaskViewModels.Interface;

namespace MO_kursasch_25.Calculation
{
    public class FullSearchMethod : MethodModel
    {
        private readonly ITask task;    // Задача оптимизации
        private readonly double step;   // Шаг дискретизации для поиска
        private readonly bool maximize; // Флаг для максимизации или минимизации
        private readonly int precision; // Погрешность округления (количество знаков после запятой)
        private readonly bool iterationMode; // Флаг ограничения на количество итераций
        private readonly int iterationCount; // Максимальное количество итераций

        public FullSearchMethod()
        {
            MethodName = "Метод полного сканирования";
        }


        public FullSearchMethod((bool iterationMode, int iterationCount) iterations, ITask task, double step = 0.1, bool maximize = false, int precision = 4)
        {
            this.task = task ?? throw new ArgumentNullException(nameof(task));
            this.step = step;
            this.maximize = maximize;
            this.precision = precision;
            this.iterationMode = iterations.iterationMode;
            this.iterationCount = iterations.iterationCount;

        }

        public FuncPoint Optimize()
        {

            var lowerBounds = task.GetLowerBounds();
            var upperBounds = task.GetUpperBounds();

            var validPoints = new List<FuncPoint>();

            int currentIteration = 0;
            bool isIterationLimitReached = false;

            for (double first = lowerBounds.FirstLower; first <= upperBounds.FirstUpper && !isIterationLimitReached; first = Math.Round(first + step, precision))
            {
                for (double second = lowerBounds.SecondLower; second <= upperBounds.SecondUpper; second = Math.Round(second + step, precision))
                {
                    currentIteration++;

                    if (iterationMode && currentIteration >= iterationCount)
                    {

                        isIterationLimitReached = true;
                        break;
                    }

                    var point = new FuncPoint(first, second);

                    bool firstOrderValid = task.CheckFirstOrderConstraints(point);
                    bool secondOrderValid = task.CheckSecondOrderConstraints(point);

                    if (firstOrderValid && secondOrderValid)
                    {
                        double funcValue = task.CalculateObjectiveFunction(point);
                        funcValue = Math.Round(funcValue, precision);

                        // Сохранение точки
                        point.First = Math.Round(first, precision);
                        point.Second = Math.Round(second, precision);
                        point.FuncNum = funcValue;
                        validPoints.Add(point);
                    }
                }
            }

            if (validPoints.Count == 0)
            {
                throw new Exception("Ошибка, оптимальное значение не было найдено");
            }

            FuncPoint bestPoint = maximize
                ? validPoints.OrderByDescending(p => p.FuncNum).First()
                : validPoints.OrderBy(p => p.FuncNum).First();

            return bestPoint;
        }
    }
}
