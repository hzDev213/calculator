using SimpleCalculatorMVVM.Services.Calculators;

namespace SimpleCalculatorMVVM.Commands.MainViewCommands
{
    internal class OperatorButtonClickCommand : Command
    {
        CalculatorEngine _calculator;

        Action<string> _setText;
        Func<string> _getText;
        Action<string> _setHistory;
        OperatorButtonClickCommand(Action<string> SetText, Func<string> GetText, Action<string> SetHistory)
        {
            _calculator = new CalculatorEngine();

            _getText = GetText;
            _setText = SetText;
            _setHistory = SetHistory;
        }
        public override bool CanExecute(object? p) => true;
        public override void Execute(object? p)
        {
            string[] operators = { "+", "-", "*", "/" };

            string? operator_ = p?.ToString();
            if (operator_ != null)
            {
                if (!operators.Any(op => _getText().Contains(op)))
                {
                    _setText(_getText() == "0" ? "0" : _getText() + operator_);
                }
                else
                {
                    _setHistory(_getText());
                    _setText(_calculator.Calculate(_getText()) + operator_);
                }
            }
        }
    }
}
