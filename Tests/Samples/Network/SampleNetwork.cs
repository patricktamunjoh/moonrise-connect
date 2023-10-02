using MoonriseGames.CloudsAhoyConnect.Enums;
using MoonriseGames.CloudsAhoyConnect.Functions;
using MoonriseGames.CloudsAhoyConnect.Objects;
using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Collections;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Samples.Network {

    [NetworkObject]
    public class SampleNetwork : SampleNetworkBase {

        public new InvocationCounter InvocationCounter { get; } = new();

        public override void NetworkFunctionOnlyBase() => InvocationCounter.RecordInvocation(nameof(NetworkFunctionOnlyBase));

        [NetworkFunction(Groups.All, Recipients.All)]
        public override void NetworkFunction() => InvocationCounter.RecordInvocation(nameof(NetworkFunction));

        [NetworkFunction(Groups.All, Recipients.All)]
        public void NiladicFunction() => InvocationCounter.RecordInvocation(nameof(NiladicFunction));

        [NetworkFunction(Groups.All, Recipients.All)]
        public void MonadicFunction(int a) => InvocationCounter.RecordInvocation(nameof(MonadicFunction), a);

        [NetworkFunction(Groups.All, Recipients.All)]
        public void DyadicFunction(int a, int b) => InvocationCounter.RecordInvocation(nameof(DyadicFunction), a, b);

        [NetworkFunction(Groups.All, Recipients.All)]
        public void TriadicFunction(int a, int b, int c) => InvocationCounter.RecordInvocation(nameof(TriadicFunction), a, b, c);

        [NetworkFunction(Groups.All, Recipients.All)]
        public void QuadradicFunction(int a, int b, int c, int d) =>
            InvocationCounter.RecordInvocation(nameof(QuadradicFunction), a, b, c, d);

        [NetworkFunction(Groups.All, Recipients.All)]
        public void NetworkObjectFunction(SampleNetworkEmpty obj) => InvocationCounter.RecordInvocation(nameof(NetworkObjectFunction), obj);

        public void RegularFunction() => InvocationCounter.RecordInvocation(nameof(RegularFunction));
    }
}
