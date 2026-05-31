namespace SimpleCalculatorMVVMDecorator.Models.Buttons
{
    public interface IButton
    {
        string OnClick();
        string DisplayTitle();
    }
}