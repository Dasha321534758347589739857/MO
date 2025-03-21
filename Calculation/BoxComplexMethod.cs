using MO_kursasch_25.Models;
using MO_kursasch_25.ViewModels.TaskViewModels.Interface;

namespace MO_kursasch_25.Calculation
{
    public class BoxComplexMethod : MethodModel
    {
        private readonly ITask task;
        private readonly int n = 2;       // Количество переменных
        private readonly int complexSize; // Размер комплекса (N ≥ n + 1)
        private readonly double epsilon;  // Точность сходимости
        private readonly int precision;   // Погрешность округления (количество знаков после запятой)
        private readonly Random random;
        private readonly bool findMaximum; // Если true - ищем максимум, если false - минимум
        private readonly bool iterationMode; // Режим итераций (true - ограничение по количеству итераций)
        private readonly int iterationCount; // Максимальное количество итераций, если iterationMode = true

        public BoxComplexMethod((bool iterationMode, int iterationCount) iterations, ITask task, bool findMaximum = false, int complexSize = 6, double epsilon = 0.1, int precision = 2)
        {
            this.task = task ?? throw new ArgumentNullException(nameof(task));
            this.complexSize = complexSize >= n + 1 ? complexSize : n + 1;
            this.epsilon = epsilon;
            this.precision = precision;
            this.random = new Random();
            this.findMaximum = findMaximum;
            this.iterationMode = iterations.iterationMode;
            this.iterationCount = iterations.iterationCount;

        }

        public BoxComplexMethod()
        {
            MethodName = "Комплексный метод Бокса";
        }


        public FuncPoint Optimize()
        {
            var lowerBounds = task.GetLowerBounds();
            var upperBounds = task.GetUpperBounds();

            FuncPoint[] complex = CreateInitialComplex(lowerBounds, upperBounds);

            int iterationLimit = iterationMode ? iterationCount : 10000;
            int iterations = 0;

            while (true)
            {
                for (int i = 0; i < complex.Length; i++)
                {
                    complex[i].FuncNum = task.CalculateObjectiveFunction(complex[i]);
                }

                (int bestIndex, int worstIndex) = FindBestAndWorstIndices(complex);

                double[] center = CalculateCenter(complex, worstIndex);

                if (!iterationMode && IsConverged(complex, center, bestIndex, worstIndex))
                {
                    break;
                }

                if (iterations >= iterationLimit)
                {
                    break;
                }

                FuncPoint newPoint = GenerateNewPoint(center, complex[worstIndex]);

                AdjustPointForConstraints(ref newPoint, lowerBounds, upperBounds, center, complex[bestIndex], complex[worstIndex].FuncNum);

                complex[worstIndex] = newPoint;
                iterations++;
            }

            int finalBestIndex = FindBestAndWorstIndices(complex).maxNumIndex;
            FuncPoint result = new FuncPoint(
                Math.Round(complex[finalBestIndex].First, precision),
                Math.Round(complex[finalBestIndex].Second, precision)
            );

            double rawFuncValue = task.CalculateObjectiveFunction(result);
            result.FuncNum = Math.Round(rawFuncValue, precision);
            return result;
        }


        private FuncPoint[] CreateInitialComplex(
     (double FirstLower, double SecondLower) lowerBounds,
     (double FirstUpper, double SecondUpper) upperBounds)
        {
            FuncPoint[] complex = new FuncPoint[complexSize];

            int P = 0; // Количество точек, удовлетворяющих ограничениям

            for (int j = 0; j < complexSize; j++)
            {
                FuncPoint point;
                bool validPoint = false;
                int attempts = 0;
                const int maxAttempts = 100;

                while (!validPoint && attempts < maxAttempts)
                {
                    double first = lowerBounds.FirstLower + random.NextDouble() * (upperBounds.FirstUpper - lowerBounds.FirstLower);
                    double second = lowerBounds.SecondLower + random.NextDouble() * (upperBounds.SecondUpper - lowerBounds.SecondLower);

                    point = new FuncPoint(first, second);
                    double rawFuncValue = task.CalculateObjectiveFunction(point);
                    point.FuncNum = Math.Round(rawFuncValue, precision);

                    bool firstOrderValid = task.CheckFirstOrderConstraints(point);
                    bool secondOrderValid = task.CheckSecondOrderConstraints(point);

                    if (firstOrderValid && secondOrderValid)
                    {
                        complex[j] = point;
                        validPoint = true;
                        P++;
                    }
                    else if (P > 0)
                    {
                        int maxShiftAttempts = 10000;
                        int shiftAttempts = 0;

                        while (!validPoint && shiftAttempts < maxShiftAttempts)
                        {
                            double[] center = CalculateCenter(complex.Take(P).ToArray(), -1);
                            point.First = (point.First + center[0]) / 2;
                            point.Second = (point.Second + center[1]) / 2;

                            // Повторная проверка ограничений
                            firstOrderValid = task.CheckFirstOrderConstraints(point);
                            secondOrderValid = task.CheckSecondOrderConstraints(point);

                            if (firstOrderValid && secondOrderValid)
                            {
                                complex[j] = point;
                                validPoint = true;
                                P++;
                            }

                            shiftAttempts++;
                        }


                    }

                    attempts++;
                }

                if (!validPoint)
                {
                    throw new Exception("Не удалось сформировать начальный комплекс: слишком строгие ограничения.");
                }
            }

            return complex;
        }


