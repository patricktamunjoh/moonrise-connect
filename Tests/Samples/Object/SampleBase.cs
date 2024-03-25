using MoonriseGames.Connect.Tests.Samples.Collections;

namespace MoonriseGames.Connect.Tests.Samples.Object
{
    public class SampleBase : ISample
    {
        public InvocationCounter InvocationCounter { get; } = new();

        public static string PrivateFunctionName => nameof(PrivateFunction);

        public void NiladicFunction() => InvocationCounter.RecordInvocation(nameof(NiladicFunction));

        public void MonadicFunction(string a) => InvocationCounter.RecordInvocation(nameof(MonadicFunction), a);

        public void DyadicFunction(int a, bool b) => InvocationCounter.RecordInvocation(nameof(DyadicFunction), a, b);

        public bool ReturningFunction() => InvocationCounter.RecordInvocation(nameof(ReturningFunction));

        public void PublicFunction() => InvocationCounter.RecordInvocation(nameof(PublicFunction));

        private void PrivateFunction() => InvocationCounter.RecordInvocation(nameof(PrivateFunction));

        public virtual void VirtualFunction() => InvocationCounter.RecordInvocation(nameof(VirtualFunction));
    }
}
