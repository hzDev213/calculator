using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleCalculator
{
    public partial class MainWindow : Window
    {
        private SimpleCalculator.CalculatorEngine _engine;
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

        
        private void Window_KeyDown(object sender, KeyEventArgs e)
{
    // Цифры (основная клавиатура и NumPad)
    if ((e.Key >= Key.D0 && e.Key <= Key.D9)) 
        AddSymbolFromKey((e.Key - Key.D0).ToString());
    else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
        AddSymbolFromKey((e.Key - Key.NumPad0).ToString());

    // Операторы
    else if (e.Key == Key.Add || (e.Key == Key.OemPlus && Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))) 
        AddSymbolFromKey("+");
    else if (e.Key == Key.Subtract || e.Key == Key.OemMinus) 
        AddSymbolFromKey("-");
    else if (e.Key == Key.Multiply) 
        AddSymbolFromKey("*");
    else if (e.Key == Key.Divide || e.Key == Key.OemQuestion) 
        AddSymbolFromKey("/");

    else if (e.Key == Key.D9) 
        AddSymbolFromKey("(");
    else if (e.Key == Key.D0) 
        AddSymbolFromKey(")");

    // Служебные клавиши
    else if (e.Key == Key.Back) 
        Button_Backspace_Click(null, null);
    else if (e.Key == Key.Enter) 
        Button_Equals_Click(null, null);
    else if (e.Key == Key.Escape) 
        Button_Clear_Click(null, null);
    else if (e.Key == Key.Decimal || e.Key == Key.OemComma || e.Key == Key.OemPeriod) 
        Button_Decimal_Click(null, null);
}


private void AddSymbolFromKey(string symbol)
{
    if (_expression == "Ошибка") _expression = "";
    _expression += symbol;
    UpdateDisplay();
}
    }
}