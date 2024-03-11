using System.Collections.Generic;
using System.Reflection;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Samples.Collections
{
    public class InvocationCounter
    {
        private Dictionary<string, List<object[]>> Invocations { get; } = new();

        public bool RecordInvocation(string name, params object[] args)
        {
            Invocations.TryAdd(name, new List<object[]>());
            Invocations[name].Add(args);
            return true;
        }

        public int InvocationCount(MethodInfo method) => InvocationCount(method.Name);

        public int InvocationCount(string method) => Invocations.TryGetValue(method, out var args) ? args.Count : 0;

        public object[] Arguments(string method, int invocation) => Invocations[method][invocation];
    }
}
