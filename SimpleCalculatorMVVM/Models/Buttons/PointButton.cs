using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculatorMVVM.Models.Buttons
{
    internal class PointButton : IButton
    {
        public string DisplayTitle() => ".";
        public string OnClick() => ".";
    }
}
