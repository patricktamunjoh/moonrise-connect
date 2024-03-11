using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Network;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Extensions;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Factories
{
    internal static class NetworkFunctionDelegateFactory
    {
        public static NetworkFunctionDelegate Build()
        {
            var sample = new SampleNetwork();
            return Build(sample, nameof(SampleNetwork.NetworkFunction));
        }

        public static NetworkFunctionDelegate Build(object target, string methodName)
        {
            var function = new NetworkFunction(Groups.All, Recipients.All);
            return Build(target, methodName, function);
        }

        public static NetworkFunctionDelegate Build(NetworkFunction function)
        {
            var sample = new SampleNetwork();
            return Build(sample, nameof(SampleNetwork.NetworkFunction), function);
        }

        public static NetworkFunctionDelegate Build(object target, string methodName, NetworkFunction function)
        {
            var methodInfo = target.GetType().GetDeclaredMethod(methodName);
            return new NetworkFunctionDelegate(target, new NetworkFunctionData(function, methodInfo));
        }
    }
}
