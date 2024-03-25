using MoonriseGames.Connect.Enums;
using MoonriseGames.Connect.Functions;
using MoonriseGames.Connect.Objects;
using MoonriseGames.Connect.Tests.Samples.Collections;

namespace MoonriseGames.Connect.Tests.Samples.Network
{
    [NetworkObject]
    public class SampleNetworkOverload
    {
        public InvocationCounter InvocationCounter { get; } = new();

        [NetworkFunction(Groups.Host, Recipients.All)]
        public void NetworkFunction() => InvocationCounter.RecordInvocation(nameof(NetworkFunction));

        [NetworkFunction(Groups.Host, Recipients.All)]
        public void NetworkFunction(string a) => InvocationCounter.RecordInvocation(nameof(NetworkFunction), a);
    }
}
