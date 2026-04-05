using SimpleCalculatorMVVM.Commands.MainViewCommands;
using SimpleCalculatorMVVM.Factories.ButtonFactories;
using SimpleCalculatorMVVM.Models.Buttons;
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
        }
    }
}