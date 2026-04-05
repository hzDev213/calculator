namespace SimpleCalculatorMVVM.Commands.MainViewCommands
{
    public class DigitButtonClickCommand : Command
    {
        private Action<string> _displayText;
        private Func<string> _getText;
        public DigitButtonClickCommand(Func<string> GetText, Action<string> DisplayText) 
        {
            _displayText = DisplayText;
            _getText = GetText;
        }

        public override bool CanExecute(object? p) => true;
        public override void Execute(object? p)
        {
            string? digit = p?.ToString();
            if (digit != null)
            {
                _displayText(_getText() == "0" ? digit : _getText() + digit);
            }
        }
    }
}
