using System.Reflection;
using MO_kursasch_25.Models;

namespace MO_kursasch_25.ViewModels.TaskViewModels.Interface
{
    public interface ITask
    {

        public double CalculateObjectiveFunction(FuncPoint point);


        public string GetFormula();


        public bool CheckFirstOrderConstraints(FuncPoint point);


        public bool CheckSecondOrderConstraints(FuncPoint point);


        public (double FirstLower, double SecondLower) GetLowerBounds();


        public (double FirstUpper, double SecondUpper) GetUpperBounds();


        public string GetParametersString()
        {

            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);


            var parameters = new List<string>();
            foreach (var property in properties)
            {
                var value = property.GetValue(this);
                parameters.Add($"{property.Name}: {value}");
            }

            return string.Join(", ", parameters);
        }
    }
}
