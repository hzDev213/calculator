using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SimpleCalculator
{
    public partial class MainWindow : Window
    {
        private CalculatorEngine _engine;
        private string _expression = ""; // Хранит всю строку целиком: "2+3*4"

        public MainWindow()
        {
            InitializeComponent();
            _engine = new CalculatorEngine();
            UpdateDisplay();
        }

        // Ввод цифр
        private void Button_Number_Click(object sender, RoutedEventArgs e)
        {
            string number = ((Button)sender).Content.ToString();
            
            // Если на экране ошибка, сбрасываем перед новым вводом
            if (_expression == "Ошибка") _expression = "";
            
            _expression += number;
            UpdateDisplay();
        }

        // Ввод операторов
        private void Button_Operator_Click(object sender, RoutedEventArgs e)
        {
            if (_expression == "Ошибка" || string.IsNullOrEmpty(_expression)) return;

            string op = ((Button)sender).Content.ToString();

            // Защита от ввода двух операторов подряд (заменяем последний)
            if ("+-*/".Contains(_expression[_expression.Length - 1].ToString()))
            {
                _expression = _expression.Substring(0, _expression.Length - 1) + op;
            }
            else
            {
                _expression += op;
            }
            UpdateDisplay();
        }

        // Кнопка "="
        private void Button_Equals_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_expression) || _expression == "Ошибка") return;

            // Записываем выражение в историю перед вычислением
            HistoryText.Text = _expression + " =";
            
            // Вычисляем
            string result = _engine.Calculate(_expression);
            _expression = result; // Результат становится новым началом
            
            UpdateDisplay();
        }

        // Сброс (C)
        private void Button_Clear_Click(object sender, RoutedEventArgs e)
        {
            _expression = "";
            HistoryText.Text = "";
            UpdateDisplay();
        }

        //! Обработчик клика по кнопке ⌫
        private void Button_Backspace_Click(object sender, RoutedEventArgs e)
        {
            EraseLastSymbol();
        }

        // Универсальный метод удаления последнего символа
        private void EraseLastSymbol()
        {
            // Если на экране ошибка или строка пуста — ничего не делаем
            if (string.IsNullOrEmpty(_expression) || _expression == "Ошибка")
            {
                _expression = "";
                UpdateDisplay();
                return;
            }

            // Удаляем последний символ
            _expression = _expression.Remove(_expression.Length - 1);
            
            UpdateDisplay();
        }


        // Запятая
        private void Button_Decimal_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_expression) || _expression == "Ошибка") return;
            
            // Простая проверка, чтобы не поставить запятую после оператора
            char lastChar = _expression[_expression.Length - 1];
            if (!"+-*/,".Contains(lastChar.ToString()))
            {
                _expression += ",";
                UpdateDisplay();
            }
        }

        // Отрицательные числа (для простоты оборачиваем в скобки: "(-5)")
        private void Button_Negate_Click(object sender, RoutedEventArgs e)
        {
             // В сложной строке менять знак сложнее. Как простой вариант - добавляем минус в начало всего выражения или оборачиваем.
             // Для базовой версии:
             if (!string.IsNullOrEmpty(_expression) && _expression != "Ошибка")
             {
                 if (_expression.StartsWith("-"))
                     _expression = _expression.Substring(1);
                 else
                     _expression = "-" + _expression;
                 UpdateDisplay();
             }
        }

        // Обновление экрана
        private void UpdateDisplay()
        {
            DisplayText.Text = string.IsNullOrEmpty(_expression) ? "0" : _expression;
        }

        // Клавиатура
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9) 
                AppendFromKeyboard((e.Key - Key.D0).ToString());
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) 
                AppendFromKeyboard((e.Key - Key.NumPad0).ToString());
            
            else if (e.Key == Key.Add || e.Key == Key.OemPlus) AppendOperator("+");
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus) AppendOperator("-");
            else if (e.Key == Key.Multiply) AppendOperator("*");
            else if (e.Key == Key.Divide || e.Key == Key.OemQuestion) AppendOperator("/");
            
            else if (e.Key == Key.Enter) Button_Equals_Click(null, null);
            else if (e.Key == Key.Escape) Button_Clear_Click(null, null);
            else if (e.Key == Key.Decimal || e.Key == Key.OemComma) Button_Decimal_Click(null, null);
        }

        private void AppendFromKeyboard(string val)
        {
            if (_expression == "Ошибка") _expression = "";
            _expression += val;
            UpdateDisplay();
        }

        private void AppendOperator(string op)
        {
            Button tempBtn = new Button { Content = op };
            Button_Operator_Click(tempBtn, null);
        }
    }
}