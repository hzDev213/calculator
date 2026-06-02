using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Calculator.Evaluator
{
    public class CalculatorEngine : ICalculator
    {
        public string Calculate(string expression)
        {
            try
            {
                if (string.IsNullOrEmpty(expression)) return "0";

                string p = expression.Replace(",", ".");

                p = Regex.Replace(p, @"sqrt\(([^)]+)\)", m =>
                {
                    double val = ExtractValue(m.Groups[1].Value);
                    return Math.Sqrt(val).ToString(CultureInfo.InvariantCulture);
                });

                p = Regex.Replace(p, @"sin\(([^)]+)\)", m =>
                {
                    double deg = ExtractValue(m.Groups[1].Value);
                    return Math.Sin(deg * Math.PI / 180.0)
                               .ToString(CultureInfo.InvariantCulture);
                });

                p = Regex.Replace(p, @"cos\(([^)]+)\)", m =>
                {
                    double deg = ExtractValue(m.Groups[1].Value);
                    return Math.Cos(deg * Math.PI / 180.0)
                               .ToString(CultureInfo.InvariantCulture);
                });

                DataTable table = new DataTable();
                var result = table.Compute(p, string.Empty);
                double final = Convert.ToDouble(result);
                return Math.Round(final, 10).ToString();
            }
            catch { return "Ошибка"; }
        }

        private double ExtractValue(string input)
        {
            if (double.TryParse(input, NumberStyles.Any,
                CultureInfo.InvariantCulture, out double res))
                return res;
            return double.Parse(Calculate(input));
        }
    }
}