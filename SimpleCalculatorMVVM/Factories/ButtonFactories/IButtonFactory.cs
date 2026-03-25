using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleCalculatorMVVM.Models.Buttons;

namespace SimpleCalculatorMVVM.Factories.ButtonFactories
{
    internal interface IButtonFactory
    {
        IButton CreateDigitButton(int digit);
        IButton CreateOperatorButton(string operator_);
        IButton CreateEqualsButton();
        IButton CreateClearButton();
        IButton CreatePointButton();

        List<IButton> GetAllButtons();
    }
}
