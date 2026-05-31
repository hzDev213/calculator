namespace SimpleCalculatorMVVMDecorator.Services.Calculators
{
    public class EngineDecorator : ICalculator
    {
        private ICalculator _wrappee;
        EngineDecorator(ICalculator wrappee) => _wrappee = wrappee;

        virtual public string Calculate(string expression)
        {
            return _wrappee.Calculate(expression);
        }
    }
}
