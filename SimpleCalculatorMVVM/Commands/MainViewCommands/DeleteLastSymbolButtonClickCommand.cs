using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculatorMVVM.Commands.MainViewCommands
{
    public class DeleteLastSymbolButtonClickCommand : Command
    {
        private Func<string> _getResult;
        private Action<string> _setResult;
        DeleteLastSymbolButtonClickCommand(Func<string> GetResult, Action<string> SetResult)
        {
            _getResult = GetResult;
            _setResult = SetResult;
        }

        public override bool CanExecute(object? p) => true;
        public override void Execute(object? p)
        {
            if (_getResult().Length == 1)
            {
                _setResult("0");
            }
            else
            {
                _setResult(_getResult().Substring(0, _getResult().Length - 1));
            }
        }
    }
}
