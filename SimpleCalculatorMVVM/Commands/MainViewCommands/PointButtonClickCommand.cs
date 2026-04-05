using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculatorMVVM.Commands.MainViewCommands
{
    public class PointButtonClickCommand : Command
    {
        private Func<string> _getResult;
        private Action<string> _setResult;

        public PointButtonClickCommand(Func<string> GetResult,Action<string> SetResult)
        {
            _getResult = GetResult;
            _setResult = SetResult;
        }

        public override bool CanExecute(object? p) => true;
        public override void Execute(object? p)
        {
            _setResult(_getResult() + '.');
        }
    }
}
