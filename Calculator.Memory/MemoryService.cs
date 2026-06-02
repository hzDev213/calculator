namespace Calculator.Memory
{
    public class MemoryService : IMemoryService
    {
        private double _stored = 0;
        public bool HasValue { get; private set; } = false;

        public void   MemoryStore(double v)    { _stored = v;  HasValue = true; }
        public void   MemoryAdd(double v)      { _stored += v; HasValue = true; }
        public void   MemorySubtract(double v) { _stored -= v; HasValue = true; }
        public double MemoryRecall()           => _stored;
        public void   MemoryClear()            { _stored = 0;  HasValue = false; }
    }
}