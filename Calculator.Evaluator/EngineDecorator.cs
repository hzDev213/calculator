namespace Calculator.Evaluator
{
    public abstract class EngineDecorator : ICalculator
    {
        protected readonly ICalculator _inner;
        protected EngineDecorator(ICalculator inner) { _inner = inner; }
        public virtual string Calculate(string expression)
            => _inner.Calculate(expression);
    }
}