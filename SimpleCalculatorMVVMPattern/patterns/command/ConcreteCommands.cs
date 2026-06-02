namespace Calculator.Patterns.Command
{
    // ──────────────────────────────────────────────
    // Конкретные команды (Concrete Commands)
    // ──────────────────────────────────────────────

    public class AddCommand : ICalculatorCommand
    {
        public string OperationSymbol => "+";
        public double Execute(double a, double b) => a + b;
        public double Undo(double result, double b) => result - b;
    }

    public class SubtractCommand : ICalculatorCommand
    {
        public string OperationSymbol => "−";
        public double Execute(double a, double b) => a - b;
        public double Undo(double result, double b) => result + b;
    }

    public class MultiplyCommand : ICalculatorCommand
    {
        public string OperationSymbol => "×";
        public double Execute(double a, double b) => a * b;
        public double Undo(double result, double b) => b != 0 ? result / b : double.NaN;
    }

    public class DivideCommand : ICalculatorCommand
    {
        public string OperationSymbol => "÷";
        public double Execute(double a, double b) => b != 0 ? a / b : double.NaN;
        public double Undo(double result, double b) => result * b;
    }
}