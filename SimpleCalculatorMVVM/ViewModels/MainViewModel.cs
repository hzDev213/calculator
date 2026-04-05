using SimpleCalculatorMVVM.Commands.MainViewCommands;
using SimpleCalculatorMVVM.Services.Calculators;
using System.Collections.ObjectModel;
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

        #region DigitButtonClickCommand
        public ICommand DigitButtonClickCommand { get; }

        private bool CanDigitButtonClickCommandExecute(object? p) => true;
        private void OnDigitButtonClickCommandExecuted(object? p)
        {
            string? digit = p?.ToString();
            if (digit != null)
            {
                DisplayText = DisplayText == "0" ? digit : DisplayText + digit;
            }
        }
        #endregion

        #region OperatorButtonClickCommand
        public ICommand OperatorButtonClickCommand { get; }

        private bool CanOperatorButtonClickCommandExecute(object? p) => true;
        private void OnOperatorButtonClickCommandExecuted(object? p)
        {
            string[] operators = { "+", "-", "*", "/" };

            string? operator_ = p?.ToString();
            if (operator_ != null)
            {
                if (!operators.Any(op => DisplayText.Contains(op)))
                {
                    DisplayText = DisplayText == "0" ? "0" : DisplayText + operator_;
                }
                else
                {
                    HistoryText = DisplayText;
                    DisplayText = _calculator.Calculate(DisplayText) + operator_;
                }
            }
        }
        #endregion

        #region ClearButtonClickCommand
        public ICommand ClearButtonClickCommand { get; }

        private bool CanClearButtonClickCommandExecute(object? p) => true;
        private void OnClearButtonClickCommandExecuted(object? p)
        {
            DisplayText = "0";
            HistoryText = "";
        }
        #endregion

        #region DeleteLastSymbolButtonClickCommand
        public ICommand DeleteLastSymbolButtonClickCommand { get; }

        private bool CanDeleteLastSymbolButtonClickCommandExecute(object? p) => true;
        private void OnDeleteLastSymbolButtonClickCommandExecuted(object? p)
        {

            if (DisplayText.Length == 1)
            {
                DisplayText = "0";
            }
            else
            {
                DisplayText = DisplayText.Substring(0, DisplayText.Length - 1);
            }
        }
        #endregion

        #region EqualsButtonClickCommand
        public ICommand EqualsButtonClickCommand { get; }

        private bool CanEqualsButtonClickCommandExecute(object? p) => true;
        private void OnEqualsButtonClickCommandExecuted(object? p)
        {
            HistoryText = DisplayText;
            DisplayText = _calculator.Calculate(HistoryText);
        }
        #endregion

        #region PointButtonClickCommand
        public ICommand PointButtonClickCommand { get; }

        private bool CanPointButtonClickCommandExecute(object? p) => true;
        private void OnPointButtonClickCommandExecuted(object? p)
        {
            if (char.IsDigit(DisplayText.Last()))
            {
                DisplayText += ".";
            }
        }
        #endregion

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
        }
    }
}