using System.IO;
using System.Text.Json;
using System.Windows;

namespace SimpleCalculatorMVVMIniFile.Services
{
    public class WindowConfig
    {
        public double Width { get; set; } = 320;
        public double Height { get; set; } = 500;
        public double MinWidth { get; set; } = 250;
        public double MinHeight { get; set; } = 400;
        public string Title { get; set; } = "Calculator";
    }

    public class FontsConfig
    {
        public string ButtonFontFamily { get; set; } = "Segoe UI";
        public double ButtonFontSize { get; set; } = 24;
        public double DisplayFontSize { get; set; } = 48;
        public double HistoryFontSize { get; set; } = 16;
        public double MenuFontSize { get; set; } = 14;
    }

    public class ThemeEntry
    {
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        public Dictionary<string, string> Colors { get; set; } = new();
    }

    public class AppConfig
    {
        public string DefaultTheme { get; set; } = "dark";
        public WindowConfig Window { get; set; } = new();
        public FontsConfig Fonts { get; set; } = new();
        public Dictionary<string, ThemeEntry> Themes { get; set; } = new();
    }

    public static class AppConfigLoader
    {
        private const string ConfigFileName = "calculator_config.json";

        /// <summary>
        /// Ищет конфиг рядом с exe, затем в папке проекта (/../../../).
        /// Возвращает AppConfig; при любой ошибке показывает MessageBox и возвращает null.
        /// </summary>
        public static AppConfig? Load()
        {
            string[] candidates =
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../", ConfigFileName),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../Resources/Configs", ConfigFileName)
            };

            string? foundPath = null;
            foreach (var c in candidates)
            {
                if (File.Exists(c)) { foundPath = Path.GetFullPath(c); break; }
            }

            if (foundPath is null)
            {
                ShowError(
                    $"Конфигурационный файл «{ConfigFileName}» не найден.\n\n" +
                    $"Ожидаемые пути:\n• {candidates[0]}\n• {candidates[1]}\n\n" +
                    "Будут применены встроенные настройки по умолчанию.");
                return null;
            }

            string json;
            try
            {
                json = File.ReadAllText(foundPath);
            }
            catch (Exception ex)
            {
                ShowError($"Не удалось прочитать файл конфигурации:\n{ex.Message}\n\n" +
                          "Будут применены встроенные настройки по умолчанию.");
                return null;
            }

            // Парсинг JSON
            AppConfig? config;
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };
                config = JsonSerializer.Deserialize<AppConfig>(json, options);
            }
            catch (JsonException ex)
            {
                ShowError($"Ошибка формата конфигурационного файла:\n{ex.Message}\n\n" +
                          "Проверьте правильность JSON-синтаксиса.\n" +
                          "Будут применены встроенные настройки по умолчанию.");
                return null;
            }

            if (config is null)
            {
                ShowError("Конфигурационный файл пуст или не содержит корректных данных.\n" +
                          "Будут применены встроенные настройки по умолчанию.");
                return null;
            }

            // Проверка обязательных полей
            var errors = new List<string>();

            if (config.Themes is null || config.Themes.Count == 0)
                errors.Add("Отсутствует раздел «themes».");

            if (!string.IsNullOrEmpty(config.DefaultTheme) &&
                config.Themes is not null &&
                !config.Themes.ContainsKey(config.DefaultTheme))
            {
                errors.Add($"Тема по умолчанию «{config.DefaultTheme}» не найдена в разделе «themes».");
            }

            if (errors.Count > 0)
            {
                ShowError("Конфигурационный файл содержит ошибки:\n• " +
                          string.Join("\n• ", errors) +
                          "\n\nБудут применены встроенные настройки по умолчанию.");
                return null;
            }

            return config;
        }

        private static void ShowError(string message) =>
            MessageBox.Show(message, "Ошибка конфигурации",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}