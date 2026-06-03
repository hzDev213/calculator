using System;
using System.Windows.Input;
using Calculator.Commands;
using Calculator.Models;
using Calculator.Patterns.Observer;

namespace Calculator.ViewModels
{
    /// <summary>
    /// CalculatorViewModel — связывает View и Model.
    /// Реализует ICalculatorObserver для получения уведомлений от модели.
    /// </summary>
    public class CalculatorViewModel : ViewModelBase, ICalculatorObserver
    {
        private readonly CalculatorModel _model;

        // ── Поля состояния ──────────────────────────

        private string _displayValue  = "0";
        private string _expression    = string.Empty;
        private string _statusMessage = "Готов";
        private string _selectedOperator = "+";
        private bool   _hasError;

        private double _operandA;
        private bool   _waitingForOperandB;

        // ── Публичные свойства (для XAML Binding) ──

        public string DisplayValue
        {
            get => _displayValue;
            set => SetProperty(ref _displayValue, value);
        }

        public string Expression
        {
            get => _expression;
            set => SetProperty(ref _expression, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public string SelectedOperator
        {
            get => _selectedOperator;
            set => SetProperty(ref _selectedOperator, value);
        }

        public bool HasError
        {
            get => _hasError;
            set => SetProperty(ref _hasError, value);
        }

        public bool CanUndo => _model.CanUndo;
        public bool CanRedo => _model.CanRedo;

        // ── WPF-команды ─────────────────────────────

        public ICommand DigitCommand       { get; }
        public ICommand OperatorCommand    { get; }
        public ICommand EqualsCommand      { get; }
        public ICommand ClearCommand       { get; }
        public ICommand BackspaceCommand   { get; }
        public ICommand UndoCommand        { get; }
        public ICommand RedoCommand        { get; }
        public ICommand DecimalCommand     { get; }
        public ICommand ToggleSignCommand  { get; }

        // ── Конструктор ─────────────────────────────

        public CalculatorViewModel()
        {
            _model = new CalculatorModel();
            _model.Attach(this);   // подписываемся как Observer

            DigitCommand      = new RelayCommand(OnDigit);
            OperatorCommand   = new RelayCommand(OnOperator);
            EqualsCommand     = new RelayCommand(_ => OnEquals(), _ => !_hasError);
            ClearCommand      = new RelayCommand(_ => OnClear());
            BackspaceCommand  = new RelayCommand(_ => OnBackspace(), _ => DisplayValue.Length > 0);
            UndoCommand       = new RelayCommand(_ => OnUndo(), _ => CanUndo);
            RedoCommand       = new RelayCommand(_ => OnRedo(), _ => CanRedo);
            DecimalCommand    = new RelayCommand(_ => OnDecimal());
            ToggleSignCommand = new RelayCommand(_ => OnToggleSign());
        }

        // ── Обработчики ──────────────────────────────

        private void OnDigit(object? param)
        {
            if (_hasError) OnClear();
            string digit = param?.ToString() ?? string.Empty;
            if (_waitingForOperandB)
            {
                DisplayValue = digit;
                _waitingForOperandB = false;
            }
            else
            {
                DisplayValue = (DisplayValue == "0") ? digit : DisplayValue + digit;
            }
        }

        private void OnOperator(object? param)
        {
            SelectedOperator = param?.ToString() ?? "+";
            if (double.TryParse(DisplayValue, out double a))
            {
                _operandA = a;
                Expression = $"{a} {SelectedOperator}";
                _waitingForOperandB = true;
            }
        }

        private void OnEquals()
        {
            if (!double.TryParse(DisplayValue, out double b)) return;

            try
            {
                double result = _model.Calculate(SelectedOperator, _operandA, b);
                if (!double.IsNaN(result))
                {
                    DisplayValue = FormatResult(result);
                    _operandA = result;
                    _waitingForOperandB = true;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                HasError = true;
            }

            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        private void OnClear()
        {
            DisplayValue = "0";
            Expression   = string.Empty;
            StatusMessage = "Готов";
            HasError = false;
            _operandA = 0;
            _waitingForOperandB = false;
            _model.Reset();
            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        private void OnBackspace()
        {
            if (DisplayValue.Length <= 1) { DisplayValue = "0"; return; }
            DisplayValue = DisplayValue[..^1];
        }

        private void OnUndo()
        {
            try
            {
                double restored = _model.Undo();
                DisplayValue = FormatResult(restored);
                StatusMessage = "Отменено";
            }
            catch (Exception ex) { StatusMessage = ex.Message; }

            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        private void OnRedo()
        {
            try
            {
                double redone = _model.Redo();
                DisplayValue = FormatResult(redone);
                StatusMessage = "Повторено";
            }
            catch (Exception ex) { StatusMessage = ex.Message; }

            OnPropertyChanged(nameof(CanUndo));
            OnPropertyChanged(nameof(CanRedo));
        }

        private void OnDecimal()
        {
            if (!DisplayValue.Contains('.'))
                DisplayValue += ".";
        }

        private void OnToggleSign()
        {
            if (double.TryParse(DisplayValue, out double v))
                DisplayValue = FormatResult(-v);
        }

        private static string FormatResult(double v) =>
            v == Math.Truncate(v) ? ((long)v).ToString() : v.ToString("G10");

        // ── ICalculatorObserver (уведомления от Model) ──

        public void OnCalculationPerformed(double result, string expression)
        {
            Expression = expression + " =";
            StatusMessage = "Готов";
            HasError = false;
        }

        public void OnError(string errorMessage)
        {
            StatusMessage = errorMessage;
            DisplayValue  = "Ошибка";
            HasError = true;
        }

        public void OnHistoryCleared() => StatusMessage = "История очищена";
    }
}