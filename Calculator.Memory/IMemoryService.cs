namespace Calculator.Memory
{
    public interface IMemoryService
    {
        void   MemoryStore(double value);
        void   MemoryAdd(double value);
        void   MemorySubtract(double value);
        double MemoryRecall();
        void   MemoryClear();
        bool   HasValue { get; }
    }
}