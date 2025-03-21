using System.IO;
using MO_kursasch_25.ViewModels.TaskViewModels.Interface;

namespace MO_kursasch_25.Service
{
    public static class TaskFactory
    {

        public static ITask CreateTaskFromTxt(string filePath, Type taskType)
        {
            if (!typeof(ITask).IsAssignableFrom(taskType))
            {
                throw new ArgumentException("Тип задачи должен наследоваться от ITask.");
            }

            try
            {
                string content = File.ReadAllText(filePath);
                var lines = content.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                string formula = lines[0].Replace("Формула задачи: ", "").Trim();
                var parameters = new Dictionary<string, string>();

                for (int i = 2; i < lines.Length; i++) // Пропускаем первую строку (формула) и вторую (заголовок "Параметры:")
                {
                    var parts = lines[i].Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        parameters[parts[0].Trim()] = parts[1].Trim();
                    }
                }

                return CreateTaskFromParameters(taskType, formula, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при загрузке задачи из текстового файла: " + ex.Message);
            }
        }

        private static ITask CreateTaskFromParameters(Type taskType, string formula, Dictionary<string, string> parameters)
        {
            var constructor = taskType.GetConstructors().FirstOrDefault();
            if (constructor == null)
            {
                throw new Exception($"Тип {taskType.Name} не имеет публичного конструктора.");
            }

            var constructorParameters = constructor.GetParameters();

            var parameterValues = new object[constructorParameters.Length];

            for (int i = 0; i < constructorParameters.Length; i++)
            {
                var paramInfo = constructorParameters[i];
                string paramName = paramInfo.Name;

                var matchingParameter = parameters.FirstOrDefault(p =>
                    p.Key.Equals(paramName, StringComparison.OrdinalIgnoreCase));

                if (matchingParameter.Key != null)
                {
                    parameterValues[i] = Convert.ChangeType(matchingParameter.Value, paramInfo.ParameterType);
                }
                else
                {
                    parameterValues[i] = paramInfo.HasDefaultValue ? paramInfo.DefaultValue : throw new Exception($"Параметр {paramName} не найден.");
                }
            }

            var task = (ITask)Activator.CreateInstance(taskType, parameterValues);

            var formulaProperty = taskType.GetProperty("Formula");
            if (formulaProperty != null && formulaProperty.CanWrite)
            {
                formulaProperty.SetValue(task, formula);
            }

            return task;
        }

    }
}
