using System;
using System.Data;

namespace SimpleCalculator
{
    public class CalculatorEngine
    {
        // Теперь метод принимает строку целиком
        public string Calculate(string expression)
        {
            try
            {
                // DataTable использует точку для дробей, поэтому заменяем запятые
                string safeExpression = expression.Replace(",", ".");
                
                // Используем встроенный метод Compute для вычисления всего выражения сразу
                DataTable table = new DataTable();
                var result = table.Compute(safeExpression, string.Empty);
                
                // Проверка на деление на ноль (которое возвращает бесконечность)
                double numericResult = Convert.ToDouble(result);
                if (double.IsInfinity(numericResult) || double.IsNaN(numericResult))
                {
                    return "Ошибка";
                }

                // Возвращаем результат обратно в виде строки
                return numericResult.ToString();
            }
            catch (Exception)
            {
                return "Ошибка";
            }
        }
    }
}