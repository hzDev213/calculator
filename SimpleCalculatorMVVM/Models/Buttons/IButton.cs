using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculatorMVVM.Models.Buttons
{
    public interface IButton
    {
        string OnClick();
        string DisplayTitle();
    }
}
