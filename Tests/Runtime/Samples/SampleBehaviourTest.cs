using MoonriseGames.Connect.Tests.Samples.Object;
using MoonriseGames.Connect.Tests.Utilities.Assertions;
using NUnit.Framework;
using UnityEngine;

namespace MoonriseGames.Connect.Tests.Samples
{
    public class SampleBehaviourTest
    {
        [Test]
        public void ShouldCorrectlyTrackNetworkFunctionInvocations()
        {
            var sut = new GameObject().AddComponent<SampleBehaviour>();
            SampleAssertions.VerifyInvocations(sut, sut.InvocationCounter, nameof(SampleBehaviour.NetworkFunction));
        }
    }
}
