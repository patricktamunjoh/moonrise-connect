using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace MoonriseGames.Connect.Hashing
{
    internal static class NetworkHashing
    {
        private static MD5 HashFunction { get; } = MD5.Create();
        public static int HashSizeBytes { get; } = HashFunction.HashSize / 8;

        public static NetworkHash Hash(MethodInfo method)
        {
            var path = $"{method.DeclaringType?.FullName ?? ""}@{method.Name}";
            return Hash(path);
        }

        public static NetworkHash Hash(string data)
        {
            var bytes = Encoding.UTF8.GetBytes(data);
            return new NetworkHash(HashFunction.ComputeHash(bytes));
        }
    }
}
