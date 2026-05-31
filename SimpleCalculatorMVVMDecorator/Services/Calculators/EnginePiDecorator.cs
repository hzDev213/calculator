using System.Globalization;

namespace SimpleCalculatorMVVMDecorator.Services.Calculators
{
    public class EnginePiDecorator : EngineDecorator
    {
        public EnginePiDecorator(ICalculator wrappee) : base(wrappee)
        {
        }

        public override string Calculate(string expression)
        {
            return base.Calculate(expression.Replace("π", Math.PI.ToString("F6", CultureInfo.InvariantCulture)));
        }
    }
}
