using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Collections;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Samples.Object {

    public class Sample : SampleBase {

        public new InvocationCounter InvocationCounter { get; } = new();

        public new static string PrivateFunctionName => nameof(PrivateFunction);


        public new void PublicFunction() => InvocationCounter.RecordInvocation(nameof(PublicFunction));

        private void PrivateFunction() => InvocationCounter.RecordInvocation(nameof(PrivateFunction));

        public override void VirtualFunction() => InvocationCounter.RecordInvocation(nameof(VirtualFunction));
    }
}
