namespace SimpleCalculatorIniFile.Models.Buttons
{
    internal class ConstButton : IButton
    {
        private string _symbol;
        public ConstButton(string symbol) => _symbol = symbol;
        public string OnClick() => _symbol;
        public string DisplayTitle() => _symbol;
    }
}