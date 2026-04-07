using SimpleCalculatorMVVM.Factories.ButtonFactories;
using SimpleCalculatorMVVM.Models.Buttons;
using SimpleCalculatorMVVM.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SimpleCalculatorMVVM
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        private readonly Dictionary<Key, (ICommand Command, object? Parameter)> _keyMappings;
        public MainWindow()
        {
            InitializeComponent();

            _viewModel = (MainWindowViewModel)DataContext;
            _keyMappings = new Dictionary<Key, (ICommand, object?)>
            {
                [Key.D0] = (_viewModel.DigitButtonClickCommand, "0"),
                [Key.D1] = (_viewModel.DigitButtonClickCommand, "1"),
                [Key.D2] = (_viewModel.DigitButtonClickCommand, "2"),
                [Key.D3] = (_viewModel.DigitButtonClickCommand, "3"),
                [Key.D4] = (_viewModel.DigitButtonClickCommand, "4"),
                [Key.D5] = (_viewModel.DigitButtonClickCommand, "5"),
                [Key.D6] = (_viewModel.DigitButtonClickCommand, "6"),
                [Key.D7] = (_viewModel.DigitButtonClickCommand, "7"),
                [Key.D8] = (_viewModel.DigitButtonClickCommand, "8"),
                [Key.D9] = (_viewModel.DigitButtonClickCommand, "9"),

                [Key.NumPad0] = (_viewModel.DigitButtonClickCommand, "0"),
                [Key.NumPad1] = (_viewModel.DigitButtonClickCommand, "1"),
                [Key.NumPad2] = (_viewModel.DigitButtonClickCommand, "2"),
                [Key.NumPad3] = (_viewModel.DigitButtonClickCommand, "3"),
                [Key.NumPad4] = (_viewModel.DigitButtonClickCommand, "4"),
                [Key.NumPad5] = (_viewModel.DigitButtonClickCommand, "5"),
                [Key.NumPad6] = (_viewModel.DigitButtonClickCommand, "6"),
                [Key.NumPad7] = (_viewModel.DigitButtonClickCommand, "7"),
                [Key.NumPad8] = (_viewModel.DigitButtonClickCommand, "8"),
                [Key.NumPad9] = (_viewModel.DigitButtonClickCommand, "9"),

                [Key.Add] = (_viewModel.OperatorButtonClickCommand, "+"),
                [Key.OemPlus] = (_viewModel.OperatorButtonClickCommand, "+"),
                [Key.Subtract] = (_viewModel.OperatorButtonClickCommand, "-"),
                [Key.OemMinus] = (_viewModel.OperatorButtonClickCommand, "-"),
                [Key.Multiply] = (_viewModel.OperatorButtonClickCommand, "*"),
                [Key.Divide] = (_viewModel.OperatorButtonClickCommand, "/"),
                [Key.OemQuestion] = (_viewModel.OperatorButtonClickCommand, "/"),  // / на русской раскладке

                [Key.Decimal] = (_viewModel.PointButtonClickCommand, "."),
                [Key.OemPeriod] = (_viewModel.PointButtonClickCommand, "."),
                [Key.OemComma] = (_viewModel.PointButtonClickCommand, "."),

                [Key.Enter] = (_viewModel.EqualsButtonClickCommand, null),
                [Key.Return] = (_viewModel.EqualsButtonClickCommand, null),
                [Key.Back] = (_viewModel.DeleteLastSymbolButtonClickCommand, null),
                [Key.Delete] = (_viewModel.ClearButtonClickCommand, null),
                [Key.Escape] = (_viewModel.ClearButtonClickCommand, null),
            };

            CreateButtons();
            this.KeyDown += OnKeyDown;
        }
        private void CreateButtons()
        {
            var btnFactory = new CommonButtonFactory();
            var buttonsList = btnFactory.GetAllButtons();

            int operatorRow = 1;

            foreach (var button in buttonsList)
            {
                var UIButton = new Button
                {
                    Content = button.DisplayTitle(),
                    Margin = new Thickness(2),
                    FontSize = 24,
                    FontFamily = new FontFamily("Segoe UI"),
                    Tag = button
                };

                _viewModel.Buttons.Add(UIButton);

                ApplyStyle(UIButton);

                // Устанавливаем кнопки на грид
                if (button is DigitButton)
                {
                    UIButton.Command = _viewModel.DigitButtonClickCommand;
                    UIButton.CommandParameter = button.OnClick();

                    Grid.SetRow(UIButton, int.Parse(button.OnClick()) / 3 + 1);
                    Grid.SetColumn(UIButton, int.Parse(button.OnClick()) % 3);
                }
                else if (button is OperatorButton)
                {
                    UIButton.Command = _viewModel.OperatorButtonClickCommand;
                    UIButton.CommandParameter = button.OnClick();

                    Grid.SetColumn(UIButton, 3);
                    Grid.SetRow(UIButton, operatorRow);

                    ++operatorRow;
                }
                else if (button is EqualsButton)
                {
                    UIButton.Command = _viewModel.EqualsButtonClickCommand;

                    Grid.SetRow(UIButton, 4);
                    Grid.SetColumn(UIButton, 2);
                }
                else if (button is ClearButton)
                {
                    UIButton.Command = _viewModel.ClearButtonClickCommand;

                    Grid.SetRow(UIButton, 0);
                    Grid.SetColumn(UIButton, 0);
                }
                else if (button is PointButton)
                {
                    UIButton.Command = _viewModel.PointButtonClickCommand;

                    Grid.SetRow(UIButton, 4);
                    Grid.SetColumn(UIButton, 1);
                }
                else if (button is DeleteLastSymbolButton)
                {
                    UIButton.Command = _viewModel.DeleteLastSymbolButtonClickCommand;

                    Grid.SetRow(UIButton, 0);
                    Grid.SetColumn(UIButton, 1);
                }
            }
        }
        private void ApplyStyle(Button button)
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

            var template = new ControlTemplate(typeof(Button))
            {
                VisualTree = borderFactory
            };
            // Реакция на клик
            var mouseOverTrigger = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
            mouseOverTrigger.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Color.FromRgb(100, 100, 100))));
            template.Triggers.Add(mouseOverTrigger);

            var pressedTrigger = new Trigger { Property = Button.IsPressedProperty, Value = true };
            pressedTrigger.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Color.FromRgb(150, 150, 150))));
            pressedTrigger.Setters.Add(new Setter(Button.RenderTransformProperty, new ScaleTransform(0.95, 0.95)));
            template.Triggers.Add(pressedTrigger);

            button.Template = template;

            // Стилизация по типу
            var logicButton = button.Tag;
            if (logicButton is DigitButton)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(45, 45, 45));
                button.Foreground = Brushes.White;
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
            }
            else if (logicButton is ClearButton)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
                button.Foreground = new SolidColorBrush(Color.FromRgb(76, 194, 255));
            }
            else if (logicButton is PointButton)
            {
                button.Background = new SolidColorBrush(Color.FromRgb(45, 45, 45));
                button.Foreground = Brushes.White;
            }
            else
            {
                button.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
                button.Foreground = new SolidColorBrush(Color.FromRgb(76, 194, 255));
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            bool isCtrlPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
            bool isAltPressed = Keyboard.Modifiers.HasFlag(ModifierKeys.Alt);

            if (!isCtrlPressed && !isAltPressed)
            {
                if (_keyMappings.TryGetValue(e.Key, out var mapping))
                {
                    mapping.Command.Execute(mapping.Parameter);
                    e.Handled = true;
                }
            }
        }
    }
}