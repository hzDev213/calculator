namespace Calculator.Patterns.Observer
{
    /// <summary>
    /// Паттерн Observer — вспомогательный, для уведомлений об изменении состояния калькулятора.
    /// </summary>
    public interface ICalculatorObserver
    {
        void OnCalculationPerformed(double result, string expression);
        void OnError(string errorMessage);
        void OnHistoryCleared();
    }

    public interface ICalculatorSubject
    {
        void Attach(ICalculatorObserver observer);
        void Detach(ICalculatorObserver observer);
    }
}