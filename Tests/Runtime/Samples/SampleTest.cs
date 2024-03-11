using MoonriseGames.CloudsAhoyConnect.Tests.Samples.Object;
using MoonriseGames.CloudsAhoyConnect.Tests.Utilities.Assertions;
using NUnit.Framework;

namespace MoonriseGames.CloudsAhoyConnect.Tests.Samples
{
    public class SampleTest
    {
        [Test]
        public void ShouldCorrectlyTrackPublicFunctionInvocations()
        {
            var sut = new Sample();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(Sample.PublicFunction));
        }

        [Test]
        public void ShouldCorrectlyTrackPrivateFunctionInvocations()
        {
            var sut = new Sample();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, Sample.PrivateFunctionName);
        }

        [Test]
        public void ShouldCorrectlyTrackVirtualFunctionInvocations()
        {
            var sut = new Sample();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(Sample.VirtualFunction));
        }
    }
}
