using System;
using System.Collections.Generic;

namespace MoonriseGames.CloudsAhoyConnect.Collections
{
    internal class DoubleKeyMap<TKey1, TKey2, TValue>
    {
        private Dictionary<TKey1, Dictionary<TKey2, TValue>> Map { get; } = new();

        public TValue this[TKey1 key1, TKey2 key2]
        {
            get => Read(key1, key2);
            set => Write(key1, key2, value);
        }

        public bool Contains(TKey1 key) => Map.ContainsKey(key);

        public bool Contains(TKey1 key1, TKey2 key2)
        {
            if (key1 == null || key2 == null)
                throw new ArgumentNullException();
            return Map.TryGetValue(key1, out var inner) && inner.ContainsKey(key2);
        }

        public bool Remove(TKey1 key) => Map.Remove(key);

        public bool Remove(TKey1 key1, TKey2 key2)
        {
            if (!Contains(key1, key2))
                return false;
            return Map[key1].Remove(key2);
        }

        public void Clear() => Map.Clear();

        private void Write(TKey1 key1, TKey2 key2, TValue value)
        {
            if (!Map.TryGetValue(key1, out var inner))
            {
                inner = new Dictionary<TKey2, TValue>();
                Map[key1] = inner;
            }

            inner[key2] = value;
        }

        private TValue Read(TKey1 key1, TKey2 key2)
        {
            if (!Map.TryGetValue(key1, out var inner))
                inner = new Dictionary<TKey2, TValue>();
            return inner[key2];
        }
    }
}
