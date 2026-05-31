using System;
using System.Collections.Generic;
using Calculator.Patterns.Command;
using Calculator.Patterns.Observer;

namespace Calculator.Models
{
    /// <summary>
    /// Модель (Model в MVVM) — чистая бизнес-логика.
    /// Использует CommandInvoker для Undo/Redo
    /// и реализует ICalculatorSubject для Observer.
    /// </summary>
    public class CalculatorModel : ICalculatorSubject
    {
        private readonly CommandInvoker _invoker = new CommandInvoker();
        private readonly List<ICalculatorObserver> _observers = new List<ICalculatorObserver>();

        // Регистр доступных команд
        private readonly Dictionary<string, ICalculatorCommand> _commands = new()
        {
            ["+"] = new AddCommand(),
            ["-"] = new SubtractCommand(),
            ["*"] = new MultiplyCommand(),
            ["/"] = new DivideCommand(),
        };

        public bool CanUndo => _invoker.CanUndo;
        public bool CanRedo => _invoker.CanRedo;

        // ── ICalculatorSubject ──────────────────────

        public void Attach(ICalculatorObserver observer) => _observers.Add(observer);
        public void Detach(ICalculatorObserver observer) => _observers.Remove(observer);

        private void NotifyResult(double result, string expression)
        {
            foreach (var o in _observers)
                o.OnCalculationPerformed(result, expression);
        }

        private void NotifyError(string msg)
        {
            foreach (var o in _observers)
                o.OnError(msg);
        }

        // ── Основные операции ───────────────────────

        public double Calculate(string operatorSymbol, double a, double b)
        {
            if (!_commands.TryGetValue(operatorSymbol, out var cmd))
                throw new ArgumentException($"Неизвестный оператор: {operatorSymbol}");

            double result = _invoker.ExecuteCommand(cmd, a, b);

            if (double.IsNaN(result))
            {
                NotifyError("Ошибка: деление на ноль");
                return double.NaN;
            }

            NotifyResult(result, $"{a} {cmd.OperationSymbol} {b}");
            return result;
        }

        public double Undo()
        {
            if (!CanUndo) throw new InvalidOperationException("Нечего отменять");
            return _invoker.Undo();
        }

        public double Redo()
        {
            if (!CanRedo) throw new InvalidOperationException("Нечего повторять");
            return _invoker.Redo();
        }

        public void Reset()
        {
            _invoker.ClearHistory();
            foreach (var o in _observers) o.OnHistoryCleared();
        }

        public CommandRecord? LastOperation() => _invoker.PeekHistory();
    }
}