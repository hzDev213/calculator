using SimpleCalculatorMVVM.Commands.MainViewCommands;
using SimpleCalculatorMVVM.Factories.ButtonFactories;
using SimpleCalculatorMVVM.Models.Buttons;
using SimpleCalculatorMVVM.Services.Calculators;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SimpleCalculatorMVVM.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private ICalculator _calculator;
        private ObservableCollection<Button> _buttons;

        private string _displayText = "0";
        private string _historyText = "";

        #region Properties
        public ObservableCollection<Button> Buttons
        {
            get => _buttons;
            set => Set(ref _buttons, value);
        }
        public string DisplayText
        {
            get => _displayText;
            set => Set(ref _displayText, value);
        }
        public string HistoryText
        {
            get => _historyText;
            set => Set(ref _historyText, value);
        }

        #endregion

        #region Commands
        public ICommand DigitButtonClickCommand { get; }
        public ICommand OperatorButtonClickCommand { get; }
        public ICommand ClearButtonClickCommand { get; }
        public ICommand DeleteLastSymbolButtonClickCommand { get; }
        public ICommand EqualsButtonClickCommand { get; }
        public ICommand PointButtonClickCommand { get; }
        #endregion

        public MainWindowViewModel()
        {
            #region Commands
            DigitButtonClickCommand = new DigitButtonClickCommand(
                            GetText: () => DisplayText,
                            DisplayText: (value) => DisplayText = value
                        );

            OperatorButtonClickCommand = new OperatorButtonClickCommand(
                SetText: (value) => DisplayText = value,
                GetText: () => DisplayText,
                SetHistory: (value) => HistoryText = value
            );

            ClearButtonClickCommand = new ClearButtonClickCommand(
                SetResult: (value) => DisplayText = value,
                SetHistory: (value) => HistoryText = value
            );

            DeleteLastSymbolButtonClickCommand = new DeleteLastSymbolButtonClickCommand(
                GetResult: () => DisplayText,
                SetResult: (value) => DisplayText = value
            );

            EqualsButtonClickCommand = new EqualsButtonClickCommand(
                GetResult: () => DisplayText,
                SetResult: (value) => DisplayText = value,
                GetHistory: () => HistoryText,
                SetHistory: (value) => HistoryText = value
            );

            PointButtonClickCommand = new PointButtonClickCommand(
                GetResult: () => DisplayText,
                SetResult: (value) => DisplayText = value
            );

            #endregion

            _calculator = new CalculatorEngine();
            _buttons = new ObservableCollection<Button>();

            CreateButtons();
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

                Buttons.Add(UIButton);

                ApplyStyle(UIButton);

                // Устанавливаем кнопки на грид
                if (button is DigitButton)
                {
                    UIButton.Command = DigitButtonClickCommand;
                    UIButton.CommandParameter = button.OnClick();

                    Grid.SetRow(UIButton, int.Parse(button.OnClick()) / 3 + 1);
                    Grid.SetColumn(UIButton, int.Parse(button.OnClick()) % 3);
                }
                else if (button is OperatorButton)
                {
                    UIButton.Command = OperatorButtonClickCommand;
                    UIButton.CommandParameter = button.OnClick();

                    Grid.SetColumn(UIButton, 3);
                    Grid.SetRow(UIButton, operatorRow);

                    ++operatorRow;
                }
                else if (button is EqualsButton)
                {
                    UIButton.Command = EqualsButtonClickCommand;

                    Grid.SetRow(UIButton, 4);
                    Grid.SetColumn(UIButton, 2);
                }
                else if (button is ClearButton)
                {
                    UIButton.Command = ClearButtonClickCommand;

                    Grid.SetRow(UIButton, 0);
                    Grid.SetColumn(UIButton, 0);
                }
                else if (button is PointButton)
                {
                    UIButton.Command = PointButtonClickCommand;

                    Grid.SetRow(UIButton, 4);
                    Grid.SetColumn(UIButton, 1);
                }
                else if (button is DeleteLastSymbolButton)
                {
                    UIButton.Command = DeleteLastSymbolButtonClickCommand;

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
    }
}