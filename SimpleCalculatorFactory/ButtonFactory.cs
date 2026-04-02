using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCalculatorFactory
{
    #region Интерфейсы продуктов
    public interface IButton
    {
        string OnClick();
    }

    #endregion

    #region Конкретные продукты
    public class DigitButton : IButton
    {
        private int _digit;
        public DigitButton(int digit)
        {
            if (digit < 0 || digit > 9) { throw new ArgumentOutOfRangeException(); }
            _digit = digit;
        }
        public string OnClick() => _digit.ToString();
    }
    public class OperatorButton : IButton
    {
        private string _operator;
        public OperatorButton(string operator_) => _operator = operator_;
        public string OnClick() => _operator.ToString();
    }
    public class EqualsButton : IButton
    {
        public string OnClick() => "=";
    }
    public class ClearButton : IButton
    {
        public string OnClick() => "C";
    }
    public class PointButton : IButton
    {
        public string OnClick() => ".";
    }

    #endregion

    #region Интерфейс фабрики
    public interface IButtonFactory
    {
        IButton CreateDigitButton(int digit);
        IButton CreateOperatorButton(string operator_);
        IButton CreateEqualsButton();
        IButton CreateClearButton();
        IButton CreatePointButton();

        List<IButton> GetAllButtons();
    }

    #endregion

    #region Конретные фабрики
    public class CommonCalculatorFactory : IButtonFactory
    {
        List<IButton>? _buttonsList = null;
        public IButton CreateDigitButton(int digit) => new DigitButton(digit);
        public IButton CreateOperatorButton(string operator_) => new OperatorButton(operator_);
        public IButton CreateEqualsButton() => new EqualsButton();
        public IButton CreateClearButton() => new ClearButton();
        public IButton CreatePointButton() => new PointButton();

        public List<IButton> GetAllButtons()
        {
            if (this._buttonsList != null) { return this._buttonsList; }

            string[] operators = { "+", "-", "*", "/" };
            _buttonsList = new List<IButton>();

            for (int i = 0; i < 10; ++i)
            {
                _buttonsList.Add(CreateDigitButton(i));
            }
            foreach (var operotor in operators)
            {
                _buttonsList.Add(CreateOperatorButton(operotor));
            }

            _buttonsList.Add(CreateEqualsButton());
            _buttonsList.Add(CreateClearButton());
            _buttonsList.Add(CreatePointButton());

            return _buttonsList;
        }
    }

    #endregion
}