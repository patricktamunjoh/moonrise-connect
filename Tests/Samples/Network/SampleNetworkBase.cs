using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Objects;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Collections;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Samples.Network
{
    [NetworkObject]
    public class SampleNetworkBase
    {
        public InvocationCounter InvocationCounter { get; } = new();

        [NetworkFunction(Groups.Host, Recipients.All)]
        public virtual void NetworkFunctionOnlyBase() => InvocationCounter.RecordInvocation(nameof(NetworkFunctionOnlyBase));

        [NetworkFunction(Groups.Host, Recipients.All)]
        public virtual void NetworkFunction() => InvocationCounter.RecordInvocation(nameof(NetworkFunction));
    }
}
