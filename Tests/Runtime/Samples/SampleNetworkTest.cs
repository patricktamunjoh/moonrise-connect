using MoonriseGames.Connect.Tests.Samples.Network;
using MoonriseGames.Connect.Tests.Utilities.Assertions;
using NUnit.Framework;

namespace MoonriseGames.Connect.Tests.Samples
{
    public class SampleNetworkTest
    {
        [Test]
        public void ShouldCorrectlyTrackNetworkFunctionOnlyBaseFunctionInvocations()
        {
            var sut = new SampleNetwork();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleNetwork.NetworkFunctionOnlyBase));
        }

        [Test]
        public void ShouldCorrectlyTrackNetworkFunctionInvocations()
        {
            var sut = new SampleNetwork();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleNetwork.NetworkFunction));
        }

        [Test]
        public void ShouldCorrectlyTrackNiladicFunctionInvocations()
        {
            var sut = new SampleNetwork();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleNetwork.NiladicFunction));
        }

        [Test]
        public void ShouldCorrectlyTrackMonadicFunctionInvocations()
        {
            var sut = new SampleNetwork();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleNetwork.MonadicFunction), 1);
        }

        [Test]
        public void ShouldCorrectlyTrackDyadicFunctionInvocations()
        {
            var sut = new SampleNetwork();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleNetwork.DyadicFunction), 1, 2);
        }

        [Test]
        public void ShouldCorrectlyTrackTriadicFunctionInvocations()
        {
            var sut = new SampleNetwork();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleNetwork.TriadicFunction), 1, 2, 3);
        }

        [Test]
        public void ShouldCorrectlyTrackQuadradicFunctionInvocations()
        {
            var sut = new SampleNetwork();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleNetwork.QuadradicFunction), 1, 2, 3, 4);
        }

        [Test]
        public void ShouldCorrectlyTrackNetworkObjectFunctionInvocations()
        {
            var data = new SampleNetworkEmpty();
            var sut = new SampleNetwork();

            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleNetwork.NetworkObjectFunction), data);
        }

        [Test]
        public void ShouldCorrectlyTrackRegularFunctionInvocations()
        {
            var sut = new SampleNetwork();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleNetwork.RegularFunction));
        }
    }
}
