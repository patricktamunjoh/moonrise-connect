using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Objects;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Collections;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Samples.Network {

    [NetworkObject]
    public class SampleNetworkOverload {

        public InvocationCounter InvocationCounter { get; } = new();


        [NetworkFunction(Groups.Host, Recipients.All)]
        public void NetworkFunction() => InvocationCounter.RecordInvocation(nameof(NetworkFunction));

        [NetworkFunction(Groups.Host, Recipients.All)]
        public void NetworkFunction(string a) => InvocationCounter.RecordInvocation(nameof(NetworkFunction), a);
    }
}
