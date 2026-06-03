using System.Globalization;

namespace Calculator.Evaluator
{
    public class EnginePiDecorator : EngineDecorator
    {
        public EnginePiDecorator(ICalculator inner) : base(inner) { }

        public override string Calculate(string expression)
        {
            string processed = expression.Replace("π",
                Math.PI.ToString(CultureInfo.InvariantCulture));
            return _inner.Calculate(processed);
        }
    }
}