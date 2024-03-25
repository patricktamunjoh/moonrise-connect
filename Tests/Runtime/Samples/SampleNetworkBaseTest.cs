using MoonriseGames.Connect.Tests.Samples.Network;
using MoonriseGames.Connect.Tests.Utilities.Assertions;
using NUnit.Framework;

namespace MoonriseGames.Connect.Tests.Samples
{
    public class SampleNetworkBaseTest
    {
        [Test]
        public void ShouldCorrectlyTrackNetworkFunctionOnlyBaseInvocations()
        {
            var sut = new SampleNetworkBase();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleNetworkBase.NetworkFunctionOnlyBase));
        }

        [Test]
        public void ShouldCorrectlyTrackNetworkFunctionInvocations()
        {
            var sut = new SampleNetworkBase();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleNetworkBase.NetworkFunction));
        }
    }
}
