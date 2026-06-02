namespace Calculator.Patterns.Command
{
    /// <summary>
    /// Поведенческий паттерн Command (Команда).
    /// Инкапсулирует математическую операцию как объект,
    /// поддерживая отмену (Undo) и повтор (Redo).
    /// </summary>
    public interface ICalculatorCommand
    {
        /// <summary>Выполнить команду</summary>
        double Execute(double a, double b);

        /// <summary>Отменить команду (обратная операция)</summary>
        double Undo(double result, double b);

        /// <summary>Название операции для отображения</summary>
        string OperationSymbol { get; }
    }
}