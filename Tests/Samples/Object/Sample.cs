using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Collections;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Samples.Object
{
    public class Sample : SampleBase
    {
        public new InvocationCounter InvocationCounter { get; } = new();

        public static new string PrivateFunctionName => nameof(PrivateFunction);

        public new void PublicFunction() => InvocationCounter.RecordInvocation(nameof(PublicFunction));

        private void PrivateFunction() => InvocationCounter.RecordInvocation(nameof(PrivateFunction));

        public override void VirtualFunction() => InvocationCounter.RecordInvocation(nameof(VirtualFunction));
    }
}
