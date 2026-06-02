using Calculator.Evaluator;
using Calculator.DevInfo;
using SimpleCalculatorLibs.Commands;
using SimpleCalculatorLibs.Services;
using System.Windows.Input;
using System;
using System.Linq;
namespace SimpleCalculatorLibs.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ICalculator         _calculator;
        private readonly DynamicMemoryLoader _memory;

        private string _displayText  = "0";
        private string _historyText  = "";
        private string _memoryStatus = "";
        private string _devInfo      = "";

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
        public string MemoryStatus
        {
            get => _memoryStatus;
            set => Set(ref _memoryStatus, value);
        }
        public string DevInfo
        {
            get => _devInfo;
            set => Set(ref _devInfo, value);
        }

        // ── Команды ──────────────────────────────

        public ICommand DigitCommand    { get; }
        public ICommand OperatorCommand { get; }
        public ICommand EqualsCommand   { get; }
        public ICommand ClearCommand    { get; }
        public ICommand BackspaceCommand{ get; }
        public ICommand DecimalCommand  { get; }
        public ICommand PiCommand       { get; }

        // Команды памяти
        public ICommand MemoryStoreCommand    { get; }
        public ICommand MemoryAddCommand      { get; }
        public ICommand MemoryRecallCommand   { get; }
        public ICommand MemoryClearCommand    { get; }

        // ── Конструктор ───────────────────────────

        public MainViewModel()
        {
            // Статически подключённые библиотеки
            _calculator = new EnginePiDecorator(new CalculatorEngine());
            DevInfo     = DeveloperInfo.GetFullInfo();

            // Динамически загружаемая библиотека памяти
            _memory = new DynamicMemoryLoader();
            MemoryStatus = _memory.Load()
                ? "Память: загружена"
                : "Память: недоступна";

            // Цифры
            DigitCommand = new RelayCommand(p =>
            {
                string d = p?.ToString() ?? "";
                DisplayText = DisplayText == "0" ? d : DisplayText + d;
            });

            // Операторы
            string[] ops = { "+", "-", "*", "/" };
            OperatorCommand = new RelayCommand(p =>
            {
                string op = p?.ToString() ?? "";
                if (!ops.Any(o => DisplayText.Contains(o)))
                    DisplayText = DisplayText == "0" ? "0" : DisplayText + op;
                else
                {
                    HistoryText = DisplayText;
                    DisplayText = _calculator.Calculate(DisplayText) + op;
                }
            });

            // Равно
            EqualsCommand = new RelayCommand(_ =>
            {
                HistoryText = DisplayText;
                DisplayText = _calculator.Calculate(HistoryText);
            });

            // Очистка
            ClearCommand = new RelayCommand(_ =>
            {
                DisplayText = "0";
                HistoryText = "";
            });

            // Backspace
            BackspaceCommand = new RelayCommand(_ =>
            {
                DisplayText = DisplayText.Length <= 1
                    ? "0"
                    : DisplayText[..^1];
            });

            // Точка
            DecimalCommand = new RelayCommand(_ =>
            {
                if (char.IsDigit(DisplayText.Last()))
                    DisplayText += ".";
            });

            // Константа π
            PiCommand = new RelayCommand(_ =>
            {
                if (DisplayText == "0") DisplayText = "π";
                else if (ops.Any(o => DisplayText.Last().ToString() == o))
                    DisplayText += "π";
            });

            // Память — работают только если dll загружена
            MemoryStoreCommand = new RelayCommand(_ =>
            {
                if (_memory.IsLoaded && double.TryParse(DisplayText, out double v))
                {
                    _memory.Store(v);
                    MemoryStatus = $"M = {v}";
                }
            }, _ => _memory.IsLoaded);

            MemoryAddCommand = new RelayCommand(_ =>
            {
                if (_memory.IsLoaded && double.TryParse(DisplayText, out double v))
                {
                    _memory.Add(v);
                    MemoryStatus = $"M = {_memory.Recall()}";
                }
            }, _ => _memory.IsLoaded);

            MemoryRecallCommand = new RelayCommand(_ =>
            {
                if (_memory.IsLoaded)
                    DisplayText = _memory.Recall().ToString();
            }, _ => _memory.IsLoaded);

            MemoryClearCommand = new RelayCommand(_ =>
            {
                if (_memory.IsLoaded)
                {
                    _memory.Clear();
                    MemoryStatus = "Память очищена";
                }
            }, _ => _memory.IsLoaded);
        }
    }
}