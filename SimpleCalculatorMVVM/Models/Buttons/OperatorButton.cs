using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculatorMVVM.Models.Buttons
{
    internal class OperatorButton : IButton
    {
        private string _operator;
        public OperatorButton(string operatoration) => _operator = operatoration;
        public string OnClick() => _operator;
        public string DisplayTitle() => _operator;
    }
}
