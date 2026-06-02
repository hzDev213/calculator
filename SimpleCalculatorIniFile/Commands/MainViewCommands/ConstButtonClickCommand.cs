namespace SimpleCalculatorIniFile.Commands.MainViewCommands
{
    public class ConstButtonClickCommand : Command
    {
        private Func<string> _getResult;
        private Action<string> _setResult;
        public ConstButtonClickCommand(Func<string> GetResult, Action<string> SetResult)
        {
            _getResult = GetResult;
            _setResult = SetResult;
        }

        public override bool CanExecute(object? p) => true;
        public override void Execute(object? p)
        {
            string[] operators = { "+", "-", "*", "/" };

            string? symbol = p?.ToString();
            if (symbol != null)
            {
                int length = _getResult().Length;

                if (_getResult() == "0")
                {
                    _setResult(symbol);
                    return;
                }

                foreach (var operator_ in operators)
                {
                    if (_getResult()[length - 1].ToString() == operator_)
                    {
                        _setResult(_getResult() + symbol);
                        return;
                    }
                }
            }
        }
    }
}
