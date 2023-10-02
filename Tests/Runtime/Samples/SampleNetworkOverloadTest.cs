using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Network;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Assertions;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Samples {
    public class SampleNetworkOverloadTest {

        [Test]
        public void ShouldCorrectlyTrackNetworkFunctionInvocations() {
            var sut = new SampleNetworkOverload();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleNetworkOverload.NetworkFunction));
        }

        [Test]
        public void ShouldCorrectlyTrackNetworkFunctionOverloadInvocations() {
            var sut = new SampleNetworkOverload();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleNetworkOverload.NetworkFunction), "example");
        }
    }
}
