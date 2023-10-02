using System;
using System.Linq;
using System.Reflection;
using MoonriseGames.CloudsAhoyConnect.Payloads;

namespace MoonriseGames.CloudsAhoyConnect.Functions {
    internal class NetworkFunctionData {

        public NetworkFunction Function { get; }
        public MethodInfo MethodInfo { get; }

        public Type[] ParameterTypes { get; }
        public Type[] ParameterAndReturnTypes { get; }

        public NetworkPayloadType PayloadType { get; }

        public NetworkFunctionData(NetworkFunction function, MethodInfo methodInfo) {
            Function = function;
            MethodInfo = methodInfo;

            ParameterTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray();
            ParameterAndReturnTypes = ParameterTypes.Append(methodInfo.ReturnType).ToArray();

            PayloadType = new NetworkPayloadType(ParameterTypes);
        }
    }
}
