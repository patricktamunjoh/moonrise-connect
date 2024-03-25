using MoonriseGames.Connect.Tests.Samples.Object;
using MoonriseGames.Connect.Tests.Utilities.Assertions;
using NUnit.Framework;

namespace MoonriseGames.Connect.Tests.Samples
{
    public class SampleBaseTest
    {
        [Test]
        public void ShouldCorrectlyTrackNiladicFunctionInvocations()
        {
            var sut = new SampleBase();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleBase.NiladicFunction));
        }

        [Test]
        public void ShouldCorrectlyTrackMonadicFunctionInvocations()
        {
            var sut = new SampleBase();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleBase.MonadicFunction), "example");
        }

        [Test]
        public void ShouldCorrectlyTrackDyadicFunctionInvocations()
        {
            var sut = new SampleBase();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleBase.DyadicFunction), 12, true);
        }

        [Test]
        public void ShouldCorrectlyTrackFunctionWithReturnValueInvocations()
        {
            var sut = new SampleBase();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleBase.ReturningFunction));
        }

        [Test]
        public void ShouldCorrectlyTrackPublicFunctionInvocations()
        {
            var sut = new SampleBase();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleBase.PublicFunction));
        }

        [Test]
        public void ShouldCorrectlyTrackPrivateFunctionInvocations()
        {
            var sut = new SampleBase();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, SampleBase.PrivateFunctionName);
        }

        [Test]
        public void ShouldCorrectlyTrackVirtualFunctionInvocations()
        {
            var sut = new SampleBase();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleBase.VirtualFunction));
        }
    }
}
