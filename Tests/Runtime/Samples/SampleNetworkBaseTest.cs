using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Network;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Assertions;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Samples
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
