namespace SimpleCalculatorMVVMDecorator.Services.Calculators
{
    public class EngineDecorator : ICalculator
    {
        protected ICalculator _wrappee;
        public EngineDecorator(ICalculator wrappee) => _wrappee = wrappee;

        virtual public string Calculate(string expression)
        {
            return _wrappee.Calculate(expression);
        }
    }
}
