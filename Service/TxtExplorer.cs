using System.IO;
using System.Text;
using System.Windows;
using MO_kursasch_25.Models;
using MO_kursasch_25.ViewModels.TaskViewModels.Interface;

namespace MO_kursasch_25.Service
{
    public static class TxtExplorer
    {
        public static async Task ExportResultsToTxtAsync(FuncPoint funcPoint, ITask task, string filePath)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (funcPoint is null)
                    {
                        throw new Exception("Оптимальное значение функции не было расчитано.");
                    }
                    using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                    {
                        string formula = task.GetFormula();
                        writer.WriteLine("Формула задачи:");
                        writer.WriteLine(formula);
                        writer.WriteLine();

                        // Записываем параметры задачи
                        string parametersString = task.GetParametersString();
                        string[] parameters = parametersString.Split(new[] { ", " }, StringSplitOptions.None);

                        writer.WriteLine("Параметры задачи:");
                        foreach (string param in parameters)
                        {
                            string[] parts = param.Split(": ");
                            if (parts.Length == 2)
                            {
                                writer.WriteLine($"{parts[0]}: {parts[1]}");
                            }
                        }
                        writer.WriteLine();

                        writer.WriteLine("Оптимальная точка:");
                        writer.WriteLine($"First={funcPoint.First}");
                        writer.WriteLine($"Second={funcPoint.Second}");
                        writer.WriteLine($"FuncNum={funcPoint.FuncNum}");
                    }

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Результаты успешно экспортированы в файл: {filePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Ошибка при экспорте результатов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            });
        }

        public static async Task ExportParametersToTxtAsync(ITask task, string filePath)
        {
            await Task.Run(() =>
            {
                try
                {
                    string formula = task.GetFormula();
                    string parametersString = task.GetParametersString();

                    string content = $"Формула задачи: {formula}\n\nПараметры:\n";
                    content += parametersString.Replace(", ", "\n");

                    File.WriteAllText(filePath, content);

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Формула и параметры успешно экспортированы в файл: {filePath}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show($"Ошибка при экспорте: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    });
                }
            });
        }
    }
}
