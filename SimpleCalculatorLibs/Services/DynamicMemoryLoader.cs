using System.Reflection;
using System.IO;
using System;
namespace SimpleCalculatorLibs.Services
{
    public class DynamicMemoryLoader
    {
        private object? _instance;
        private Type?   _type;

        public bool IsLoaded => _instance != null;

        public bool Load()
        {
            try
            {
                string dllPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Calculator.Memory.dll");

                if (!File.Exists(dllPath)) return false;

                var asm   = Assembly.LoadFrom(dllPath);
                _type     = asm.GetType("Calculator.Memory.MemoryService");
                _instance = Activator.CreateInstance(_type!);
                return true;
            }
            catch { return false; }
        }

        public void   Store(double v)    => Invoke("MemoryStore", v);
        public void   Add(double v)      => Invoke("MemoryAdd", v);
        public void   Subtract(double v) => Invoke("MemorySubtract", v);
        public void   Clear()            => Invoke("MemoryClear");
        public double Recall()
        {
            var r = _type?.GetMethod("MemoryRecall")?.Invoke(_instance, null);
            return r is double d ? d : 0;
        }

        private void Invoke(string method, object? arg = null)
        {
            var args = arg != null ? new[] { arg } : null;
            _type?.GetMethod(method)?.Invoke(_instance, args);
        }
    }
}