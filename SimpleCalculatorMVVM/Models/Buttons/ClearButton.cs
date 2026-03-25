using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculatorMVVM.Models.Buttons
{
    internal class ClearButton : IButton
    {
        public string OnClick() => "";
        public string DisplayTitle() => "C";
    }
}
