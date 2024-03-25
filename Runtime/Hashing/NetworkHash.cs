using System;
using System.Linq;

namespace MoonriseGames.Connect.Hashing
{
    internal readonly struct NetworkHash
    {
        public byte[] Hash { get; }

        public bool IsValid => Hash != null && Hash.Length == NetworkHashing.HashSizeBytes;

        public NetworkHash(byte[] hash) => Hash = hash;

        public NetworkHash(string encoding) => Hash = Convert.FromBase64String(encoding);

        public string ToBase64() => Convert.ToBase64String(Hash);

        public override bool Equals(object obj)
        {
            if (obj is not NetworkHash other)
                return false;
            if (other.Hash == null || Hash == null)
                return other.Hash == Hash;
            return other.Hash.SequenceEqual(Hash);
        }

        public override int GetHashCode()
        {
            if (Hash == null)
                return 0;
            return Hash.Sum(x => x);
        }
    }
}
