using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleCalculator
{
    public partial class MainWindow : Window
    {
        private CalculatorEngine _engine;
        private string _expression = "";

        public MainWindow()
        {
            InitializeComponent();
            _engine = new CalculatorEngine();
        }

        // МЕТОД 1: Для обычных цифр, скобок и Пи
        private void Button_Number_Click(object sender, RoutedEventArgs e)
        {
            string val = ((Button)sender).Content.ToString();
            if (_expression == "Ошибка") _expression = "";
            _expression += val;
            UpdateDisplay();
        }

        // МЕТОД 2: Для функций sin, cos, sqrt
        private void Button_MathFunction_Click(object sender, RoutedEventArgs e)
        {
            string func = ((Button)sender).Content.ToString();
            if (_expression == "Ошибка") _expression = "";
            
            // Добавляем функцию и сразу открываем скобку
            _expression += func + "(";
            UpdateDisplay();
        }

        // МЕТОД 3: Для операторов +, -, *, /
        private void Button_Operator_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_expression) || _expression == "Ошибка") return;
            string op = ((Button)sender).Content.ToString();
            _expression += op;
            UpdateDisplay();
        }

        // МЕТОД 4: Равно (=)
        private void Button_Equals_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_expression)) return;

            HistoryText.Text = _expression + " =";
            string result = _engine.Calculate(_expression);
            _expression = result;
            UpdateDisplay();
        }

        // МЕТОД 5: Стирание последнего символа (Backspace)
        private void Button_Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_expression) && _expression != "Ошибка")
            {
                // Если стираем функцию (напр. "sin("), можно усложнить, 
                // но для базы просто стираем последний символ.
                _expression = _expression.Remove(_expression.Length - 1);
            }
            else { _expression = ""; }
            UpdateDisplay();
        }

        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            _expression = "";
            HistoryText.Text = "";
            UpdateDisplay();
        }

        private void Button_Decimal_Click(object sender, RoutedEventArgs e)
        {
            _expression += ",";
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            DisplayText.Text = string.IsNullOrEmpty(_expression) ? "0" : _expression;
        }

        // Глобальная обработка клавиш
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back) Button_Backspace_Click(null, null);
            else if (e.Key == Key.Enter) Button_Equals_Click(null, null);
            else if (e.Key == Key.Escape) Button_Clear_Click(null, null);
            // Остальные клавиши можно добавить по аналогии
        }
    }
}