        private (int maxNumIndex, int minNumIndex) FindBestAndWorstIndices(FuncPoint[] complex)
        {
            for (int i = 0; i < complex.Length; i++)
            {
            }

            var minNumIndex = complex.Select((point, index) => new { point, index })
                .OrderBy(p => findMaximum ? p.point.FuncNum : -p.point.FuncNum)
                .First().index;
            var maxNumIndex = complex.Select((point, index) => new { point, index })
                .OrderBy(p => findMaximum ? -p.point.FuncNum : p.point.FuncNum)
                .First().index;
            return (maxNumIndex, minNumIndex);
        }

        private double[] CalculateCenter(FuncPoint[] complex, int excludeIndex)
        {
            double[] center = new double[2];
            int count = 0;

            for (int i = 0; i < complex.Length; i++)
            {
                if (i != excludeIndex && complex[i] != null)
                {
                    center[0] += complex[i].First;
                    center[1] += complex[i].Second;
                    count++;
                }
            }

            if (count > 0)
            {
                center[0] /= count;
                center[1] /= count;
            }

            return center;
        }


        private bool IsConverged(FuncPoint[] complex, double[] center, int bestIndex, int worstIndex)
        {
            double bestToCenter = Math.Sqrt(
               (complex[bestIndex].First - center[0]) * (complex[bestIndex].First - center[0]) +
               (complex[bestIndex].Second - center[1]) * (complex[bestIndex].Second - center[1])
           );
            double worstToCenter = Math.Sqrt(
                (complex[worstIndex].First - center[0]) * (complex[worstIndex].First - center[0]) +
                (complex[worstIndex].Second - center[1]) * (complex[worstIndex].Second - center[1])
            );
            double b = Math.Sqrt(0.5 * (bestToCenter * bestToCenter + worstToCenter * worstToCenter));


            bool converged = b < epsilon;
            return converged;
        }


        private FuncPoint GenerateNewPoint(double[] center, FuncPoint worstPoint)
        {
            double first = 2.3 * center[0] - 1.3 * worstPoint.First;
            double second = 2.3 * center[1] - 1.3 * worstPoint.Second;
            var newPoint = new FuncPoint(first, second);
            double rawFuncValue = task.CalculateObjectiveFunction(newPoint);
            newPoint.FuncNum = Math.Round(rawFuncValue, precision);
            return newPoint;
        }


        private void AdjustPointForConstraints(
            ref FuncPoint point,
            (double FirstLower, double SecondLower) lowerBounds,
            (double FirstUpper, double SecondUpper) upperBounds,
            double[] center,
            FuncPoint bestPoint,
            double worstValue)
        {

            bool firstOrderValid = task.CheckFirstOrderConstraints(point);
            if (!firstOrderValid)
            {
                if (point.First < lowerBounds.FirstLower)
                {
                    point.First = lowerBounds.FirstLower;
                }
                if (point.First > upperBounds.FirstUpper)
                {
                    point.First = upperBounds.FirstUpper;
                }
                if (point.Second < lowerBounds.SecondLower)
                {
                    point.Second = lowerBounds.SecondLower;
                }
                if (point.Second > upperBounds.SecondUpper)
                {
                    point.Second = upperBounds.SecondUpper;
                }
                double rawFuncValue = task.CalculateObjectiveFunction(point);
                point.FuncNum = Math.Round(rawFuncValue, precision);
            }

            int secondOrderAttempts = 0;
            const int maxSecondOrderAttempts = 10000;
            bool secondOrderValid = task.CheckSecondOrderConstraints(point);
            while (!secondOrderValid && secondOrderAttempts < maxSecondOrderAttempts)
            {
                point.First = (point.First + center[0]) / 2;
                point.Second = (point.Second + center[1]) / 2;
                double rawFuncValue = task.CalculateObjectiveFunction(point);
                point.FuncNum = Math.Round(rawFuncValue, precision);
                secondOrderValid = task.CheckSecondOrderConstraints(point);
                secondOrderAttempts++;
            }

            int improvementAttempts = 0;
            const int maxImprovementAttempts = 50;
            bool improvementCondition = findMaximum ? point.FuncNum < bestPoint.FuncNum : point.FuncNum > bestPoint.FuncNum;
            while (improvementCondition && improvementAttempts < maxImprovementAttempts)
            {
                point.First = (point.First + bestPoint.First) / 2;
                point.Second = (point.Second + bestPoint.Second) / 2;
                double rawFuncValue = task.CalculateObjectiveFunction(point);
                point.FuncNum = Math.Round(rawFuncValue, precision);
                improvementCondition = findMaximum ? point.FuncNum < bestPoint.FuncNum : point.FuncNum > bestPoint.FuncNum;
                improvementAttempts++;
            }


        }
    }
}
