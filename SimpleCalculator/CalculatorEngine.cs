using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SimpleCalculator
{
    public class CalculatorEngine
    {
        public string Calculate(string expression)
        {
            try
            {
                if (string.IsNullOrEmpty(expression)) return "0";

                // 1. Подготовка: заменяем запятые на точки для стандарта вычислений
                // и заменяем символ Пи на его числовое значение
                string processedExpr = expression.Replace(",", ".");
                processedExpr = processedExpr.Replace("π", Math.PI.ToString(CultureInfo.InvariantCulture));

                // 2. Обработка КОРНЯ: sqrt(x)
                processedExpr = Regex.Replace(processedExpr, @"sqrt\(([^)]+)\)", m => {
                    double val = ExtractValue(m.Groups[1].Value);
                    return Math.Sqrt(val).ToString(CultureInfo.InvariantCulture);
                });

                // 3. Обработка SIN: sin(x) - ожидаем градусы
                processedExpr = Regex.Replace(processedExpr, @"sin\(([^)]+)\)", m => {
                    double degrees = ExtractValue(m.Groups[1].Value);
                    double radians = degrees * (Math.PI / 180.0);
                    return Math.Sin(radians).ToString(CultureInfo.InvariantCulture);
                });

                // 4. Обработка COS: cos(x)
                processedExpr = Regex.Replace(processedExpr, @"cos\(([^)]+)\)", m => {
                    double degrees = ExtractValue(m.Groups[1].Value);
                    double radians = degrees * (Math.PI / 180.0);
                    return Math.Cos(radians).ToString(CultureInfo.InvariantCulture);
                });

                // 5. Финальный расчет всей строки через DataTable
                DataTable table = new DataTable();
                var result = table.Compute(processedExpr, string.Empty);

                double finalResult = Convert.ToDouble(result);

                // Округляем до 10 знаков, чтобы избежать мусора типа 0.0000000000000001
                return Math.Round(finalResult, 10).ToString();
            }
            catch (Exception)
            {
                return "Ошибка";
            }
        }

        // Вспомогательный метод для извлечения числа из скобок
        private double ExtractValue(string input)
        {
            if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double res))
                return res;
            // Если внутри функции другое выражение (напр. sin(30+60)), считаем его рекурсивно
            return double.Parse(Calculate(input));
        }
    }
}