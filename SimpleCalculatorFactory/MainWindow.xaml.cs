using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SimpleCalculatorFactory
{
    public partial class MainWindow : Window
    {
        private CalculatorEngine _engine;
        private string _expression = "";
        private List<IButton>_buttons;

        public MainWindow()
        {
            InitializeComponent();

            _engine = new CalculatorEngine();
            var buttonFactory = new CommonCalculatorFactory();
            _buttons = buttonFactory.GetAllButtons();

            CreateButtons();
        }

        private void CreateButtons()
        {
            int operatorRow = 1;

            foreach (var btn in _buttons)
            {
                var button = new Button
                {
                    Content = btn.OnClick(),
                    Margin = new Thickness(2),
                    FontSize = 24,
                    FontFamily = new FontFamily("Segoe UI"),
                    Tag = btn
                };

                ApplyStyle(button, btn);
                button.Click += Button_Click;
                ButtonsGrid.Children.Add(button);

                if (btn is OperatorButton)
                {
                    Grid.SetColumn(button, 3);
                    Grid.SetRow(button, operatorRow);

                    ++operatorRow;
                }
            }
        }

        private void ApplyStyle(Button button, IButton logicButton)
        {
            button.BorderThickness = new Thickness(0);
            button.Cursor = Cursors.Hand;

            // Шаблон кнопки
            var borderFactory = new FrameworkElementFactory(typeof(Border));
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(8));
            borderFactory.SetBinding(Border.BackgroundProperty, new System.Windows.Data.Binding("Background") { Source = button });

            var contentFactory = new FrameworkElementFactory(typeof(ContentPresenter));
            contentFactory.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentFactory.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);

            borderFactory.AppendChild(contentFactory);

            button.Template = new ControlTemplate(typeof(Button))
            {
                VisualTree = borderFactory
            };

            // Стилизация по типу
            if (logicButton is DigitButton)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(45, 45, 45));
                button.Foreground = Brushes.White;

                Grid.SetRow(button, int.Parse(logicButton.OnClick()) / 3 + 1);
                Grid.SetColumn(button, int.Parse(logicButton.OnClick()) % 3);
            }
            else if (logicButton is OperatorButton)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
                button.Foreground = new SolidColorBrush(Color.FromRgb(76, 194, 255));
            }
            else if (logicButton is EqualsButton)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(76, 194, 255));
                button.Foreground = Brushes.Black;

                Grid.SetRow(button, 4);
                Grid.SetColumn(button, 2);
            }
            else if (logicButton is ClearButton)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
                button.Foreground = new SolidColorBrush(Color.FromRgb(76, 194, 255));

                Grid.SetRow(button, 0);
                Grid.SetColumn(button, 0);
            }
            else if (logicButton is PointButton)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(45, 45, 45));
                button.Foreground = Brushes.White;

                Grid.SetRow(button, 4);
                Grid.SetColumn(button, 1);
            }
            else
            {
                button.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
                button.Foreground = new SolidColorBrush(Color.FromRgb(76, 194, 255));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var logicButton = button?.Tag as IButton;

            if (logicButton != null)
            {
                string value = logicButton.OnClick();
                ProcessInput(value);
            }
        }

        private void ProcessInput(string value)
        {
            if (int.TryParse(value, out _))
            {
                if (_expression == "Ошибка") _expression = "";
                _expression += value;
            }
            else if (value == "+" || value == "-" || value == "*" || value == "/")
            {
                if (string.IsNullOrEmpty(_expression)) return;
                _expression += value;
            }
            else if (value == "=")
            {
                try
                {
                    var result = _engine.Calculate(_expression);
                    _expression = result.ToString();
                }
                catch
                {
                    _expression = "Ошибка";
                }
            }
            else if (value == "C")
            {
                _expression = "";
            }
            else if (value == ".")
            {
                _expression += ".";
            }

            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            DisplayText.Text = string.IsNullOrEmpty(_expression) ? "0" : _expression;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9)
                ProcessInput((e.Key - Key.D0).ToString());
            else if (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9)
                ProcessInput((e.Key - Key.NumPad0).ToString());
            else if (e.Key == Key.Add) ProcessInput("+");
            else if (e.Key == Key.Subtract) ProcessInput("-");
            else if (e.Key == Key.Multiply) ProcessInput("*");
            else if (e.Key == Key.Divide) ProcessInput("/");
            else if (e.Key == Key.Enter) ProcessInput("=");
            else if (e.Key == Key.Back) ProcessInput("C");
            else if (e.Key == Key.Decimal || e.Key == Key.OemComma) ProcessInput(",");
            else if (e.Key == Key.Escape) ProcessInput("C");
        }
    }
}