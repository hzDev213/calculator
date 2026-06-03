using System;
using System.Collections.Generic;

namespace Calculator.Patterns.Command
{
    /// <summary>
    /// Invoker — управляет историей команд.
    /// Хранит стеки выполненных и отменённых операций (Undo/Redo).
    /// </summary>
    public class CommandInvoker
    {
        private readonly Stack<CommandRecord> _history = new Stack<CommandRecord>();
        private readonly Stack<CommandRecord> _redoStack = new Stack<CommandRecord>();

        public bool CanUndo => _history.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>Выполнить команду и сохранить в историю</summary>
        public double ExecuteCommand(ICalculatorCommand command, double operandA, double operandB)
        {
            double result = command.Execute(operandA, operandB);
            _history.Push(new CommandRecord(command, operandA, operandB, result));
            _redoStack.Clear();   // новое действие сбрасывает redo-стек
            return result;
        }

        /// <summary>Отменить последнюю команду</summary>
        public double Undo()
        {
            if (!CanUndo) throw new InvalidOperationException("История пуста");
            var record = _history.Pop();
            double undone = record.Command.Undo(record.Result, record.OperandB);
            _redoStack.Push(record);
            return undone;
        }

        /// <summary>Повторить отменённую команду</summary>
        public double Redo()
        {
            if (!CanRedo) throw new InvalidOperationException("Нет операций для повтора");
            var record = _redoStack.Pop();
            double result = record.Command.Execute(record.OperandA, record.OperandB);
            _history.Push(new CommandRecord(record.Command, record.OperandA, record.OperandB, result));
            return result;
        }

        public void ClearHistory()
        {
            _history.Clear();
            _redoStack.Clear();
        }

        public CommandRecord? PeekHistory() => _history.Count > 0 ? _history.Peek() : null;
    }

    /// <summary>Запись команды в истории</summary>
    public class CommandRecord
    {
        public ICalculatorCommand Command { get; }
        public double OperandA { get; }
        public double OperandB { get; }
        public double Result { get; }

        public CommandRecord(ICalculatorCommand command, double a, double b, double result)
        {
            Command = command;
            OperandA = a;
            OperandB = b;
            Result = result;
        }

        public override string ToString() =>
            $"{OperandA} {Command.OperationSymbol} {OperandB} = {Result}";
    }
}