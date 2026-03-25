using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculatorMVVM.Models.Buttons
{
    internal class DigitButton : IButton
    {
        private int _digit;
        public DigitButton(int digit) => _digit = digit;
        public string OnClick() => _digit.ToString();
        public string DisplayTitle() => _digit.ToString();
    }
}
