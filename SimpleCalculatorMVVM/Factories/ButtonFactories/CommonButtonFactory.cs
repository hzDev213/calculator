using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleCalculatorMVVM.Models.Buttons;

namespace SimpleCalculatorMVVM.Factories.ButtonFactories
{
    internal class CommonButtonFactory : IButtonFactory
    {

        public IButton CreateDigitButton(int digit) => new DigitButton(digit);
        public IButton CreateOperatorButton(string operator_) => new OperatorButton(operator_);
        public IButton CreateEqualsButton() => new EqualsButton();
        public IButton CreateClearButton() => new ClearButton();
        public IButton CreatePointButton() => new PointButton();
        public IButton CreateDeleteLastSymbolButton() => new DeleteLastSymbolButton();

        public List<IButton> GetAllButtons()
        {
            string[] operators = { "+", "-", "*", "/" };
            var buttonsList = new List<IButton>();

            for (int i = 0; i < 10; ++i)
            {
                buttonsList.Add(CreateDigitButton(i));
            }
            foreach (var operotor in operators)
            {
                buttonsList.Add(CreateOperatorButton(operotor));
            }

            buttonsList.Add(CreateEqualsButton());
            buttonsList.Add(CreateDeleteLastSymbolButton());
            buttonsList.Add(CreateClearButton());
            buttonsList.Add(CreatePointButton());

            return buttonsList;
        }
    }
}