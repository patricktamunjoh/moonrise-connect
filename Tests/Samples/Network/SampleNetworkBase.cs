using MoonriseGames.Connect.Enums;
using MoonriseGames.Connect.Functions;
using MoonriseGames.Connect.Objects;
using MoonriseGames.Connect.Tests.Samples.Collections;

namespace MoonriseGames.Connect.Tests.Samples.Network
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
