using Microsoft.Win32;
using MO_kursasch_25.Enums;

namespace MO_kursasch_25.Service
{
    public static class FileProvider
    {

        public static string GetFilePath(FileMode mode, string filter, string defaultFileName = "", string title = "Выберите файл")
        {
            if (string.IsNullOrEmpty(filter))
                throw new ArgumentException("Параметр filter не может быть пустым.");

            switch (mode)
            {
                case FileMode.Open:
                    return OpenFileDialog(filter, title);
                case FileMode.Save:
                    return SaveFileDialog(filter, defaultFileName, title);
                default:
                    throw new ArgumentException("Недопустимый режим. Используйте FileMode.Open или FileMode.Save.");
            }
        }


        private static string OpenFileDialog(string filter, string title)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = filter,
                Title = title,
                Multiselect = false // Разрешаем выбор только одного файла
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }


        private static string SaveFileDialog(string filter, string defaultFileName, string title)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = filter,
                Title = title,
                FileName = defaultFileName,
                DefaultExt = GetDefaultExtension(filter),
                OverwritePrompt = true
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }
            return null;
        }

        private static string GetDefaultExtension(string filter)
        {
            string[] parts = filter.Split('|');
            if (parts.Length > 1)
            {
                string firstExtension = parts[1].Split(';')[0].Replace("*", "").Trim();
                return firstExtension;
            }
            return "";
        }
    }
}
