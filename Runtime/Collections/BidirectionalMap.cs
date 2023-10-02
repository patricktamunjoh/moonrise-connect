using System.Collections.Generic;

namespace MoonriseGames.CloudsAhoyConnect.Collections {

    internal class BidirectionalMap<THead, TTail> {

        private Dictionary<THead, TTail> Head { get; } = new();
        private Dictionary<TTail, THead> Tail { get; } = new();

        public int Count => Head.Count;

        public TTail this[THead key] { get => Head[key]; set => Write(key, value); }

        public THead this[TTail key] { get => Tail[key]; set => Write(value, key); }

        public bool Contains(THead key) => Head.ContainsKey(key);
        public bool Contains(TTail key) => Tail.ContainsKey(key);

        public bool Remove(THead key) {
            if (!Contains(key)) return false;
            Remove(key, Head[key]);
            return true;
        }

        public bool Remove(TTail key) {
            if (!Contains(key)) return false;
            Remove(Tail[key], key);
            return true;
        }

        public void Clear() {
            Head.Clear();
            Tail.Clear();
        }

        private void Write(THead item1, TTail item2) {
            if (Contains(item1)) Remove(item1);
            if (Contains(item2)) Remove(item2);

            Head[item1] = item2;
            Tail[item2] = item1;
        }

        private void Remove(THead item1, TTail item2) {
            Head.Remove(item1);
            Tail.Remove(item2);
        }
    }
}
