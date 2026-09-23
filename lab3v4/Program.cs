using System;
using System.Collections.Generic;

namespace Lab3
{
    public class MemoryCache : IDisposable
    {
        private bool _disposed = false;
        private bool _isActive;
        private Dictionary<string, string>? _cacheData;

        public bool IsActive => _isActive;

        public MemoryCache()
        {
            _cacheData = new Dictionary<string, string>();
            _isActive = true;
            Console.WriteLine("Кеш створено, ресурс активний.");
        }

        public void Set(string key, string value)
        {
            if (_disposed || !_isActive || _cacheData == null)
                throw new ObjectDisposedException(nameof(MemoryCache), "Кеш закрито або знищено.");

            _cacheData[key] = value;
            Console.WriteLine($"Записано: {key} = {value}");
        }

        public string? Get(string key)
        {
            if (_disposed || !_isActive || _cacheData == null)
                throw new ObjectDisposedException(nameof(MemoryCache), "Кеш закрито або знищено.");

            return _cacheData.TryGetValue(key, out var value) ? value : null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Console.WriteLine("Очищення керованого ресурсу.");
                    _cacheData?.Clear();
                    _cacheData = null;
                }

                if (_isActive)
                {
                    Console.WriteLine("Деактивація ресурсу.");
                    _isActive = false;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        ~MemoryCache()
        {
            Console.WriteLine("Спрацював деструктор через GC!");
            Dispose(false);
        }
    }

    class Program
    {
        static void Main()
        {
            Console.WriteLine("Сценарій 1: Використання оператора using");
            using (var cache1 = new MemoryCache())
            {
                cache1.Set("user_1", "Артем");
                Console.WriteLine($"Отримано з кешу: {cache1.Get("user_1")}");
            }

            Console.WriteLine("\nСценарій 2: Виклик Dispose()");
            var cache2 = new MemoryCache();
            cache2.Set("session_token", "XYZ12345");
            cache2.Dispose();

            Console.WriteLine("\nСценарій 3: Робота деструктора через GC");
            CreateAndForgetObject();

            Console.WriteLine("Виклик Збирача Сміття (GC.Collect)...");
            GC.Collect();
            GC.WaitForPendingFinalizers();

    }

        static void CreateAndForgetObject()
        {
            var cache3 = new MemoryCache();
            cache3.Set("temp_key", "value");
        }
    }
}
