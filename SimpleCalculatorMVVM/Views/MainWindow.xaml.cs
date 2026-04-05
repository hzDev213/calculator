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
        private Grid _buttonGrid;
        public MainWindow()
        {
            InitializeComponent();

            _viewModel = (MainWindowViewModel)DataContext;
            _buttonGrid = (Grid)FindName("ButtonsContainer");

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
    }
}