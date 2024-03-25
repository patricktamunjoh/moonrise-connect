using MoonriseGames.Connect.Tests.Samples.Network;
using MoonriseGames.Connect.Tests.Utilities.Assertions;
using NUnit.Framework;

namespace MoonriseGames.Connect.Tests.Samples
{
    public class SampleNetworkOverloadTest
    {
        [Test]
        public void ShouldCorrectlyTrackNetworkFunctionInvocations()
        {
            var sut = new SampleNetworkOverload();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleNetworkOverload.NetworkFunction));
        }

        [Test]
        public void ShouldCorrectlyTrackNetworkFunctionOverloadInvocations()
        {
            var sut = new SampleNetworkOverload();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleNetworkOverload.NetworkFunction), "example");
        }
    }
}
