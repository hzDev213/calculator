using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculatorMVVM.Commands.MainViewCommands
{
    public class ClearButtonClickCommand : Command
    {
        private Action<string> _setResult;
        private Action<string> _setHistory;
        ClearButtonClickCommand(Action<string> SetResult, Action<string> SetHistory)
        {
            _setResult = SetResult;
            _setHistory = SetHistory;
        }

        public override bool CanExecute(object? p) => true;
        public override void Execute(object? p)
        {
            _setResult("0");
            _setHistory("");
        }
    }
}
