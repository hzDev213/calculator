using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleCalculatorMVVM.Services.Calculators;

namespace SimpleCalculatorMVVM.Commands.MainViewCommands
{
    public class EqualsButtonClickButton : Command
    {
        CalculatorEngine _calculator;

        private Func<string> _getResult;
        private Action<string> _setResult;
        private Func<string> _getHistory;
        private Action<string> _setHistory;
        public EqualsButtonClickButton(Func<string> GetResult, Action<string> SetResult, Func<string> GetHistory, Action<string> SetHistory)
        {
            _calculator = new CalculatorEngine();

            _getResult = GetResult;
            _setResult = SetResult;
            _getHistory = GetHistory;
            _setHistory = SetHistory;
        }

        public override bool CanExecute(object? p) => true;
        public override void Execute(object? p)
        {
            _setHistory(_getResult());
            _setResult(_calculator.Calculate(_getHistory()));
        }
    }
}
